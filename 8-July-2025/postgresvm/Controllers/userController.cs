using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostgresVm.contexts;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly VmContext _context;

    public UserController(VmContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
    {
        var user = new User
        {
            name = userDto.Name,
            age = userDto.Age,
            education = userDto.Education
        };
        _context.users.Add(user);
        await _context.SaveChangesAsync();
        return Ok(user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _context.users.ToListAsync();
        return Ok(users);
    }
}