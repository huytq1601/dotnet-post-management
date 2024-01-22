using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EnityFrameworkRelationShip.Services
{
    public class PostsService: IPostsService
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

        public async Task<IEnumerable<PostWithTagsDto>> GetAllPostsAsync()
        {
            var cacheKey = GetCacheKey();
            var posts = await GetOrSetCacheAsync(cacheKey, () => _postsRepository.GetAllAsync());
            //var posts = await _postsRepository.GetAllAsync();

            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<IEnumerable<PostWithTagsDto>> GetAllPostsAsync(string tag)
        {
            var cacheKey = GetCacheKey(tag);
            var posts = await GetOrSetCacheAsync(cacheKey, () => _postsRepository.SearchAsync(p => p.PostTags.Any(pt => pt.Tag.Name.Contains(tag))));
            //var posts = await _postsRepository.SearchAsync(p => p.PostTags.Any(pt => pt.Tag.Name.Contains(tag)));

            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<PostWithTagsDto?> GetPostByIdAsync(Guid id)
        {
            var post = await _postsRepository.GetByIdAsync(id);

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

            await _postsRepository.CreateAsync(post);

            return _mapper.Map<PostWithTagsDto>(post);
        }

        public async Task<bool> UpdatePostAsync(UpdatePostDto updatePostDto)
        {
            if (updatePostDto == null)
            {
                throw new ArgumentNullException(nameof(updatePostDto));
            }   

            var post = await _postsRepository.GetByIdAsync(updatePostDto.Id);

            if (post == null)
            {
                // The post does not exist in the database
                return false;
            }

            post.Title = updatePostDto.Title;
            post.Content = updatePostDto.Content;
            var updatedTagNames = updatePostDto.TagNames ?? new List<string>();

            await HandleTagsAsync(post, updatedTagNames);

            await _postsRepository.UpdateAsync(post);

            return true;
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            var post = await _postsRepository.GetByIdAsync(id);
            if (post == null)
            {
                // If the post does not exist, we return false indiciating we didn't delete anything.
                return false;
            }

            post.IsDeleted = true;
            await _postsRepository.UpdateAsync(post);

            _cacheService.RemoveByPrefix(_cachePrefix);

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
                        await _tagsRepository.UpdateAsync(postTag.Tag);
                    }
                    tagNames.Remove(postTag.Tag.Name);
                }
            }

            // For each remaining tag name, add it if it doesn't exist already
            foreach (var tagName in tagNames)
            {
                var tag = await _tagsRepository.FindOneAsync(t => t.Name == tagName)
                         ?? new Tag { Name = tagName };

                // Add to context if it's a new tag
                if (tag.Id == Guid.Empty)
                {
                    await _tagsRepository.CreateAsync(tag);
                }

                if (tag.IsDeleted)
                {
                    tag.IsDeleted = false;
                    await _tagsRepository.UpdateAsync(tag);
                }

                post.PostTags.Add(new PostTag { Tag = tag });
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
