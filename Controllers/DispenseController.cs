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

            // Set the read timeout to 500 ms
            sp.ReadTimeout = 5000;

            // Open the port
            sp.Open();

            // Read data from the port until the user presses a key
            Console.WriteLine("Reading data from GPS device...");
            Console.WriteLine("Press any key to stop.");
            while (!Console.KeyAvailable)
            {
                try
                {
                    Console.WriteLine("Trying to Read...");
                    // Read a line of data
                    sp.WriteLine("5");
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

    // This is the code the arduino had at the time of success...

    // void setup() {
    //   Serial.begin(9600);

    // }

    // void loop() {
    //   if (Serial.available())
    //   {
    //     int num = Serial.read()
    //     // String str = Serial.readStringUntil(' ');
    //     Serial.println(str);
    //     // if (str == "5")
    //     //   Serial.println("Message DOUBLE received");
    //   }
    //   // Serial.println("Talking from Arduino");
    //   delay(1000);
    // }

}
