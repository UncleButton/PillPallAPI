//Britton Bailer, January 2024

using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    [Route("getAllContainers")]
    public IActionResult GetCurrentMedication()
    {        
        var containersIds = _dbContext.Containers.Select(container => container.Id).ToList();

        var currentMedIds = _dbContext.ContainerMedMaps.GroupBy(entity => entity.ContainerId) // Group by ContainerId
            .Select(group => group.OrderByDescending(entity => entity.Id).First()) // Select the highest from each group
            .Select(entity => entity.MedId);

        var meds = _dbContext.Medications.Where(meds => currentMedIds.Contains(meds.Id) && meds.NumPills > 0).ToList();

        return Ok(meds);
    }

    [HttpPost]
    [Route("newMedication")]
    public async Task<IActionResult> NewMedication([FromBody] Medication newMedication)
    {
        Console.WriteLine(newMedication.Id);
        
        //medication is new
        if(newMedication.Id == -1){
            var containersIds = _dbContext.Containers.Select(container => container.Id).ToList();

            var currentMedIds = _dbContext.ContainerMedMaps.GroupBy(entity => entity.ContainerId) // Group by ContainerId
                .Select(group => group.OrderByDescending(entity => entity.Id).First()) // Select the highest from each group
                .Select(entity => entity.MedId);

            var nonZeroMedIds = _dbContext.Medications.Where(meds => currentMedIds.Contains(meds.Id) && meds.NumPills > 0).ToList().Select(entity => entity.Id);

            var containersInUse = _dbContext.ContainerMedMaps.Where(entity => nonZeroMedIds.Contains(entity.MedId)).Select(entity => entity.ContainerId);

            var containersNotInUse = _dbContext.Containers.Where(entity => !containersInUse.Contains(entity.Id)).Select(entity => entity.Id);

            var newLargestMedId = _dbContext.Medications.OrderByDescending(entity => entity.Id).First().Id;

            newLargestMedId++;

            ContainerMedMap newContainerMedMap = new ContainerMedMap() { 
                    ContainerId = containersNotInUse.First(), 
                    MedId = newLargestMedId
                };

            Console.WriteLine(containersNotInUse.First());

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
    
}
