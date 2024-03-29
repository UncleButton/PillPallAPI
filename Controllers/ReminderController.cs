//Britton Bailer, January 2024

//any requests coming into the api dealing with reminders come into the api via this file

/*

    1. RemindSchedule(schedule)

*/

using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace PillPallAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ReminderController : ControllerBase
{
    private readonly MyDbContext _dbContext;
    private SmtpClient client = new SmtpClient("smtp-mail.outlook.com", 587)
    {
        Credentials = new NetworkCredential("pillpalmachine@outlook.com", "sxouibditeeflwup"),
        EnableSsl = true
    };
    private MailMessage message = new MailMessage();
    public ReminderController(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Sends reminder to user that a schedule is coming in 30 minutes.
    /// </summary>
    /// <param name="schedule"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("sendReminder")]
    public IActionResult Reminder([FromBody] Reminder reminder)
    {
        int attempts = 0;
        int maxRetries = 3;
        while (attempts < maxRetries)
        {
            try
            {               
                message.From = new MailAddress("pillpalmachine@outlook.com");

                if(!string.IsNullOrWhiteSpace(reminder.ToAddress))
                    message.To.Add(reminder.ToAddress);
                else
                    return BadRequest("No email address listed");//shouldnt happen

                message.Subject = reminder.Subject;
                message.Body = reminder.Body;

                client.Send(message);
                return Ok();
            }
            catch (Exception)
            {
                attempts++;
                Thread.Sleep(1000);
                if (attempts == maxRetries)
                {
                    throw new InvalidOperationException("Failed to send email after multiple attempts.");
                }
            }
        }
        return Ok();
    }    
}
