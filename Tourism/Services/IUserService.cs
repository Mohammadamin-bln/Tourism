using Tourism.Dto;

namespace Tourism.Services
{
    public interface IUserService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<string> LoginAsync(LoginDto loginDto);
        Task<UsersDto> GetUserByUsernameAsync(string username);

        Task<bool> UpdateProfileAsync(string username, UpdateProfileDto updateProfileDto);

        Task<bool> SubmitArticleAsync(string username, ArticleDto articleDto);
        Task<bool> ApproveArticleAsync(int articleId, bool isApproved);
    }
}
