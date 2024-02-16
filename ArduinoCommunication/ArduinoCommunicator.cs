// ArduinoCommunicator.cs

// Acts as the middle-man between the API and the Arduino. The API sends the request for the Arduino to perform certain tasks here,
// and this file forwards that request to the Arduino adn awaits a "success" acknowledgement.

// Justin Feldmann, February 2024

using System.IO.Ports;

namespace PillPallAPI.ArduinoCommunication;

public class ArduinoCommunicator
{
    const byte REQUEST_OFFSET = 6;  // The two-bit request needs to be shifted to bits 6 and 7 of the first byte in the message


    public ArduinoCommunicator() {}

    // Takes in the request type and the data being sent and sends them as a message to the Arduino
    public bool SendRequest(int request, int[] data)
    {
        Console.WriteLine("Request is " + request);
        SerialPort sp = new SerialPort("COM7", 9600, Parity.None, 8, StopBits.One);

        // Set the read timeout to 5 s
        sp.ReadTimeout = 5000;

        // Open the port
        sp.Open();

        // Message array being sent to Arduino. Needs to be the length of data + 2 so the first byte can hold the request and the length of the
        // message, and the final byte holds the checksum.
        byte[] message = new byte[data.Length + 2];

        // bits 7:6 hold the two request bits, and the remaining bits hold the length of the data
        message[0] = (byte) ((request << REQUEST_OFFSET) + (byte) data.Length);
        Console.WriteLine("(request << REQUEST_OFFSET) = " + (request << REQUEST_OFFSET));
        Console.WriteLine("Request is now " + (int) message[0]);
        // Console.WriteLine((byte) 134);

        // The checksum is a method used to ensure valid data is received on the Arduino. 
        byte chkSum = message[0];
        for (byte i = 0; i < data.Length; i++)
        {
            message[i+1] = (byte) data[i];
            chkSum += message[i+1];
        }
        message[data.Length+1] = chkSum;
        foreach(var item in message)
        {
            Console.WriteLine((int) item);
        }

        while (!Console.KeyAvailable)
        {
            try
            {
                Console.WriteLine("Trying to Write stuff...");
                // Write the array to the arduino via the serial port. "disps" is the array of bytes, 0 is the offset (leave at 0), and 10 is the amount of bytes
                sp.Write(message, 0, message.Length);
                Thread.Sleep(1000);
                // Read the result back from the arduino
                string result = sp.ReadLine();
                // Display the data
                Console.WriteLine(result);

                // result = sp.ReadLine();
                // // Display the data
                // Console.WriteLine(result);
                break;
            }
            catch (TimeoutException)
            {
                Console.WriteLine("Didn't get data back");
            }
        }

        return true;
    }
}