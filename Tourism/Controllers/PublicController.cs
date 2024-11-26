using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tourism.Application.Dto;
using Tourism.Application.Features.Commands.Login;
using Tourism.Application.Features.Commands.Register;
using Tourism.Infrastructure.Repositories;

namespace Tourism.Controllers
{
    [Route("")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        //interface config
        public PublicController(IUserService userService, IMapper mapper,IMediator mediator)
        {
            _userService = userService;
            _mapper = mapper;
            _mediator = mediator;
        }
        [HttpPost("SingUp")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            var command= new RegisterCommand(registerDto);
            var result = await _mediator.Send(command);

            if (!result)
                return BadRequest("User allready exists");
            return Ok("Register successfully");
        }
        [HttpPost("SingIn")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var command= new LoginCommand(loginDto);
            var token= await _mediator.Send(command);

            if (token == null)
                return Unauthorized("Invalid credentials");

            return Ok(new { Token = token });
        }

    }
}
