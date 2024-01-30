using PostManagement.Application.Dtos.Post;
using PostManagement.Core.Common;

namespace PostManagement.Application.Interfaces
{
    public interface IPostsService
    {
        Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetAllPostsAsync(PostFilter filter);
        Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetPostsOfOtherAsync(string userId, PostFilter filter);
        Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetPostsByUserAsync(string userId, PostFilter filter);
        Task<Response<PostWithTagsDto>> GetPostByIdAsync(Guid id);
        Task<PostWithTagsDto> CreatePostAsync(PostDto postDto, string userId);
        Task UpdatePostAsync(UpdatePostDto updatePostDto);
        Task DeletePostAsync(Guid postId);
    }
}
