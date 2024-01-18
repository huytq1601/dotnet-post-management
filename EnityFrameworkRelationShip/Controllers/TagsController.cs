using AutoMapper;
using EnityFrameworkRelationShip.Dtos.Tag;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Interfaces.Repository;
using EnityFrameworkRelationShip.Interfaces.Service;
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
    }
}
