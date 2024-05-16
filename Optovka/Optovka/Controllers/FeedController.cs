using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Optovka.Model;

namespace Optovka;

[Route("api/feed")]
[ApiController]
public class FeedController(IUserPostsService userPostsService) : Controller
{
    [HttpGet]
    [Authorize(Policy = "User")]
    public async Task<IActionResult> Index(
        [FromQuery] SortingOrderType sortingOrderType,
        // null => no filter
        // value => equals to value
        [FromQuery] string? sectionFilter,
        [FromQuery] int skip,
        [FromQuery] int limit = 50)
    {
        if (limit is > 50 or <= 0)
            return StatusCode(StatusCodes.Status400BadRequest,
                new { Status = "Error", Message = "From 1 to 50 posts at one request is allowed." });
        if (skip <= 0)
            return StatusCode(StatusCodes.Status400BadRequest,
                new { Status = "Error", Message = "Skip value must be positive." });

        var userPosts =  await userPostsService.GetAsync(sectionFilter = sectionFilter, skip = skip, limit = limit, sortingOrderType = sortingOrderType);
        return Ok(userPosts);
    }
}

