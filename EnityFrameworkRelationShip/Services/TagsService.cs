using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Tag;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Models;

namespace EnityFrameworkRelationShip.Services
{
    public class TagsService: ITagsService
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;

        public TagsService(IRepository<Tag> tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
        {
            var tags = await _tagRepository.FindManyAsync(t => !t.IsDeleted);
            return _mapper.Map<List<TagDto>>(tags);
        }

        public async Task<TagDto> GetTagByIdAsync(Guid id)
        {
            var tag = await _tagRepository.FindOneAsync(t => !t.IsDeleted && t.Id == id);
            return _mapper.Map<TagDto>(tag);
        }
    }
}
