using static Tourism.Core.Enums.Enums;

namespace Tourism.Application.Dto
{
    public class TicketDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string AdminResponse { get; set; }
        public string StatusName { get; set; }

    }
}
