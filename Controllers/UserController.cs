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
        var user = new User(){Id = -1, Name = "not set", PIN = -1};
        Console.WriteLine(userToGet.Name);
        if(userToGet.Id != null)
            user = _dbContext.Users.Where(entity => entity.Id == userToGet.Id).ToList().First();
        if(!string.IsNullOrEmpty(userToGet.Name))
            user = _dbContext.Users.Where(entity => entity.Name == userToGet.Name).ToList().First();

        return Ok(user);
    }

    [HttpPost]
    [Route("getUsers")]
    public IActionResult GetUsers()
    {
        var users = _dbContext.Users;

        return Ok(users);
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
