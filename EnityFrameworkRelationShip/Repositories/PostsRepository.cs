using EnityFrameworkRelationShip.Data;
using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Interfaces.Repository;
using EnityFrameworkRelationShip.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
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
            // Fetch posts with eager loading
            var posts = await _context.Posts
                .Where(p => !p.IsDeleted)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .ToListAsync();

            // Filter deleted tags within each post's PostTags collection
            foreach (var post in posts)
            {
                post.PostTags = post.PostTags.Where(pt => !pt.Tag.IsDeleted).ToList();
            }

            return posts;
        }

        public async Task<IEnumerable<Post>> GetPostsByTagAsync(string tag)
        {
            var posts = await _context.Posts
                .Where(p => !p.IsDeleted)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .Where(p => p.PostTags.Any(pt => !pt.Tag.IsDeleted && pt.Tag.Name.Contains(tag)))
                .ToListAsync();
            return posts;
        }

        public async Task<Post?> GetPostByIdAsync(Guid id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
                return null;

            post.PostTags = await _context.Entry(post)
                .Collection(p => p.PostTags)
                .Query()
                .Where(pt => !pt.Tag.IsDeleted)
                .Include(pt => pt.Tag)
                .ToListAsync();

            return post;
        }

        public async Task<Post> CreatePostAsync(PostDto postDto, string userId)
        {
            if(postDto == null)
            {
                throw new ArgumentNullException(nameof(postDto));
            }

            var post = new Post
            {
                Title = postDto.Title,
                Content = postDto.Content,
                DatePublished = DateTime.Now,
                UserId = userId
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
                            .Where(p => !p.IsDeleted)
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
            var post = await _context.Posts.Where(p => p.Id == postId && !p.IsDeleted).FirstOrDefaultAsync();
            if (post == null )
            {
                // If the post does not exist, we return false indiciating we didn't delete anything.
                return false;
            }

            post.IsDeleted = true;
            _context.Posts.Update(post);
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

            var existingPostTags = post.PostTags.ToList();

            foreach (var postTag in existingPostTags)
            {
                if (!tagNames.Contains(postTag.Tag.Name))
                {
                    post.PostTags.Remove(postTag);
                }
                else
                {
                    if(postTag.Tag.IsDeleted)
                    {
                        postTag.Tag.IsDeleted = false;
                        _context.Tags.Update(postTag.Tag);
                    }
                    tagNames.Remove(postTag.Tag.Name); // Remove so we don't attempt to add it again
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

                if(tag.IsDeleted)
                {
                    tag.IsDeleted = false;
                    _context.Tags.Update(tag);
                }

                post.PostTags.Add(new PostTag { Tag = tag });
            }
        }
    }
}
