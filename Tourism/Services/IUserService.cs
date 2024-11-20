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
        Task<bool> DeleteArticleAsync(int articleId);
        Task<int?> TicketSubmitAsync(string username, TicketDto ticketDto);
        Task<bool> RespondToTicketAsync(int ticketId, string adminResponse);

        Task<List<TicketDetailDto>> GetUserTicketsAsync(string username);

        Task<bool> TicketUpdateAsync(int ticketId, TicketUpdateDto ticketUpdateDto);
        Task<bool> CloseTicketAsync(int ticketId);


    }
}
