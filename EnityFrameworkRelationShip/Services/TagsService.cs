using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Tag;
using EnityFrameworkRelationShip.Interfaces.Repository;
using EnityFrameworkRelationShip.Interfaces.Service;
using EnityFrameworkRelationShip.Models;
using Microsoft.EntityFrameworkCore;

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
            var tags = await _tagRepository.GetAllAsync();
            var tagDtos = _mapper.Map<List<TagDto>>(tags);
            return tagDtos;
        }
    }
}
