using EnityFrameworkRelationShip.Data;
using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Linq.Expressions;

namespace EnityFrameworkRelationShip.Repositories
{
    public class PostsRepository: IPostsRepository
    {
        private readonly DataContext _context;

        public PostsRepository(DataContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAllPostsAsync()
        {
            return await _context.Posts
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByTagAsync(string tag)
        {
            var posts = await _context.Posts
                .Where(post => post.PostTags.Any(pt => pt.Tag.Name.Contains(tag)))
                .Include(post => post.PostTags)
                .ThenInclude(postTag => postTag.Tag)
                .ToListAsync();
            return posts;
        }

        public async Task<Post?> GetPostByIdAsync(Guid id)
        {
           return await _context.Posts
                .Where(p => p.Id == id)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync();
        }

        public async Task<Post> CreatePostAsync(PostDto postDto)
        {
            if(postDto == null)
            {
                throw new ArgumentNullException(nameof(postDto));
            }

            var post = new Post
            {
                Title = postDto.Title,
                Content = postDto.Content,
                DatePublished = DateTime.Now
            };

            var tagNames = postDto.TagNames ?? new List<string>();

            await HandleTagsAsync(post, tagNames);

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<bool> UpdatePostAsync(UpdatePostDto updatePostDto)
        {
            if (updatePostDto == null)
                throw new ArgumentNullException(nameof(updatePostDto));

            var post = await _context.Posts
                            .Include(p => p.PostTags)
                            .ThenInclude(pt => pt.Tag)
                            .FirstOrDefaultAsync(p => p.Id == updatePostDto.Id);
            if (post == null)
            {
                // The post does not exist in the database
                return false;
            }

            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;
            var updatedTagNames = updatePostDto.TagNames ?? new List<string>();

            await HandleTagsAsync(post, updatedTagNames);


            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                // If the post does not exist, we return false indiciating we didn't delete anything.
                return false;
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task HandleTagsAsync(Post post, List<string> tagNames)
        {
            if (tagNames == null || tagNames.Count == 0)
            {
                post.PostTags.Clear();
                return;
            }

            var existingTags = post.PostTags.ToList();

            foreach (var tag in existingTags)
            {
                if (!tagNames.Contains(tag.Tag.Name))
                {
                    post.PostTags.Remove(tag);
                }
                else
                {
                    tagNames.Remove(tag.Tag.Name); // Remove so we don't attempt to add it again
                }
            }

            // For each remaining tag name, add it if it doesn't exist already
            foreach (var tagName in tagNames)
            {
                var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == tagName)
                         ?? new Tag { Name = tagName };

                // Add to context if it's a new tag
                if (tag.Id == Guid.Empty)
                {
                    _context.Tags.Add(tag);
                }

                post.PostTags.Add(new PostTag { Tag = tag });
            }
        }
    }
}
