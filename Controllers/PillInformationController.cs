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

    
}
