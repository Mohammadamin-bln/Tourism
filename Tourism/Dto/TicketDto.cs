namespace Tourism.Dto
{
    public class TicketDto
    {
        public string Title { get; set; }

        public string Description { get; set; }
        public string Status { get; set; }

        public IFormFile Photo{ get; set; }

        public string FilePath{ get; set; }
    }
}
