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
