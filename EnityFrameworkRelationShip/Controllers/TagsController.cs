using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Tag;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace EnityFrameworkRelationShip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagsRepository _tagsRepository;
        private readonly IMapper _mapper;

        public TagsController(ITagsRepository tagsRepository, IMapper mapper)
        {
            _tagsRepository = tagsRepository;
            _mapper = mapper;
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            var tags = await _tagsRepository.GetAllTagsAsync();
            var tagTdos = _mapper.Map<List<TagDto>>(tags);
            return Ok(tagTdos);
        }
    }
}
