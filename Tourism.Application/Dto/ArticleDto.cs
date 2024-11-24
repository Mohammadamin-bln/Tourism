using static Tourism.Core.Enums.Enums;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;


namespace Tourism.Application.Dto
{
    public class ArticleDto
    {
        public string Description { get; set; }

        public List<IFormFile> Photos { get; set; }

        [Required]
        public int CityId { get; set; }


        [Required]
        public int TopicId { get; set; }



    }
}
