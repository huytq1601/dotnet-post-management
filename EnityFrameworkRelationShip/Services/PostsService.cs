using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Post;
using Microsoft.EntityFrameworkCore;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Interfaces.Repository;
using EnityFrameworkRelationShip.Interfaces.Service;
using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Services
{
    public class PostsService: IPostsService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Post> _postsRepository;
        private readonly IRepository<Tag> _tagsRepository;

        public PostsService(IMapper mapper, IRepository<Post> postsRepository, IRepository<Tag> tagsRepository)
        {
            _mapper = mapper;
            _postsRepository = postsRepository;
            _tagsRepository = tagsRepository;
        }

        public async Task<IEnumerable<PostWithTagsDto>> GetAllPostsAsync()
        {
            var posts = await _postsRepository.GetAll()
                .Where(p => !p.IsDeleted)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .ToListAsync();

            foreach (var post in posts)
            {
                post.PostTags = post.PostTags.Where(pt => !pt.Tag.IsDeleted).ToList();
            }

            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<IEnumerable<PostWithTagsDto>> GetAllPostsAsync(string tag)
        {
            var posts = await _postsRepository.GetAll()
                .Where(p => !p.IsDeleted && p.PostTags.Any(pt => !pt.Tag.IsDeleted && pt.Tag.Name.Contains(tag)))
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .ToListAsync(); ;

            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<PostWithTagsDto?> GetPostByIdAsync(Guid id)
        {
            var post = await _postsRepository.GetAll()
                .Where(p => !p.IsDeleted && p.Id == id)
                .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync();

            return _mapper.Map<PostWithTagsDto>(post);
        }

        public async Task<PostWithTagsDto> CreatePostAsync(PostDto postDto, string userId)
        {
            if (postDto == null)
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

            _postsRepository.Create(post);
            _postsRepository.SaveChanges();

            return _mapper.Map<PostWithTagsDto>(post);
        }

        public async Task<bool> UpdatePostAsync(UpdatePostDto updatePostDto)
        {
            if (updatePostDto == null)
                throw new ArgumentNullException(nameof(updatePostDto));

            var post = await _postsRepository.GetAll()
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

            _postsRepository.Update(post);
            _postsRepository.SaveChanges();

            return true;
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var post = _postsRepository.GetById(id);
            if (post == null)
            {
                // If the post does not exist, we return false indiciating we didn't delete anything.
                return false;
            }
            _postsRepository.Delete(post);
            _postsRepository.SaveChanges();

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
                    if (postTag.Tag.IsDeleted)
                    {
                        postTag.Tag.IsDeleted = false;
                        _tagsRepository.Update(postTag.Tag);
                    }
                    tagNames.Remove(postTag.Tag.Name); // Remove so we don't attempt to add it again
                }
            }

            // For each remaining tag name, add it if it doesn't exist already
            foreach (var tagName in tagNames)
            {
                var tag = await _tagsRepository.GetAll().FirstOrDefaultAsync(t => t.Name == tagName)
                         ?? new Tag { Name = tagName };

                // Add to context if it's a new tag
                if (tag.Id == Guid.Empty)
                {
                    _tagsRepository.Create(tag);
                }

                if (tag.IsDeleted)
                {
                    tag.IsDeleted = false;
                    _tagsRepository.Update(tag);
                }

                post.PostTags.Add(new PostTag { Tag = tag });
            }
        }
    }
}
