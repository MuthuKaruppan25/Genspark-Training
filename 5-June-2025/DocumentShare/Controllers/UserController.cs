using DocumentShare.Interfaces;
using DocumentShare.Models;
using Microsoft.AspNetCore.Mvc;
using SecondWebApi.Misc;

namespace DocumentShare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;


        public UserController(IUserService userService)
        {
            _userService = userService;

        }

        [HttpPost("register")]
        [ServiceFilter(typeof(CustomExceptionFilter))]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userDto)
        {


            var newUser = await _userService.RegisterUser(userDto);
            return Ok(new
            {
                message = "User registered successfully",
                username = newUser.Username,
                role = newUser.Role
            });


        }
    }
}
