using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Tourism.Application.Dto;

namespace Tourism.Application.Features.Commands.Login
{
    public class LoginCommand : IRequest<string>
    {

        public LoginDto LoginDto { get; set; }

        public LoginCommand(LoginDto loginDto)
        {
            LoginDto = loginDto;
        }

    }
}
