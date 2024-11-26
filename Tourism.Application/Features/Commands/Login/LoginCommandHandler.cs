using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Tourism.Infrastructure.Repositories;

namespace Tourism.Application.Features.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand,string>
    {
        private readonly IUserService _userService;

        public LoginCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            
            return await _userService.LoginAsync(request.LoginDto);
        }

    }
}
