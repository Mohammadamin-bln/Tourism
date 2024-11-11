using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tourism.Entitiy.Dto;
using Tourism.Services;

namespace Tourism.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserServices _userService;
        private readonly IMapper _mapper;

        public UsersController(UserServices userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var success = await _userService.RegisterAsync(registerDto);

            if (!success)
                return BadRequest("User already exists");

            return Ok("Registration successful");
        }

        // Login chek
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var success = await _userService.LoginAsync(loginDto);

            if (!success)
                return Unauthorized("Invalid credentials");

            return Ok("Login successful");
        }

        // get user by name
        [HttpGet("user/{username}")]
        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var userDto = await _userService.GetUserByUsernameAsync(username);

            if (userDto == null)
                return NotFound("User not found");

            return Ok(userDto);
        }
    }
}
