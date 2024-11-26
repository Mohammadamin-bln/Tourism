using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Swashbuckle.AspNetCore.SwaggerGen;
using Tourism.Application.Dto;

namespace Tourism.Application.Features.Commands.Register
{
    public class RegisterCommand :IRequest<bool>
    {
        public RegisterDto RegisterDto { get; set; }
        public RegisterCommand(RegisterDto registerDto)
        {
            RegisterDto = registerDto;
        }
    }

}
