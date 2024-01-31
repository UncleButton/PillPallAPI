//Britton Bailer, January 2024

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

        return Ok();
    }
    
}
