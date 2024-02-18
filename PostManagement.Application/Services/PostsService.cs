using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PostManagement.Application.Dtos.Post;
using PostManagement.Application.Interfaces;
using PostManagement.Core.Common;
using PostManagement.Core.Entities;
using PostManagement.Core.Exceptions;
using PostManagement.Core.Interfaces;
using System.Linq.Expressions;

namespace PostManagement.Application.Services
{
    public class PostsService : IPostsService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Post> _postsRepository;
        private readonly IRepository<Tag> _tagsRepository;
        private readonly IMemoryCache _cache;
        private readonly ICacheService _cacheService;
        private readonly string _cachePrefix = "postsCacheKey";

        public PostsService(IMapper mapper, IRepository<Post> postsRepository, IRepository<Tag> tagsRepository, IMemoryCache cache, ICacheService cacheService)
        {
            _mapper = mapper;
            _cache = cache;
            _cacheService = cacheService;
            _postsRepository = postsRepository;
            _tagsRepository = tagsRepository;
        }

        public async Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetAllPostsAsync(PostFilter filter)
        {
            Expression<Func<Post, bool>> filterExpression;

            if (filter.Tag != null)
            {
                filterExpression = p => p.Tags.Any(t => t.Name.Contains(filter.Tag));
            }
            else
            {
                filterExpression = p => p.IsDeleted == false;
            }

            var response = await GetPageResponseAsync(filter, filterExpression);

            return response;
        }

        public async Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetPostsOfOtherAsync(string userId, PostFilter filter)
        {
            Expression<Func<Post, bool>> filterExpression;

            if(filter.Tag != null)
            {
                filterExpression = p => p.UserId != userId && p.Tags.Any(t => t.Name.Contains(filter.Tag));
            }
            else
            {
                filterExpression = p => p.UserId != userId;
            }
           
            var response = await GetPageResponseAsync(filter, filterExpression);

            return response;
        }

        public async Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetPostsByUserAsync(string userId, PostFilter filter)
        {
            Expression<Func<Post, bool>> filterExpression;

            if (filter.Tag != null)
            {
                filterExpression = p => p.UserId == userId && p.Tags.Any(t => t.Name.Contains(filter.Tag));
            }
            else
            {
                filterExpression = p => p.UserId == userId;
            }

            var response = await GetPageResponseAsync(filter, filterExpression);

            return response;
        }

        public async Task<Response<PostWithTagsDto>> GetPostByIdAsync(Guid id)
        {
            var post = await _postsRepository.GetByIdAsync(id);

            if(post is not null)
            {
                var postWithTags = _mapper.Map<PostWithTagsDto>(post);
                var response = new Response<PostWithTagsDto>(postWithTags);

                return response;
            }
            else
            {
                throw new NotFoundException("Post not found");
            }
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

            var tagNames = postDto.Tags ?? new List<string>();

            var tags = new List<Tag>();

            foreach (var tagName in tagNames)
            {
                var tag = await _tagsRepository.FindOneAsync(t => t.Name == tagName);

                if (tag == null)
                {
                    // If tag doesn't exist, create it and add it to the database
                    var newTag = new Tag { Name = tagName };
                    await _tagsRepository.CreateAsync(newTag);
                    tags.Add(newTag);
                }
                else if (tag.IsDeleted)
                {
                    tag.IsDeleted = false;
                    await _tagsRepository.UpdateAsync(tag);
                }
                else
                {
                    tags.Add(tag);
                }
            }

            post.Tags = tags;

            await _postsRepository.CreateAsync(post);
            await _postsRepository.SaveChangesAsync();
            return _mapper.Map<PostWithTagsDto>(post);
        }

        public async Task UpdatePostAsync(UpdatePostDto updatePostDto)
        {
            if (updatePostDto == null)
            {
                throw new BadRequestException("Invalid Post Data");
            }

            var post = await _postsRepository.GetByIdAsync(updatePostDto.Id) ?? throw new NotFoundException("Post Not Found");

            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;
            var updatedTagNames = updatePostDto.Tags ?? new List<string>();

            await HandleTagsAsync(post, updatedTagNames);

            await _postsRepository.UpdateAsync(post);
        }

        public async Task DeletePostAsync(Guid id)
        {
            var post = await _postsRepository.GetByIdAsync(id) ?? throw new NotFoundException($"Post with Id {id} Not Found");

            post.IsDeleted = true;
            await _postsRepository.UpdateAsync(post);

            _cacheService.RemoveByPrefix(_cachePrefix);
        }

        private async Task<PageResponse<IEnumerable<PostWithTagsDto>>> GetPageResponseAsync(PostFilter filter, Expression<Func<Post, bool>> filterCondition)
        {
            var query = _postsRepository.GetQuery().Where(filterCondition);

            var count = await query.CountAsync();

            var posts = await query
               .Skip((filter.PageNumber - 1) * filter.PageSize)
               .Take(filter.PageSize)
               .Include(p => p.User)
               .ToListAsync();

            var data = _mapper.Map<IEnumerable<PostWithTagsDto>>(posts);

            var totalPages = Convert.ToInt32(Math.Ceiling((decimal)count / filter.PageSize));

            var response = new PageResponse<IEnumerable<PostWithTagsDto>>(data, filter.PageNumber, filter.PageSize)
            {
                TotalItems = count,
                TotalPages = totalPages
            };

            return response;
        }

        private async Task HandleTagsAsync(Post post, List<string> tagNames)
        {
            if (tagNames == null || tagNames.Count == 0)
            {
                post.Tags.Clear();
                return;
            }

            var tagsToRemove = post.Tags.Where(tag => !tagNames.Contains(tag.Name)).ToList();
            foreach (var tagToRemove in tagsToRemove)
            {
                post.Tags.Remove(tagToRemove);
            }

            var existingTagNames = post.Tags.Select(tag => tag.Name);
            var tagNamesToAdd = tagNames.Except(existingTagNames).ToList();

            foreach (var tagName in tagNamesToAdd)
            {
                var tag = await _tagsRepository.FindOneAsync(t => t.Name == tagName);

                if (tag == null)
                {
                    tag = new Tag { Name = tagName };
                    await _tagsRepository.CreateAsync(tag);
                }
                else if (tag.IsDeleted)
                {
                    tag.IsDeleted = false;
                    await _tagsRepository.UpdateAsync(tag);
                }

                post.Tags.Add(tag);
            }
        }

        private string GetCacheKey(string? tag = null)
        {
            return string.IsNullOrEmpty(tag) ? _cachePrefix : $"{_cachePrefix}-{tag}";
        }

        private async Task<IEnumerable<Post>> GetOrSetCacheAsync(string cacheKey, Func<Task<IEnumerable<Post>>> getPosts)
        {
            if (_cache.TryGetValue(cacheKey, out IEnumerable<Post> cachedPosts))
            {
                return cachedPosts ?? new List<Post>();
            }

            var posts = await getPosts();

            _cacheService.SetWithKeyTracking(cacheKey, posts);

            return posts;
        }
    }
}
