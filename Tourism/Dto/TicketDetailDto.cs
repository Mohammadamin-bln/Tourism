namespace Tourism.Dto
{
    public class TicketDetailDto
    {
        public int Id { get; set; }               
        public string Title { get; set; }         
        public string Description { get; set; }   
        public bool IsOpen { get; set; }              
        public DateTime CreatedAt { get; set; }   
        public string AdminResponse { get; set; } 

    }
}
