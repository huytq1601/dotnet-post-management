using PostManagement.Application.Dtos.User;

namespace PostManagement.Application.Dtos.Post
{
    public class PostWithTagsDto : PostDto
    {
        public Guid Id { get; set; }
        public DateTime DatePublished { get; set; }
        public SimpleUserDto Author { get; set; } = null!;
    }
}
