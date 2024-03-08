//Britton Bailer, January 2024

//any requests coming into the api dealing with reminders come into the api via this file

/*

    1. RemindSchedule(schedule)

*/

using System.Net;
using System.Net.Mail;
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
    [Route("remindSchedule")]
    public IActionResult RemindSchedule([FromBody] Schedule schedule)
    {
        int attempts = 0;
        int maxRetries = 10;
        while (attempts < maxRetries)
        {
            try
            {               
                message.From = new MailAddress("pillpalmachine@outlook.com");

                message.To.Add(new MailAddress("britton.bailer@gmail.com"));

                message.Subject = "Scheduled Medication Reminder";
                message.Body = "Don't forget to take your meds! Your schedule \"" + schedule.Name + "\" is in 30 minutes.  Have a great day!";

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
