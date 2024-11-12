using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tourism.Dto;
using Tourism.Services;

namespace Tourism.Controllers
{
    [Route("")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        // Constructor now expects IUserService
        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        // Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var success = await _userService.RegisterAsync(registerDto);

            if (!success)
                return BadRequest("User already exists");

            return Ok("Registration successful");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _userService.LoginAsync(loginDto);

            if (token == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { Token = token });
        }
        [Authorize]
        [HttpPut("Edit")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var username = User.Identity.Name;
            var success= await _userService.UpdateProfileAsync(username, updateProfileDto);
            if (!success)
                return BadRequest("could not update profile. please check your data");
            return Ok("Profile updated successfully");
        }

    }
}
