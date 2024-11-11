using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Tourism.Data;
using Tourism.Entitiy.Dto;

namespace Tourism.Services
{
    public class UserServices
    {
        private readonly TourismDbContext _context;
        private readonly IMapper _mapper;

        public UserServices(TourismDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == registerDto.Username);

            if (existingUser != null)
                return false; 

            // Hash the password before saving
            var passwordHash = HashPassword(registerDto.Password);

            var user = _mapper.Map<User>(registerDto); 
            user.PasswordHash = passwordHash;
            user.UserRole = "User";  // Default role is "User"

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        
        public async Task<bool> LoginAsync(LoginDto loginDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null)
                return false; 

            var hashedPassword = HashPassword(loginDto.Password);

            if (hashedPassword != user.PasswordHash)
                return false; 

            return true; 
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

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}

