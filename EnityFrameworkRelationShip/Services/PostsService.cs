﻿using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Models;
using Microsoft.Extensions.Caching.Memory;

namespace EnityFrameworkRelationShip.Services
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

        public async Task<IEnumerable<PostWithTagsDto>> GetAllPostsAsync()
        {
            var cacheKey = GetCacheKey();
            var posts = await GetOrSetCacheAsync(cacheKey, () => _postsRepository.GetAllAsync());

            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<IEnumerable<PostWithTagsDto>> GetAllPostsAsync(string tagName)
        {
            var cacheKey = GetCacheKey(tagName);
            var posts = await GetOrSetCacheAsync(cacheKey, () => _postsRepository.SearchAsync(p => p.Tags.Any(tag => tag.Name.Contains(tagName))));

            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<IEnumerable<PostWithTagsDto>> GetPostsOfOtherAsync(string userId)
        {
            var posts = await _postsRepository.SearchAsync(p => p.UserId != userId);
            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<IEnumerable<PostWithTagsDto>> GetPostsByUser(string userId)
        {
            var posts = await _postsRepository.SearchAsync(p => p.UserId == userId);
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

            var tagNames = postDto.Tags ?? new List<string>();

            await HandleTagsAsync(post, tagNames);

            await _postsRepository.CreateAsync(post);

            _cacheService.RemoveByPrefix(_cachePrefix);

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
            var updatedTagNames = updatePostDto.Tags ?? new List<string>();

            await HandleTagsAsync(post, updatedTagNames);

            await _postsRepository.UpdateAsync(post);

            _cacheService.RemoveByPrefix(_cachePrefix);

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
                post.Tags.Clear();
                return;
            }

            var tagsToRemove = post.Tags.Where(tag => !tagNames.Contains(tag.Name)).ToList();
            foreach (var tagToRemove in tagsToRemove)
            {
                post.Tags.Remove(tagToRemove);
            }

            var existingTagNames = post.Tags.Select(tag => tag.Name);
            var tagNamesToAdd = tagNames.Except(existingTagNames);

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
