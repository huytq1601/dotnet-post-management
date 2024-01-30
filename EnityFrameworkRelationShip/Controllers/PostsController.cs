﻿using EnityFrameworkRelationShip.Common;
using EnityFrameworkRelationShip.Dtos.Post;
using EnityFrameworkRelationShip.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EnityFrameworkRelationShip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IPostsService _postsService;
        public PostsController(IPostsService postsService)
        {
            _postsService = postsService;
        }

        // GET: api/Posts
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PageResponse<IEnumerable<PostWithTagsDto>>>> GetPosts([FromQuery] PostFilter filter)
        {
            var response = await _postsService.GetAllPostsAsync(filter);
            return Ok(response);
        }

        [HttpGet("others")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PageResponse<IEnumerable<PostWithTagsDto>>>> GetOtherUserPosts([FromQuery] PostFilter filter)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == "uid")!.Value;
            var response = await _postsService.GetPostsOfOtherAsync(currentUserId, filter);
            return Ok(response);
        }

        [HttpGet("currentUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<PageResponse<IEnumerable<PostWithTagsDto>>>> GetCurrentrUserPosts([FromQuery] PostFilter filter)
        {
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == "uid")!.Value;
            var response = await _postsService.GetPostsByUserAsync(currentUserId, filter);
            return Ok(response);
        }

        // GET: api/Posts/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PostWithTagsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PostWithTagsDto>> GetPost(Guid id)
        {
            var postWithTags = await _postsService.GetPostByIdAsync(id);

            return postWithTags == null ? NotFound() : Ok(postWithTags);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PostWithTagsDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PostWithTagsDto>> CreatePost(PostDto postDto)
        {
            if (postDto == null)
            {
                return BadRequest("Post cannot be null");
            }

            string userId = User.Claims.FirstOrDefault(c => c.Type == "uid")!.Value;

            var postWithTags = await _postsService.CreatePostAsync(postDto, userId);
            return CreatedAtAction(nameof(GetPost), new { id = postWithTags.Id }, postWithTags);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            bool deleteResult = await _postsService.DeletePostAsync(id);
            if (!deleteResult)
            {
                return NotFound($"Post with ID: {id} not found.");
            }

            return NoContent();
        }
    }
}
