using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tourism.Data;
using Tourism.Services;

namespace Tourism.Controllers
{
    [Route("")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly TourismDbContext _context;
        private readonly IMapper _mapper;

        // Constructor now expects IUserService
        public AdminsController(IUserService userService, IMapper mapper, TourismDbContext context)
        {
            _userService = userService;
            _mapper = mapper;
            _context = context;
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


        [Authorize(Roles = "Admin")]
        [HttpGet("Articles")]
        public async Task<IActionResult> GetPendingArticles()
        {
            var pendingArticles= await _context.Articles
                .Where(a=>a.IsApproved==false)
                .ToListAsync();

            if (pendingArticles.Count == 0)
                return NotFound("Not found any articles");
            return Ok(pendingArticles);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("User/Tickets")]
        public async Task<IActionResult> GetPendingTickets()
        {
            var pendingTickets=await _context.Tickets
                .Where(a=>a.IsOpen==true)
                .ToListAsync();
            if (pendingTickets.Count == 0)
                return NotFound("Not found any tickets");
            return Ok(pendingTickets);
        }


        [Authorize(Roles ="Admin")]
        [HttpPost("Confirm/{articleId}")]
        public async Task<IActionResult> ApproveArticle(int articleId, [FromBody] bool isApproved)
        {

                var result = await _userService.ApproveArticleAsync(articleId, isApproved);

                if (!result)
                    return BadRequest("Could not approve the article. Please try again.");

                return Ok("Article approval status updated successfully.");
        }
        [Authorize(Roles ="Admin")]
        [HttpDelete("delete/{articleId}")]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {
            var result=await _userService.DeleteArticleAsync(articleId);
            if (!result)
                return BadRequest("Coult not delete the article");

            return Ok("Aticle deleted successfully");

        }
    }
    
    }
