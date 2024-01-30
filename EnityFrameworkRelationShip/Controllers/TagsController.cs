using Microsoft.AspNetCore.Mvc;
using PostManagement.Application.Dtos.Tag;
using PostManagement.Application.Interfaces;

namespace PostManagement.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagsService _tagsService;

        public TagsController(ITagsService tagsService)
        {
            _tagsService = tagsService;
        }

        // GET: api/Tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            var tagTdos = await _tagsService.GetAllTagsAsync();
            return Ok(tagTdos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTagById(Guid id)
        {
            var tagDto = await _tagsService.GetTagByIdAsync(id);

            if (tagDto == null)
                return NotFound();

            return Ok(tagDto);
        }
    }
}
