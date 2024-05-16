using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Optovka.Model
{
    public class UserPostsService(ApplicationDbContext context, InMemoryCache inMemoryCache) : IUserPostsService
    {
        public async Task AddAsync(UserPostDto dto, string userId)
        {
            var userPost = new UserPost(dto, userId);

            var posts = context.UserPosts;
            posts.Add(userPost);
            await context.SaveChangesAsync();
        }

        public async Task TryUpdateAsync(UserPostDto dto, UserPost userPost)
        {
            userPost.Title = dto.Title;
            userPost.Section = dto.Section;
            userPost.Description = dto.Description;
            userPost.RequiredQuantity = dto.RequiredQuantity;

            context.UserPosts.Update(userPost);
            await context.SaveChangesAsync();
        }

        public async Task<UserPost?> TryGetByIdAsync(int userPostId)
        {
            return await context.UserPosts.FirstOrDefaultAsync(post => post.Id == userPostId);
        }

        public async Task TakePartAsync(int desiredQuantity, UserPost userPost)
        {
            userPost.TakenQuantity += desiredQuantity;
            var posts = context.UserPosts.Update(userPost);
            await context.SaveChangesAsync();
        }

        public async Task<List<UserPost>> GetAllAsync()
        {
            var userPosts = await context.UserPosts.ToListAsync();
            return userPosts;
        }

        public async Task<UserPost?> TryGetByTitleAsync(string title)
        {
            var userPost = await context.UserPosts.FindAsync(title);
            return userPost;
        }

        public async Task<IEnumerable<UserPost>> GetAsync(string sectionFilter, int skip, int limit, SortingOrderType sortingOrderType)
        {
            var cacheKey = $"Feed_{sectionFilter}_{skip}_{limit}_{sortingOrderType}";

            if (inMemoryCache.Contains(cacheKey))
            {
                return inMemoryCache.Get<IEnumerable<UserPost>>(cacheKey);
            }

            Func<UserPost, bool> filter = _ => true;
            // move to FeedService
            if (sectionFilter != "null")
                filter = post => post.Section == sectionFilter;

            var userPosts = (await context
                .UserPosts
                .ToListAsync())
                .Where(up => filter(up))
                .Skip(skip)
                .Take(limit);

            var orderedPosts = sortingOrderType switch
            {
                SortingOrderType.Asc => userPosts.OrderBy(up => up.Id),
                SortingOrderType.Desc => userPosts.OrderByDescending(up => up.Id),
                _ => throw new InvalidOperationException($"{sortingOrderType} this sorting order type is nt supported")
            };
            inMemoryCache.Set(cacheKey, orderedPosts);
            return orderedPosts;
        }
    }
}
