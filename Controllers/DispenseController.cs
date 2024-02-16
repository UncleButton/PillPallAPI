//Britton Bailer, January 2024

using Microsoft.AspNetCore.Mvc;
using PillPallAPI.ArduinoCommunication;
using SQLitePCL;

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
        ArduinoCommunicator comm = new ArduinoCommunicator();
        comm.SendRequest(2, new int[] {135,136,137,138,139,140});
        //JUSTIN'S AREA

        //PUT LOGIC HERE THAT DOES THE ACTUAL DISPENSING/USES YOUR DEVICE CONTROLLERS

        return Ok();
    }
    
}
