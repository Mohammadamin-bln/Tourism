using AutoMapper;
using Tourism.Dto;
using Tourism.Entitiy;

namespace Tourism.Services
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            // Map User entity to UsersDto
            CreateMap<User, UsersDto>();

            // Map RegisterDto to User for registration
            CreateMap<RegisterDto, User>();


            // Map LoginDto to User for login 
            CreateMap<LoginDto, User>();
        }
    }
}

