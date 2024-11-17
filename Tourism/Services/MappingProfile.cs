using AutoMapper;
using Tourism.Dto;
using Tourism.Entitiy;
using static Tourism.Enums.Enums;

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
            CreateMap<ArticleDto, UserArticle>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))  
                .ForMember(dest => dest.UserId, opt => opt.Ignore());

            CreateMap<ArticleDto, UserArticle>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => (Cities)src.CityId))
            .ForMember(dest => dest.Topic, opt => opt.MapFrom(src => Enum.GetName(typeof(ArticleTopic), src.TopicId)));
        }
    }
}

