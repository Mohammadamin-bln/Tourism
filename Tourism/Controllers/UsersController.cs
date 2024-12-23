﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tourism.Application.Dto;
using Tourism.Application.Features.Commands.UserTickets;
using Tourism.Infrastructure.Repositories;
using static Tourism.Core.Enums.Enums;

namespace Tourism.Controllers
{
    [Route("")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private string GetTicketStatusName(TicketStatus status)
        {
            return status switch
            {
                TicketStatus.WaitingForResponse => "Waiting for Response",
                TicketStatus.Responded => "Responded",
                TicketStatus.Closed => "Closed",
                _ => "Unknown Status"
            };
        }

        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        //interface config
        public UsersController(IUserService userService, IMapper mapper, IMediator mediator)
        {
            _userService = userService;
            _mapper = mapper;
            _mediator = mediator;
        }

        // Register

        [Authorize]
        [HttpPut("Edit/profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto updateProfileDto)
        {
            var username = User.Identity.Name;
            var success = await _userService.UpdateProfileAsync(username, updateProfileDto);
            if (!success)
                return BadRequest("could not update profile. please check your data");
            return Ok("Profile updated successfully");
        }
        [Authorize]
        [HttpPost("Article/new")]
        public async Task<IActionResult> SubmitArticle([FromForm] ArticleDto articleDto)
        {
            var username = User.Identity.Name;

            var result = await _userService.SubmitArticleAsync(username, articleDto);

            if (!result)
                return BadRequest("Could not submit article. Please try again.");

            return Ok("Article submitted successfully");

        }
        [HttpGet("GetCities")]
        public IActionResult GetCities()
        {
            var cities = Enum.GetValues(typeof(Cities))
                             .Cast<Cities>()
                             .Select(city => new { Id = (int)city, Name = city.ToString() })
                             .ToList();

            return Ok(cities);
        }
        [HttpGet("GetTopics")]
        public IActionResult GetTopics()
        {
            var topics = Enum.GetValues(typeof(ArticleTopic))
                             .Cast<ArticleTopic>()
                             .Select(topic => new { Id = (int)topic, Name = topic.ToString() })
                             .ToList();

            return Ok(topics);
        }

        [Authorize]
        [HttpPost("Submit/NewTicket")]
        public async Task<IActionResult> SubmitTicket([FromForm] TicketDto ticket)
        {
            var username = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(username))
                return Unauthorized("User is not authenticated.");


            var command =  new TicketSubmitCommand(username, ticket);
            var ticketId=  await _mediator.Send(command);

            if (ticketId == null)
                return BadRequest("could not submit ");
            return Ok(new { TicketId = ticketId });


        }
        [Authorize]
        [HttpGet("MyTickets")]
        public async Task<IActionResult> GetUserTickets()
        {
            var username = User.Identity.Name;

            if (string.IsNullOrEmpty(username))
                return Unauthorized("User is not authenticated.");

            var tickets = await _userService.GetUserTicketsAsync(username);

            if (tickets == null || !tickets.Any())
                return NotFound("No tickets found for the user.");

            // Add a human-readable status to each ticket before returning it
            foreach (var ticket in tickets)
            {
                // Map the numeric status to a user-friendly status
                ticket.StatusName = GetTicketStatusName(ticket.Status);
            }

            return Ok(tickets);
        }

        [Authorize]
        [HttpPut("UpdateTicket/{ticketId}")]
        public async Task<IActionResult> UpdateTicket(int ticketId, [FromBody] TicketUpdateDto ticketUpdateDto)
        {
            var success = await _userService.TicketUpdateAsync(ticketId, ticketUpdateDto);

            if (!success)
                return BadRequest("Ticket could not be updated or does not exist.");

            return Ok("Ticket updated successfully.");
        }

        [Authorize]
        [HttpGet("Articles/SortBy")]
        public async Task<IActionResult> GetSortedArticles([FromQuery] int? cityId, [FromQuery] int? topicId)
        {
            var articles = await _userService.GetSortedArticlesAsync(cityId, topicId);

            if (articles == null || !articles.Any())
                return NotFound("No articles found for the given criteria.");

            return Ok(articles);
        }





    }
}
