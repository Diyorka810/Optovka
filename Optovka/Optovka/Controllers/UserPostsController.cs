﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optovka.Model;

namespace Optovka
{//модификкация поста получить список получить инфу о посте
    [Route("api/userPost")]
    [ApiController]
    public class UserPostsController(IUserPostsService userPostsService) : Controller
    {
        [HttpPut]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Create([FromBody] UserPostDto dto)
        {
            var userId = this.HttpContext.User.FindFirst("userId")?.Value;
            if (userId == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Status = "Error", Message = "You are not authorized" });

            await userPostsService.AddAsync(dto, userId);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPatch("{userPostId}")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> Edit([FromBody] UserPostDto dto, [FromRoute] int userPostId)
        {
            var userId = HttpContext.User.FindFirst("userId")?.Value;
            if (userId == null)
                return StatusCode(
                    StatusCodes.Status401Unauthorized,
                    new { Status = "Error", Message = "You are not authorized" });

            var userPost = await userPostsService.TryGetByIdAsync(userPostId);

            if (userPost == null || userPost.AuthorUserId != userId)
                return StatusCode(
                    StatusCodes.Status401Unauthorized,
                    new { Status = "Error", Message = "You can change only your posts" });

            await userPostsService.TryUpdateAsync(dto, userPost);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPatch("takePart")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> TakePart([FromQuery] int desiredQuantity, [FromRoute] int postId)
        {
            //добавить таблицу - сохранить сколько юзер взял в определенном посте
            var userId = this.HttpContext.User.FindFirst("userId")?.Value;
            if (userId == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Status = "Error", Message = "You are not authorized" });

            var userPost = await userPostsService.TryGetByIdAsync(postId);

            if (userPost == null)
                return StatusCode(StatusCodes.Status401Unauthorized, new { Status = "Error", Message = "You can change only your posts" });

            await userPostsService.TakePartAsync(desiredQuantity, userPost);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpGet("getByTitle")]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> FindByTitle([FromQuery] string postTitle)
        {
            var userPost = await userPostsService.TryGetByTitleAsync(postTitle);

            return userPost == null
                ? StatusCode(
                    StatusCodes.Status400BadRequest,
                    new { Status = "Error", Message = "There is no post with this title" })
                : Ok(userPost);
        }

        [HttpGet]
        [Authorize(Policy = "User")]
        public async Task<IActionResult> GetAll()
        {
            var userPosts = await userPostsService.GetAllAsync();
            return Ok(userPosts);
        }
    }
}
