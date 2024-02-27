//Britton Bailer, January 2024

//any requests coming into the api dealing with dispensing come into the api via this file

/*

    1. DispenseSchedule(schedule)


*/

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
    [Route("dispenseSchedule")]
    public IActionResult DispenseSchedule([FromBody] Schedule schedule)
    {
        //get schedule id, container id, and num pills for each of the requested pills
        var containersToDispense = _dbContext.ContainerMedMaps.Join(_dbContext.ScheduleMeds, medMapTable => medMapTable.MedId, scheduleMedTable => scheduleMedTable.MedicationId, 
            (medMapTable, scheduleMedTable) => new DispenseContainer{
                scheduleId = (int)scheduleMedTable.ScheduleId!,
                containerId = medMapTable.ContainerId,
                numPills = scheduleMedTable.NumPills
            }
        ).Where(entity => entity.scheduleId == schedule.Id);

        //initialize dispense array with 0s
        var dispenseArray = new int[]{0, 0, 0, 0, 0, 0};

        //fill dispense array with the number of each pill requested
        foreach(DispenseContainer containerToDispense in containersToDispense){
            dispenseArray[containerToDispense.containerId] = containerToDispense.numPills;
        }

        //thank you justin.  Send request to arduino communicator
        try {
            ArduinoCommunicator comm = new ArduinoCommunicator();
            comm.SendRequest(2, dispenseArray);
        }
        catch(Exception e) {
            return BadRequest("Failed to communicate with device. " + e.Message);
        }

        return Ok();
    }

    [HttpPost]
    [Route("dispense")]
    public IActionResult DispenseMedication([FromBody] int[] dispenseList)
    {
        ArduinoCommunicator comm = new ArduinoCommunicator();
        comm.SendRequest(2, new int[] {135,136,137,138,139,140});

        return Ok();
    }
    
}
