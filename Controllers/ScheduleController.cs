//Britton Bailer, January 2024

//all things dealing with schedules come into the api through this file

/*
    1. GetSchedules()
    2. SaveSchedule(schedlueMeds[])
    3. UpdateSchedule(schedule)
    4. DeleteSchedule(schedule)
*/

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
        //get all schedules        
        var schedules = _dbContext.Schedules;

        //loop through all schedules
        foreach(Schedule schedule in schedules){
            //get scheduleMeds for each of the schedules (and set it on each schedule)
            var scheduleMeds = _dbContext.ScheduleMeds.Where(entity => entity.ScheduleId == schedule.Id).ToList();
            schedule.ScheduleMeds = scheduleMeds;

            //get times for each of the schedules (and set it on each schedule)
            var timeIds = _dbContext.Times.Where(entity => entity.ScheduleId == schedule.Id).Select(entity => entity.Id).ToList();
            var times = _dbContext.Times.Where(entity => timeIds.Contains(entity.Id)).ToList();
            schedule.Times = times;
        }

        //return all schedules
        return Ok(schedules);
    }

    [HttpPost]
    [Route("saveSchedule")]
    public async Task<IActionResult> SaveSchedule([FromBody] ScheduleMed[] meds)
    {     
        //get each medication out of the scheduleMed object
        foreach(ScheduleMed med in meds){
            //get the reference to the existing medication (if it exists)
            var existingMed = _dbContext.Medications.Where(medication => med.Medication!.Id == medication.Id);
            if(existingMed.Any())
                med.Medication = existingMed.First();
            
            //get the referenced schedule if it exists
            var existingSchedule = _dbContext.Schedules.Where(schedule => med.Schedule!.Name.Equals(schedule.Name));

            if(existingSchedule.Any()) {
                med.Schedule = existingSchedule.First();
                med.ScheduleId = existingSchedule.First().Id;
            }
            
            //add new scheduleMed records (auto adds any nonexistent schedules)
            _dbContext.ScheduleMeds.Add(med);
            await _dbContext.SaveChangesAsync();
        }
        return Ok();
    }

    [HttpPost]
    [Route("updateSchedule")]
    public async Task<IActionResult> UpdateSchedule([FromBody] Schedule schedule)
    {     
        //get reference to exising schedule
        var existingSchedule = _dbContext.Schedules.Where(entity => entity.Id == schedule.Id).FirstOrDefault();

        //return bad request if no existing schedule
        if(existingSchedule == null)
            return BadRequest();

        //we only allow updating name and PIN (for now), so update
        existingSchedule.Name = schedule.Name;
        existingSchedule.PIN = schedule.PIN;

        //save changes
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("deleteSchedule")]
    public async Task<IActionResult> DeleteSchedule([FromBody] Schedule schedule)
    {     
        //get reference to existing schedule
        var existingSchedule = _dbContext.Schedules.Where(entity => entity.Id == schedule.Id).FirstOrDefault();

        //return bad request if no such schedule was found
        if(existingSchedule == null)
            return BadRequest();

        //remove schedule
        _dbContext.Remove(existingSchedule);

        //save
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    // private void messageMe(){
    //     var message = new MailMessage();
    //     message.From = new MailAddress("pillpalmachine@outlook.com");

    //     message.To.Add(new MailAddress("4407592699@tmomail.net"));

    //     //message.Subject = "This is my subject";
    //     message.Body = "this is the content";

    //     var client = new SmtpClient("smtp-mail.outlook.com", 587)
    //         {
    //             Credentials = new NetworkCredential("pillpalmachine@outlook.com", "sxouibditeeflwup"),
    //             EnableSsl = true
    //         };
    //     client.Send(message);

    // }
    
}
