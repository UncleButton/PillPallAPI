// ArduinoCommunicator.cs

// Acts as the middle-man between the API and the Arduino. The API sends the request for the Arduino to perform certain tasks here,
// and this file forwards that request to the Arduino adn awaits a "success" acknowledgement.

// Justin Feldmann, February 2024

using System.IO.Ports;
using Microsoft.VisualBasic;

namespace PillPallAPI.ArduinoCommunication;

public static class ArduinoCommunicator
{
    const byte REQUEST_OFFSET = 6;  // The two-bit request needs to be shifted to bits 6 and 7 of the first byte in the message
    const byte MAX_TRANSFER_ATTEMPTS = 5;   // MAX number of times we try to send the data before determining some failure
    const char MESSAGE_SUCCESS = '0';   // If the message was a success, we return a 0. Don't ask why it's a char, it just is
    const string PORT_NAME = "/dev/ttyACM0";    // This is the port used for the serial communication, CHANGE FOR PI
    static int num = 0;

    // Create new port with pre-determined port name
    static SerialPort sp = new SerialPort(PORT_NAME, 9600, Parity.None, 8, StopBits.One);

    public static void OpenCommunication(){
        if(!sp.IsOpen){
            Console.WriteLine("Opening communication");

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
    public static bool SendRequest(int request, int[] data)
    {
        num++;
        Console.WriteLine("Dispense: " + num);
        if(!sp.IsOpen){
            //open communication before request
            OpenCommunication();
            return true;
        }
        

        // Message array being sent to Arduino. Needs to be the length of data + 2 so the first byte can hold the request and the length of the
        // message, and the final byte holds the checksum.
        byte[] message = new byte[data.Length + 2];

        // bits 7:6 hold the two request bits, and the remaining bits hold the length of the data in bytes
        message[0] = (byte) ((request << REQUEST_OFFSET) + data.Length);

        // The checksum is a method used to ensure valid data is received on the Arduino.
        byte chkSum = message[0];
        for (byte i = 0; i < data.Length; i++)
        {
            message[i+1] = (byte) data[i];
            chkSum += message[i+1];
        }
        message[data.Length+1] = chkSum;

        // We will only allow for 5 attempts to transfer the data correctly. If we fail 5 times, something must be wrong with either the data or
        // the communication line
        byte failedAttempts = 0;
        while (failedAttempts < MAX_TRANSFER_ATTEMPTS)
        {
            try
            {
                // Need to clear both in and out buffers, as there might still be data there from a previous attempt
                sp.DiscardInBuffer();
                sp.DiscardOutBuffer();

                // Write the array to the arduino via the serial port. "message" is the array of bytes, 0 is the offset (leave at 0), and
                // message.Length indicates how many bytes to send.
                sp.Write(message, 0, message.Length);
                
                // Now wait on the success response from the Arduino
                int result = sp.ReadByte();

                if (result == MESSAGE_SUCCESS)
                {
                    Console.WriteLine("Received correct message");
                    sp.ReadTimeout = -1;
                    result = sp.ReadByte();
                    if (result == MESSAGE_SUCCESS)
                    {
                        Console.WriteLine("Dispensed all pills successfully");
                        break;
                    }
                    else {
                        sp.ReadTimeout = 5000;
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Received CORRUPTED message");
                    failedAttempts++;
                }
            }
            catch (Exception e) // TimeoutExceptions don't work?...so general one for now
            {
                Console.WriteLine("Timeout on reading back a success indication");
            }
        }
        // //  Close the port since we're done with this transaction
        // sp.Close();
        sp.ReadTimeout = 5000;

        return failedAttempts < MAX_TRANSFER_ATTEMPTS;
    }

    public static bool Refill(int containerId){
        //refill is of the form: (1, [containerId])
        return SendRequest(1, new int[]{containerId});
    }

    public static bool Dispense(int[] dispenseArray){
        //refill is of the form: (2, [num from container 0, num from container 1..., num from container 5])
        return SendRequest(2, dispenseArray);
    }
}