//Britton Bailer, January 2024

using System.Net;
using System.Net.Mail;
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
        //messageMe();
        
        var schedules = _dbContext.Schedules;
        var scheduleBuses = new List<ScheduleBus>();

        foreach(Schedule schedule in schedules){
            var scheduleBus = new ScheduleBus();

            scheduleBus.UserId = schedule.UserId;
            scheduleBus.Name = schedule.Name;
            scheduleBus.PIN = schedule.PIN;

            var scheduleMeds = _dbContext.ScheduleMeds.Where(entity => entity.ScheduleId == schedule.Id).ToList();
            scheduleBus.ScheduleMeds = scheduleMeds;

            var timeIds = _dbContext.ScheduleTimes.Where(entity => entity.ScheduleId == schedule.Id).Select(entity => entity.TimeId).ToList();
            var times = _dbContext.Times.Where(entity => timeIds.Contains(entity.Id)).ToList();

            scheduleBus.Times = times;

            scheduleBuses.Add(scheduleBus);
        }

        Console.WriteLine("Schedules: " + scheduleBuses.Count);

        return Ok(scheduleBuses);
    }

    [HttpPost]
    [Route("saveSchedule")]
    public async Task<IActionResult> SaveSchedule([FromBody] ScheduleBus newSchedule)
    {     
        //schedule is new
        if(newSchedule.Id == -1){
            var schedule = new Schedule(){
                UserId = newSchedule.UserId,
                Name = newSchedule.Name,
                PIN = newSchedule.PIN
            };
            _dbContext.Schedules.Add(schedule);

            _dbContext.Times.AddRange(newSchedule.Times);
            _dbContext.ScheduleMeds.AddRange(newSchedule.ScheduleMeds);
        }
        else //schedule is being edited
        {
            //to-do
        }

        await _dbContext.SaveChangesAsync();
        return Ok();
    }

    private void messageMe(){
        var message = new MailMessage();
        message.From = new MailAddress("pillpalmachine@outlook.com");

        message.To.Add(new MailAddress("4407592699@tmomail.net"));

        //message.Subject = "This is my subject";
        message.Body = "this is the content";

        var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                Credentials = new NetworkCredential("pillpalmachine@outlook.com", "sxouibditeeflwup"),
                EnableSsl = true
            };
        client.Send(message);

    }
    
}
