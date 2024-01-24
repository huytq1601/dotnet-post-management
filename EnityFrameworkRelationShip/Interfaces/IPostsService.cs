using EnityFrameworkRelationShip.Dtos.Post;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface IPostsService
    {
        Task<IEnumerable<PostWithTagsDto>> GetAllPostsAsync();
        Task<IEnumerable<PostWithTagsDto>> GetAllPostsAsync(string tag);
        Task<IEnumerable<PostWithTagsDto>> GetPostsOfOtherAsync(string userId);
        Task<PostWithTagsDto?> GetPostByIdAsync(Guid id);
        Task<PostWithTagsDto> CreatePostAsync(PostDto postDto, string userId);
        Task<bool> UpdatePostAsync(UpdatePostDto updatePostDto);
        Task<bool> DeletePostAsync(Guid postId);
    }
}
