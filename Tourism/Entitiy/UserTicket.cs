using static Tourism.Enums.Enums;

namespace Tourism.Entitiy
{
    public class UserTicket
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
     
        public int UserId { get; set; } 
        public User User { get; set; }

        public string AdminResponse { get; set; }
    }
}
