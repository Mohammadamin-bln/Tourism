using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Tourism.Infrastructure.Repositories;

namespace Tourism.Application.Features.Commands.UserTickets
{
    public class SubmitTicketCommandHandler : IRequestHandler<TicketSubmitCommand, int?>
    {
        private readonly IUserService _userService;

        public SubmitTicketCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<int?> Handle(TicketSubmitCommand request, CancellationToken cancellationToken)
        {
            // Call the existing method in IUserService to submit the ticket
            return await _userService.TicketSubmitAsync(request.Username, request.TicketDto);
        }
    }

}
