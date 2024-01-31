//Britton Bailer, January 2024

using Microsoft.AspNetCore.Mvc;

namespace PillPallAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly MyDbContext _dbContext;

    public ScheduleController(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    [Route("getSchedules")]
    public IActionResult GetSchedules()
    {        
        var schedules = _dbContext.Schedules;
        var scheduleResponses = new List<ScheduleResponse>();

        foreach(Schedule schedule in schedules){
            var scheduleResponse = new ScheduleResponse();

            scheduleResponse.Id = schedule.Id;
            scheduleResponse.UserId = schedule.UserId;

            var scheduleMeds = _dbContext.ScheduleMeds.Where(entity => entity.ScheduleId == schedule.Id).ToList();
            scheduleResponse.ScheduleMeds = scheduleMeds;

            var timeIds = _dbContext.ScheduleTimes.Where(entity => entity.ScheduleId == schedule.Id).Select(entity => entity.TimeId);
            var times = _dbContext.Times.Where(entity => timeIds.Contains(entity.Id)).ToList();

            scheduleResponse.Times = times;

            scheduleResponses.Add(scheduleResponse);
        }

        Console.WriteLine(scheduleResponses.Count);

        return Ok(scheduleResponses);
    }
    
}
