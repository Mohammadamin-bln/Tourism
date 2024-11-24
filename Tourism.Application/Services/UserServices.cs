using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using Microsoft.VisualBasic;
using static Tourism.Core.Enums.Enums;
using Tourism.Core.Entitiy;
using Tourism.Application.Dto;
using Tourism.Infrastructure.Repositories;
using Tourism.Infrastructure.Data;
using Microsoft.AspNetCore.Http;


namespace Tourism.Application.Services
{
    public class UserServices : IUserService
    {
        private readonly TourismDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;



        public UserServices(TourismDbContext context, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;

        }
        // ARTICLES
        public async Task<bool> SubmitArticleAsync(string username, ArticleDto articleDto)
        {

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return false;


            var cityName = Enum.GetName(typeof(Cities), articleDto.CityId);

            if (cityName == null)
                return false;


            var photoUrls = await SavePhotosAsync(articleDto.Photos);


            var article = _mapper.Map<UserArticle>(articleDto);

            article.UserId = user.Id;
            article.PhotoUrl = string.Join(";", photoUrls);


            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CloseTicketAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .SingleOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                return false;


            if (ticket.Status == TicketStatus.Closed)
                return false;


            ticket.Status = TicketStatus.Closed;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TicketUpdateAsync(int ticketId, TicketUpdateDto ticketUpdateDto)
        {
            var ticket = await _context.Tickets
                .SingleOrDefaultAsync(a => a.Id == ticketId);

            if (ticket == null)
                return false;

            if (!string.IsNullOrEmpty(ticketUpdateDto.Description))
            {
                ticket.Description = ticketUpdateDto.Description;
            }

            ticket.UpdatedAt = DateTime.UtcNow;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int?> TicketSubmitAsync(string username, TicketDto ticketDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            var userTicket = _mapper.Map<UserTicket>(ticketDto);
            userTicket.UserId = user.Id;
            userTicket.CreatedAt = DateTime.UtcNow;
            userTicket.Status = TicketStatus.WaitingForResponse; // Or the default starting status
            userTicket.AdminResponse = "null";

            if (ticketDto != null)
            {
                var photoUrl = await SaveTicketPhotoAsync(ticketDto.Photo);
                if (photoUrl != null)
                {
                    userTicket.FilePath = photoUrl;
                }
            }

            await _context.Tickets.AddAsync(userTicket);
            await _context.SaveChangesAsync();

            // Return the ID of the created ticket
            return userTicket.Id;
        }

        public async Task<bool> ResponedToTicketAsync(int ticketId, string adminRespond)
        {
            var ticket = await _context.Tickets
                .SingleOrDefaultAsync(a => a.Id == ticketId);

            if (ticket == null)
                return false;
            if (ticket.Status != TicketStatus.WaitingForResponse)
                return false;

            ticket.AdminResponse = adminRespond;
            ticket.Status = TicketStatus.Responded;

            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<TicketDetailDto>> GetUserTicketsAsync(string username)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            var tickets = await _context.Tickets
                .Where(t => t.UserId == user.Id)
                .ToListAsync();


            var ticketDtos = _mapper.Map<List<TicketDetailDto>>(tickets);

            return ticketDtos;
        }


        //register service
        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username);

            if (existingUser != null)
                return false;  // User already exists

            // Hash the password before saving
            var passwordHash = HashPassword(registerDto.Password);

            var user = _mapper.Map<User>(registerDto);
            user.PasswordHash = passwordHash;


            if (registerDto.Username == "Admin")
            {
                user.UserRole = "Admin";
            }
            else
            {
                user.UserRole = "User";
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UserArticle>> GetSortedArticlesAsync(int? cityId, int? topicId)
        {
            var query = _context.Articles.AsQueryable();

            if (cityId != null)
            {
                var cityName = Enum.GetName(typeof(Cities), cityId.Value);

                if (cityName == null)
                    throw new ArgumentException("invalid City ID");

                query = query.Where(ua => ua.City == cityName);
            }

            if (topicId != null)
            {
                var topicName = Enum.GetName(typeof(ArticleTopic), topicId.Value);
                if (topicName == null)
                    throw new ArgumentException("Invalid topic ID");

                query = query.Where(ua => ua.Topic == topicName);
            }

            return await query.ToListAsync();

        }

        public async Task<bool> ApproveArticleAsync(int articleId, bool isApproved)
        {
            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.Id == articleId);
            if (article == null)
                return false;
            article.IsApproved = isApproved;

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteArticleAsync(int articleId)
        {
            var article = await _context.Articles
                .FirstOrDefaultAsync(a => a.Id == articleId);
            if (article == null)
                return false;
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return true;

        }

        //login service
        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            // cheking database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            //if user not exists return false
            if (user == null || !VerifyPassword(loginDto.Password, user.PasswordHash))
                return null;
            // generate the user token
            GenerateJwtToken(user);

            return GenerateJwtToken(user);
        }
        //editing profile 
        public async Task<bool> UpdateProfileAsync(string username, UpdateProfileDto updateProfileDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
            //if username  not exists
            if (user == null)
                return false;
            // if password was not valid
            if (!string.IsNullOrEmpty(updateProfileDto.CurrentPassword))
            {
                if (!VerifyPassword(updateProfileDto.CurrentPassword, user.PasswordHash)) return false;

            }
            //updating username 
            if (!string.IsNullOrEmpty(updateProfileDto.Username) && updateProfileDto.Username != username)
            {
                user.Username = updateProfileDto.Username;
            }
            //updating phone number
            if (!string.IsNullOrEmpty(updateProfileDto.NewPhoneNumber))
            {
                user.PhoneNumber = updateProfileDto.NewPhoneNumber;
            }
            //updating password
            if (!string.IsNullOrEmpty(updateProfileDto.NewPassword))
            {

                user.PasswordHash = HashPassword(updateProfileDto.NewPassword);
            }
            //saving changes to database
            await _context.SaveChangesAsync();
            return true;
        }


        private bool VerifyPassword(string password, string storedHash)
        {

            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
        // generating JwtToken
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        public async Task<UsersDto> GetUserByUsernameAsync(string username)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return null;

            return _mapper.Map<UsersDto>(user);
        }

        // Helper function to hash passwords
        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hash = BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hash;
        }
        private async Task<List<string>> SavePhotosAsync(List<IFormFile> photos)
        {

            var photoUrls = new List<string>();

            var directorypath = Path.Combine("uploads", "photos");
            if (!Directory.Exists(directorypath))
            {
                Directory.CreateDirectory(directorypath);
            }
            foreach (var photo in photos)
            {
                if (photo != null && photo.Length > 0)
                {
                    var filePath = Path.Combine(directorypath, Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName));
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await photo.CopyToAsync(stream);
                    }
                    photoUrls.Add(filePath);
                }
            }
            return photoUrls;

        }
        private async Task<string> SaveTicketPhotoAsync(IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return null;
            var directoryPath = Path.Combine("uploads", "Ticket_photos");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var filepath = Path.Combine(directoryPath, Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName));

            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }
            return filepath;
        }
    }
}

