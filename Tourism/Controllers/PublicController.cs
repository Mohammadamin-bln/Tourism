using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tourism.Dto;
using Tourism.Services;

namespace Tourism.Controllers
{
    [Route("")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        //interface config
        public PublicController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [HttpPost("SingUp")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var success = await _userService.RegisterAsync(registerDto);

            if (!success)
                return BadRequest("User already exists");

            return Ok("Registration successful");
        }
        [HttpPost("SingIn")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var token = await _userService.LoginAsync(loginDto);

            if (token == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { Token = token });
        }

    }
}
