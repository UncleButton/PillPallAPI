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
        var entities = new User(){Name = "not set"};

        if(userToGet.Id!=0)
            entities = _dbContext.Users.Where(entity => entity.Id == userToGet.Id).ToList().First();
        if(!string.IsNullOrEmpty(userToGet.Name))
            entities = _dbContext.Users.Where(entity => entity.Name == userToGet.Name).ToList().First();

        return Ok(entities);
    }

    [HttpPost]
    [Route("newUser")]
    public async Task<IActionResult> NewUser([FromBody] User newUser)
    {
        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync();

        return Ok();
    }
}
