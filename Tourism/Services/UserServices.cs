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
            var user=await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
            //if username  not exists
            if (user==null)
                return false;
            // if password was not valid
            if (!string.IsNullOrEmpty(updateProfileDto.CurrentPassword))
            {
                if(!VerifyPassword(updateProfileDto.CurrentPassword,user.PasswordHash)) return false;

            }
            //updating username 
            if (!string.IsNullOrEmpty(updateProfileDto.Username) &&updateProfileDto.Username != username)
            {
                user.Username=updateProfileDto.Username;
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
            var claims= new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserRole)
            };
            var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
);

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
            var hash=BCrypt.Net.BCrypt.HashPassword(password, salt);
            return hash;
        }
    }
}

