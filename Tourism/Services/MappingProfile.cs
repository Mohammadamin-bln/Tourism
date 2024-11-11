using AutoMapper;
using Tourism.Data;
using Tourism.Entitiy.Dto;

namespace Tourism.Services
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            // Map User entity to UsersDto
            CreateMap<User, UsersDto>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            // Map RegisterDto to User for registration
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            // Map LoginDto to User for login 
            CreateMap<LoginDto, User>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username));
        }
    }
}

