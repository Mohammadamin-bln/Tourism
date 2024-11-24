using System.ComponentModel.DataAnnotations;

namespace Tourism.Application.Dto
{
    public class TicketUpdateDto
    {
        [Required]
        public string Description { get; set; }
    }
}
