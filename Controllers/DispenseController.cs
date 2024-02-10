//Britton Bailer, January 2024

using System;
using System.IO.Ports;
using Microsoft.AspNetCore.Mvc;

namespace PillPallAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class DispenseController : ControllerBase
{
    private readonly MyDbContext _dbContext;

    public DispenseController(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    [Route("dispense")]
    public IActionResult DispenseMedication([FromBody] int[] dispenseList)
    {
        //JUSTIN'S AREA

        //PUT LOGIC HERE THAT DOES THE ACTUAL DISPENSING/USES YOUR DEVICE CONTROLLERS

        SerialPort sp = new SerialPort("COM7", 9600, Parity.None, 8, StopBits.One);

        // Set the read timeout to 5 s
        sp.ReadTimeout = 5000;

        // Open the port
        sp.Open();

        // Array with preset values is sent to the arduino, which sums the values and writes back the result
        byte[] disps = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
        while (!Console.KeyAvailable)
        {
            try
            {
                Console.WriteLine("Trying to Write...");
                // Write the array to the arduino via the serial port. "disps" is the array of bytes, 0 is the offset (leave at 0), and 10 is the amount of bytes
                sp.Write(disps, 0, 10);
                // Read the result back from the arduino
                string data = sp.ReadLine();

                // Display the data
                Console.WriteLine(data);
            }
            catch (TimeoutException)
            {
                // Ignore timeout errors
            }
        }

        // Close the port
        sp.Close();

        return Ok();
    }
}

// This is the code the arduino had at the time of success...

// void setup()
// {
//   delay(3000);
//   Serial.begin(9600);
// }

// void loop()
// {
//     if (Serial.available())
//     {
//         int sum = 0;
//         for (int i = 0; i < 10; i++)
//         {
//             sum += Serial.read();
//         }
//         Serial.println(sum);
//     }
//     delay(1000);
// }