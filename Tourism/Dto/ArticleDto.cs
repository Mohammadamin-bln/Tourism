namespace Tourism.Dto
{
    public class ArticleDto
    {
        public string Description { get; set; }

        public IFormFile Photo { get; set; }

        public int CityId { get; set; }

        public int TopicId { get; set; }
        


    }
}
