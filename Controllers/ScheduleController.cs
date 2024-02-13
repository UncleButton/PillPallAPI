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

            var timeIds = _dbContext.Times.Where(entity => entity.ScheduleId == schedule.Id).Select(entity => entity.Id).ToList();
            var times = _dbContext.Times.Where(entity => timeIds.Contains(entity.Id)).ToList();

            scheduleBus.Times = times;

            scheduleBuses.Add(scheduleBus);
        }

        Console.WriteLine("Schedules: " + scheduleBuses.Count);

        return Ok(scheduleBuses);
    }

    [HttpPost]
    [Route("saveSchedule")]
    public async Task<IActionResult> SaveSchedule([FromBody] ScheduleMed[] meds)
    {     
        //schedule is new
        //if(newSchedule.Id == -1){
            foreach(ScheduleMed med in meds){
                var existingMed = _dbContext.Medications.Where(medication => med.Medication.Id == medication.Id);
                if(existingMed.Any())
                    med.Medication = existingMed.First();
                
                var existingSchedule = _dbContext.Schedules.Where(schedule => med.Schedule.Name.Equals(schedule.Name));
                Console.WriteLine("matching schedules: " + existingSchedule.Count());
                if(existingSchedule.Any()) {
                    med.Schedule = existingSchedule.First();
                    med.ScheduleId = existingSchedule.First().Id;
                }
                    
                _dbContext.ScheduleMeds.Add(med);
                await _dbContext.SaveChangesAsync();
            }

            // foreach(Time time in newSchedule.Times){
            //     if(!time.DateTime.Equals("nana"))
            //         _dbContext.Times.Add(time);
            // }
            // _dbContext.ScheduleMeds.AddRange(newSchedule.ScheduleMeds);
        //}
        //else //schedule is being edited
        //{
            //to-do
        //}

        
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
