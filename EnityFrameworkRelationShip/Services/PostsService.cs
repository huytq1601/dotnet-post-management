using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Services
{
    public class PostsService: IPostsService
    {
        private readonly IMapper _mapper;
        private readonly IPostsRepository _postsRepository;

        public PostsService(IMapper mapper, IPostsRepository postsRepository)
        {
            this._mapper = mapper;
            this._postsRepository = postsRepository;
        }

        public async Task<IEnumerable<PostWithTagsDto>> GetAllPostsAsync()
        {
            var posts = await _postsRepository.GetAllPostsAsync();
            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<IEnumerable<PostWithTagsDto>> GetPostsByTagAsync(string tag)
        {
            var posts = await _postsRepository.GetPostsByTagAsync(tag);
            return _mapper.Map<List<PostWithTagsDto>>(posts);
        }

        public async Task<PostWithTagsDto?> GetPostByIdAsync(Guid id)
        {
            var post = await _postsRepository.GetPostByIdAsync(id);
            return _mapper.Map<PostWithTagsDto>(post);
        }

        public async Task<PostWithTagsDto> CreatePostAsync(PostDto postDto)
        {
            var post = await _postsRepository.CreatePostAsync(postDto);
            return _mapper.Map<PostWithTagsDto>(post);
        }

        public async Task<bool> UpdatePostAsync(UpdatePostDto updatePostDto)
        {
            return await _postsRepository.UpdatePostAsync(updatePostDto);
        }

        public async Task<bool> DeletePostAsync(Guid id)
        {
            return await _postsRepository.DeletePostAsync(id);
        }
    }
}
