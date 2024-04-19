// ArduinoCommunicator.cs

// Acts as the middle-man between the API and the Arduino. The API sends the request for the Arduino to perform certain tasks here,
// and this file forwards that request to the Arduino adn awaits a "success" acknowledgement.

// Justin Feldmann, February 2024

using System.IO.Ports;
using Microsoft.VisualBasic;

namespace PillPallAPI.ArduinoCommunication;

public static class ArduinoCommunicator
{
    const byte REQUEST_REFILL = 1;
    const byte REQUEST_DISPENSE = 2;
    const byte REQUEST_OFFSET = 6;  // The two-bit request needs to be shifted to bits 6 and 7 of the first byte in the message
    const byte MAX_TRANSFER_ATTEMPTS = 5;   // MAX number of times we try to send the data before determining some failure
    const byte RECEIVED_VALID_MESSAGE = 6;   // If the message was a success, we return a 0. Don't ask why it's a char, it just is
    const byte RECEIVED_INVALID_MESSAGE = 7;   // If the message was a fail, we return a 1. Don't ask why it's a char, it just is
    const byte TOF_NOT_INITIALIZED = 8;   // TOF sensor had error while trying to initialize
    const byte HEARTBEAT = 9;   // Heartbeat, still communicating with Arduino correctly
    const byte DISPENSE_SUCCESS = 10;    // Successfully dispensed a pill
    const byte DISPENSE_FAIL = 11;   // Did not successfully dispense
    const byte FINISHED_SUCCESS = 12;   // Finished dispensing or refilling correctly
    const byte FINISHED_FAIL = 13;   // Finished dispensing or refilling correctly
    const string PI_PORT_NAME = "/dev/ttyACM0";    // This is the port used for the serial communication, CHANGE FOR PI
    const string JUSTIN_PORT_NAME = "COM7";    // This is the port used for the serial communication, CHANGE FOR PI
    static int num = 0;

    // Create new port with pre-determined port name
    static SerialPort sp = new SerialPort(PI_PORT_NAME, 9600, Parity.None, 8, StopBits.One);

    public static void OpenCommunication(){
        if(!sp.IsOpen){
            Console.WriteLine("Opening communication");
            if (SerialPort.GetPortNames().Contains(JUSTIN_PORT_NAME))
                sp.PortName = JUSTIN_PORT_NAME;

            // Set the read and write timeouts to 5 s (probably will be increased or changed depending on the request)
            sp.WriteTimeout = 5000;
            sp.ReadTimeout = 5000;

            // Open the port
            sp.Open();
        }
    }

    public static void CloseCommunication(){
        // Close the port
        sp.Close();
    }

    // Takes in the request type and the data being sent and sends them as a message to the Arduino
    public static (bool, int[]) SendRequest(int request, int[,] data)
    {
        num++;
        Console.WriteLine("Dispense: " + num);
        if(!sp.IsOpen){
            OpenCommunication();
        }

        // Message array being sent to Arduino. Needs to be the length of data + 2 so the first byte can hold the request and the length of the
        // message, and the final byte holds the checksum.
        byte[] message = new byte[data.Length + 2];

        // bits 7:6 hold the two request bits, and the remaining bits hold the length of the data in bytes
        message[0] = (byte) ((request << REQUEST_OFFSET) + data.Length);

        //  Response array will initially hold the number of pills needed to be dispensed from each cartridge. As each pill is dispensed, the
        //  corresponding number here will be decremented. If all are decremented to 0, the entire dispense was a success
        int[] response = new int[data.Length / 2];

        // The checksum is a method used to ensure valid data is received on the Arduino.
        byte chkSum = message[0];
        if (request == REQUEST_DISPENSE)
        {
            for (byte i = 0; i < data.Length / 2; i++)
            {
                message[(2*i)+1] = (byte) data[i, 0];
                message[(2*i)+2] = (byte) data[i, 1];
                response[i] = data[i, 0];
                // Console.WriteLine(i + ": " + "[" + message[(2*i)+1] + "," + message[(2*i)+2] + "]");
                chkSum += message[(2*i)+1];
                chkSum += message[(2*i)+2];
            }
        }
        else if (request == REQUEST_REFILL)
        {
            message[1] = (byte) data[0, 0];
            chkSum += message[1];
        }
        // Console.WriteLine("ChkSum: " + chkSum);
        message[data.Length+1] = chkSum;

        sp.ReadTimeout = 5000;
        if (sp.BytesToRead > 0 && sp.ReadByte() == TOF_NOT_INITIALIZED)
        {
            Console.WriteLine("Error when trying to initialize the Time of Flight sensor");
            return (false, response);
        }

        // We will only allow for 5 attempts to transfer the data correctly. If we fail 5 times, something must be wrong with either the data or
        // the communication line
        byte failedAttempts = 0;
        int readResult = -1;
        while (failedAttempts < MAX_TRANSFER_ATTEMPTS)
        {
            try
            {
                // Need to clear both in and out buffers, as there might still be data there from a previous attempt
                sp.DiscardInBuffer();
                sp.DiscardOutBuffer();
                Thread.Sleep(500);
                Console.WriteLine("Buffer is: " + sp.ReadExisting());

                // Write the array to the arduino via the serial port. "message" is the array of bytes, 0 is the offset (leave at 0), and
                // message.Length indicates how many bytes to send.
                sp.Write(message, 0, message.Length);
                // while (true)
                //     Console.WriteLine(sp.ReadLine());
                // Now wait on the success response from the Arduino
                readResult = sp.ReadByte();

                if (readResult == RECEIVED_VALID_MESSAGE)
                    break;
                
                Console.WriteLine("Received CORRUPTED message");
                failedAttempts++;
            }
            catch (Exception) // TimeoutExceptions don't work?...so general one for now
            {
                Console.WriteLine("Timeout on reading back a success indication");
                failedAttempts++;
            }
        }

        if (readResult == RECEIVED_VALID_MESSAGE)
        {
            Console.WriteLine("Successful message");
            sp.ReadTimeout = 30000;
            try
            {
                do
                {
                    readResult = sp.ReadByte();
                    // Console.WriteLine("result is: " + readResult);
                    if (readResult == DISPENSE_SUCCESS)
                    {
                        readResult = sp.ReadByte();
                        // Console.WriteLine("result is: " + readResult);
                        response[readResult]--;
                        readResult = sp.ReadByte();
                        // Console.WriteLine("result is: " + readResult);
                    }
                }
                while (readResult == HEARTBEAT);
            }
            catch (Exception)
            {
                Console.WriteLine("Timeout on Dispense/Refill");
                return (false, response);
            }

            if (readResult == FINISHED_SUCCESS)
            {
                Console.WriteLine("Successfully Dispensed/Refilled");
                for (int i = 0; i < 6; i++)
                    Console.Write(response[i] + " ");
                return (true, response);
            }
            else
            {
                Console.WriteLine("Unsuccessfully Dispensed/Refilled");
                return (false, response);
            }   
        }
        Console.WriteLine("Unsuccessful message");
        return (false, response);
    }

    public static (bool, int[]) Refill(int containerId){
        //refill is of the form: (1, [containerId])
        return SendRequest(REQUEST_REFILL, new int[,]{{containerId}});
    }

    public static (bool, int[]) Dispense(int[,] dispenseArray){
        //refill is of the form: (2, [num from container 0, num from container 1..., num from container 5])
        return SendRequest(REQUEST_DISPENSE, dispenseArray);
    }
}