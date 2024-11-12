using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tourism.Services;

namespace Tourism.Controllers
{
    [Route("")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        // Constructor now expects IUserService
        public AdminsController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{username}")]

        public async Task<IActionResult> GetUserByUsername(string username)
        {
            var userDto = await _userService.GetUserByUsernameAsync(username);

            if (userDto == null)
                return NotFound("User not found");

            return Ok(userDto);
        }

    }
}
