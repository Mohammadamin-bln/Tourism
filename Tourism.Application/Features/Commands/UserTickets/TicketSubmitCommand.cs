using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tourism.Application.Dto;

namespace Tourism.Application.Features.Commands.UserTickets
{
    public class TicketSubmitCommand : IRequest<int?>  
    {
        public string Username { get; set; }
        public TicketDto TicketDto { get; set; }

        public TicketSubmitCommand(string username, TicketDto ticketDto)
        {
            Username = username;
            TicketDto = ticketDto;
        }
    }
}
