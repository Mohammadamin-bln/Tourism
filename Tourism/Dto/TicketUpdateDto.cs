using System.ComponentModel.DataAnnotations;

namespace Tourism.Dto
{
    public class TicketUpdateDto
    {
        [Required]
        public string Description { get; set; }
    }
}
