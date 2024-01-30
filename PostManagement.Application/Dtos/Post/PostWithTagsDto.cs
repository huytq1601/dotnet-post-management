namespace PostManagement.Application.Dtos.Post
{
    public class PostWithTagsDto : PostDto
    {
        public Guid Id { get; set; }
        public DateTime DatePublished { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
