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
            sp.ReadTimeout = 500;

            // Open the port
            sp.Open();

            // Read data from the port until the user presses a key
            Console.WriteLine("Reading data from GPS device...");
            Console.WriteLine("Press any key to stop.");
            while (!Console.KeyAvailable)
            {
                try
                {
                    // Read a line of data
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
