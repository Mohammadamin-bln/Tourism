using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Tourism.Data;
using Tourism.Dto;
using Tourism.Entitiy;
using BCrypt.Net;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using Microsoft.VisualBasic;
using static Tourism.Enums.Enums;

namespace Tourism.Services
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

            var photoUrl = await SavePhotoAsync(articleDto.Photo);

            var article = _mapper.Map<UserArticle>(articleDto);

            article.UserId = user.Id;
            article.PhotoUrl = photoUrl;

            // Save the article to the database
            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();

            return true;
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

        public async Task<bool> ApproveArticleAsync(int articleId,bool isApproved)
        {
            var article= await _context.Articles
                .FirstOrDefaultAsync(a=> a.Id==articleId);
            if (article==null) 
                return false;
            article.IsApproved = isApproved;

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
        private async Task<string> SavePhotoAsync(IFormFile photo)
        {
            // Check if the photo exists
            if (photo == null || photo.Length == 0)
                return null;

            // Ensure the upload directory exists
            var directoryPath = Path.Combine("uploads", "photos");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Create a unique file name and save the photo
            var filePath = Path.Combine(directoryPath, Guid.NewGuid().ToString() + Path.GetExtension(photo.FileName));

            // Save the photo
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return filePath;
        }
    }
}

