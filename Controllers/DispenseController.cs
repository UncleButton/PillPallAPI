//Britton Bailer, January 2024

//any requests coming into the api dealing with dispensing come into the api via this file

/*

    1. DispenseSchedule(schedule)
    2. DispenseCustom(scheduleMeds[])

*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    /// <summary>
    /// Sends communication to the device to dispense pills based on the schedule that was passed into this endpoint.
    /// </summary>
    /// <param name="schedule"></param>
    /// <returns></returns>
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
            comm.Dispense(dispenseArray);
        }
        catch(Exception e) {
            return BadRequest("Failed to communicate with device. " + e.Message);
        }

        //add dispense log
        foreach(ScheduleMed scheduleMed in schedule.ScheduleMeds!){
            var dispenseLog = new DispenseLog{
                MedId = scheduleMed.MedicationId,
                NumPills = scheduleMed.NumPills,
                ScheduleId = schedule.Id,
                Timestamp = DateTime.Now
            };
            _dbContext.DispenseLogs.Add(dispenseLog);
        }
        

        foreach(ScheduleMed scheduleMed in schedule.ScheduleMeds!){
            //subtract pills from tracked numPills
            updateMedNumPills((int)scheduleMed.MedicationId!, scheduleMed.NumPills);
        }

        _dbContext.SaveChanges();

        return Ok();
    }

    /// <summary>
    /// Sends communication to the device to dispense pills based on the list of ScheduleMeds passed into this endpoint.
    /// </summary>
    /// <param name="scheduleMeds"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("dispenseCustom")]
    public IActionResult DispenseCustom([FromBody] ScheduleMed[] scheduleMeds)
    {
        //initialize dispense array with 0s
        var dispenseArray = new int[]{0, 0, 0, 0, 0, 0};

        //fill dispense array with the number of each pill requested
        foreach(ScheduleMed scheduleMed in scheduleMeds){
            var container = _dbContext.ContainerMedMaps.Where(entity => entity.MedId == scheduleMed.Medication!.Id).FirstOrDefault();

            if(container == null)
                return BadRequest("Encountered a problem mapping the medication to the container.");

            dispenseArray[container.ContainerId] = scheduleMed.NumPills; 

            //add dispense log
            var dispenseLog = new DispenseLog{
                MedId = scheduleMed.Medication?.Id,
                NumPills = scheduleMed.NumPills,
                Timestamp = DateTime.Now
            };
            _dbContext.DispenseLogs.Add(dispenseLog);

            //subtract pills from tracked numPills
            updateMedNumPills(scheduleMed.Medication!.Id, scheduleMed.NumPills);
        }

        //thank you justin.  Send request to arduino communicator
        try {
            ArduinoCommunicator comm = new ArduinoCommunicator();
            comm.Dispense(dispenseArray);
        }
        catch(Exception e) {
            throw new Exception("Failed to communicate with device. " + e.Message);
        }

        _dbContext.SaveChanges();

        return Ok();
    }

    /// <summary>
    /// Returns a list of all dispense logs.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("getDispenseLogs")]
    public IActionResult GetDispenseLogs()
    {        
        var verboseDispenseLogs = new List<VerboseDispenseLog>();

        //get all dispense logs in order from most recent to least recent
        var dispenseLogs = _dbContext.DispenseLogs;

        if(dispenseLogs.ToList().Count == 0)
            return Ok(verboseDispenseLogs);

        foreach(DispenseLog dispenseLog in dispenseLogs){
            var verboseDispenseLog = new VerboseDispenseLog
            {
                Timestamp = dispenseLog.Timestamp,
                NumPills = dispenseLog.NumPills
            };

            var med = _dbContext.Medications.FirstOrDefaultAsync(entity => entity.Id == dispenseLog.MedId);
            verboseDispenseLog.MedName = med.Result == null ? "Error" : med.Result.Name;

            var schedule = _dbContext.Schedules.FirstOrDefaultAsync(entity => entity.Id == dispenseLog.ScheduleId);
            verboseDispenseLog.ScheduleName = schedule.Result == null ? "Custom Dispense" : schedule.Result.Name;

            verboseDispenseLogs.Add(verboseDispenseLog);
        }

        //return all verbose dispense logs
        return Ok(verboseDispenseLogs.OrderByDescending(entity => entity.Timestamp));
    }

    [HttpPost]
    [Route("dispense")]
    public IActionResult DispenseMedication([FromBody] int[] dispenseList)
    {
        ArduinoCommunicator comm = new ArduinoCommunicator();
        comm.Dispense(new int[] {135,136,137,138,139,140});

        return Ok();
    }

    private void updateMedNumPills(int medId, int numPills){
        var med = _dbContext.Medications.Where(entity => entity.Id == medId).First();
        med.NumPills -= numPills;
    }
    
}
