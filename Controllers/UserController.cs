using Microsoft.AspNetCore.Mvc;

namespace PillPallAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly MyDbContext _dbContext;

    public UserController(MyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    [Route("getUser")]
    public IActionResult GetUser([FromBody] User userToGet)
    {
        var entities = _dbContext.Users.Where(entity => entity.PIN == userToGet.PIN).ToList();
        return Ok(entities);
    }

    [HttpPost]
    [Route("newUser")]
    public async Task<IActionResult> NewUser([FromBody] User newUser)
    {
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        var entities = _dbContext.Users.Where(entity => entity.Id == 1).ToList();
        return Ok(entities);
    }
}
