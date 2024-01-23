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

    [HttpGet(Name = "GetPillInformation")]
    public IActionResult Get()
    {
        var entities = _dbContext.Users.ToList();
        return Ok(entities);
    }
}
