using System.ComponentModel.DataAnnotations;

namespace PostManagement.Application.Dtos.Post
{
    public class PostDto
    {
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new List<string>();
    }
}
