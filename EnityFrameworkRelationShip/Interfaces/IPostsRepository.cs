using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Interfaces
{
    public interface IPostsRepository
    {
        Task<IEnumerable<Post>> GetAllPostsAsync();
        Task<IEnumerable<Post>> GetPostsByTagAsync(string tag);
        Task<Post?> GetPostByIdAsync(Guid id);
        Task<Post> CreatePostAsync(PostDto post);
        Task<bool> UpdatePostAsync(UpdatePostDto updatePostDto);
        Task<bool> DeletePostAsync(Guid id);
    }
}
