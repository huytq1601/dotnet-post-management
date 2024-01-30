using EnityFrameworkRelationShip.Common;
using EnityFrameworkRelationShip.Dtos.Post;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface IPostsService
    {
        Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetAllPostsAsync(PostFilter filter);
        Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetPostsOfOtherAsync(string userId, PostFilter filter);
        Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetPostsByUserAsync(string userId, PostFilter filter);
        Task<PostWithTagsDto?> GetPostByIdAsync(Guid id);
        Task<PostWithTagsDto> CreatePostAsync(PostDto postDto, string userId);
        Task<bool> UpdatePostAsync(UpdatePostDto updatePostDto);
        Task<bool> DeletePostAsync(Guid postId);
    }
}
