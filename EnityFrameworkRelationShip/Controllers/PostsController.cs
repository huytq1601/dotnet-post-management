using AutoMapper;
using EnityFrameworkRelationShip.Data;
using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Interfaces.Service;
using EnityFrameworkRelationShip.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;
using System.Security.Claims;

namespace EnityFrameworkRelationShip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : Controller
    {
        private readonly IPostsService _postsService;
        public PostsController(IPostsService postsService)
        {
            _postsService = postsService;
        }

        // GET: api/Posts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostWithTagsDto>>> GetPosts([FromQuery(Name = "tag")] string? tag = null)
        {
            var postWithTagsDtos = await (tag != null ? _postsService.GetAllPostsAsync(tag) : _postsService.GetAllPostsAsync());
            return Ok(postWithTagsDtos);
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PostWithTagsDto>> GetPost(Guid id)
        {
            var postWithTags = await _postsService.GetPostByIdAsync(id);
            if (postWithTags == null)
            {
                return NotFound();
            }

            return Ok(postWithTags);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<PostWithTagsDto>> CreatePost(PostDto postDto)
        {
            if (postDto == null)
            {
                return BadRequest("Post cannot be null");
            }

            try
            {
                string userId = User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value ?? string.Empty;
                if(userId.Length == 0)
                {
                    return Unauthorized();
                }

                var postWithTags = await _postsService.CreatePostAsync(postDto, userId);
                return CreatedAtAction(nameof(GetPost), new { id = postWithTags.Id }, postWithTags);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the post");
            }
           
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePost(Guid id, UpdatePostDto updatePostDto)
        {
            if (updatePostDto == null || updatePostDto.Id != id)
            {
                return BadRequest("Invalid post data");
            }

            bool updateResult = await _postsService.UpdatePostAsync(updatePostDto);
            if (!updateResult)
            {
                return NotFound($"Post with ID: {id} not found.");
            }

            return NoContent(); // Returns a 204 No Content, indicating that the resource was updated successfully.
        }

        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            bool deleteResult = await _postsService.DeletePostAsync(id);
            if (!deleteResult)
            {
                return NotFound($"Post with ID: {id} not found.");
            }

            return NoContent(); // Returns a 204 No Content response, indicating successful deletion without any content to return.
        }
    }
}
