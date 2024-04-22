//Britton Bailer, January 2024

//any requests dealing with medication come into the api through this file

/*

    1. GetCurrentMedication()
    2. SaveMedication(medication)

*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PillPallAPI.ArduinoCommunication;

namespace PillPallAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PillInformationController : ControllerBase
{
    private readonly MyDbContext _dbContext;

    public PillInformationController(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Returns a list of all current medications.  (Medications in the database without any pills left are NOT included)
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("getAllContainers")]
    public IActionResult GetCurrentMedication()
    {        
        //this gets all current medications

        //get all current medication ids
        var currentMedIds = _dbContext.ContainerMedMaps.GroupBy(entity => entity.ContainerId) // Group by ContainerId
            .Select(group => group.OrderByDescending(entity => entity.Id).First()) // Select the highest from each group
            .Select(entity => entity.MedId);

        //only keep ones with more than 0 pills
        var meds = _dbContext.Medications.Where(meds => currentMedIds.Contains(meds.Id) && meds.NumPills > 0);

        //return all current, non-empty medications for display
        return Ok(meds);
    }

    /// <summary>
    /// Saves a new medication or updates the medication if it already exists in the database.
    /// </summary>
    /// <param name="newMedication"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("saveMedication")]
    public async Task<IActionResult> SaveMedication([FromBody] Medication newMedication)
    {
        Console.WriteLine(newMedication.Id);
        
        //medication is new
        if(newMedication.Id == 0){
            var containersIds = _dbContext.Containers.Select(container => container.Id).ToList();
            Console.WriteLine("containers count: " + containersIds.Count());

            var currentMeds = _dbContext.ContainerMedMaps;

            var containerToFill = 0;
            var newLargestMedId = 0;
            if(currentMeds.Any()) // not first med being added to machine
            {
                //get all current medication ids
                var currentMedIds = currentMeds.GroupBy(entity => entity.ContainerId) // Group by ContainerId
                    .Select(group => group.OrderByDescending(entity => entity.Id).First()) // Select the highest from each group
                    .Select(entity => entity.MedId);

                //keep only the ones that are non-zero
                var nonZeroMedIds = _dbContext.Medications.Where(meds => currentMedIds.Contains(meds.Id) && meds.NumPills > 0).ToList().Select(entity => entity.Id);

                //get containers that are currently in use
                var containersInUse = _dbContext.ContainerMedMaps.Where(entity => nonZeroMedIds.Contains(entity.MedId)).Select(entity => entity.ContainerId);

                //get a container NOT in use
                containerToFill = _dbContext.Containers.Where(entity => !containersInUse.Contains(entity.Id)).Select(entity => entity.Id).First();

                //get the current largest medication id
                var largestMed = _dbContext.Medications.OrderByDescending(entity => entity.Id).FirstOrDefault();

                //incrememnt that to get what our new med id will be
                newLargestMedId = largestMed == null ? 0 : largestMed.Id;
            }
            
            newLargestMedId++;

            //create new med map that maps one of the unused containers with the new med id
            ContainerMedMap newContainerMedMap = new ContainerMedMap() { 
                ContainerId = containerToFill, 
                MedId = newLargestMedId
            };

            //thank you justin.  Send request to arduino communicator (fill/refill conatinerNotInUse)
            try {
                ArduinoCommunicator.Refill(containerToFill);
            }
            catch(Exception e){
                //return BadRequest("Failed to communicate with machine. " + e.Message);
            }

            //set new medication id and save both the medication and the container med map (so we know which container the medication is in)
            newMedication.Id = newLargestMedId;
            _dbContext.Medications.Add(newMedication);
            _dbContext.ContainerMedMaps.Add(newContainerMedMap);
        }
        else //medication is being edited
        {
            _dbContext.Medications.Update(newMedication);
        }

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    /// <summary>
    /// Refills a medication. Moves the cartrige over to the correct spot for refilling and updates the medication 'numPills' in the database.
    /// </summary>
    /// <param name="refillObject"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("refill")]
    public async Task<IActionResult> Refill([FromBody] RefillObject refillObject)
    {
        //move cartrige to opening
        //thank you justin.  Send request to arduino communicator (fill/refill conatinerNotInUse)
        var container = await _dbContext.ContainerMedMaps.Where(entity => entity.MedId == refillObject.MedicationId).OrderByDescending(entity => entity.id).FirstAsync();
        Console.WriteLine("REFILLING CONTAINER ID: " + container.ContainerId);
        try {
            ArduinoCommunicator.Refill(container.ContainerId);
        }
        catch(Exception e){
            Console.WriteLine(e);
            return BadRequest("Failed to communicate with machine. " + e.Message);
        }

        //add pills to medication
        if(refillObject.MedicationId != -1 && refillObject.Qty > 0){
            var refillMedication = await _dbContext.Medications.Where(entity => entity.Id == refillObject.MedicationId).FirstOrDefaultAsync();
            refillMedication!.NumPills += refillObject.Qty;
        } else {
            return BadRequest("Something went wrong!");
        }

        //return
        await _dbContext.SaveChangesAsync();
        Console.WriteLine("Refilling container: " + refillObject.ContainerId);
        return Ok();
    }

    [HttpPost]
    [Route("deleteMedication")]
    public async Task<IActionResult> DeleteMedication([FromBody] Medication medication)
    {     
        //get reference to existing medication
        var existingMed = _dbContext.Medications.Where(entity => entity.Id == medication.Id).FirstOrDefault();

        //return bad request if no such medication was found
        if(existingMed == null)
            return BadRequest();

        //remove medication
        existingMed.NumPills = 0;

        //save
        await _dbContext.SaveChangesAsync();

        return Ok();
    }  
}
