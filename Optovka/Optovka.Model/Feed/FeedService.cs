using Microsoft.EntityFrameworkCore;

namespace Optovka.Model
{
    public class FeedService(ApplicationDbContext context, IInMemoryCache inMemoryCache) : IFeedService
    {
        public async Task<IEnumerable<UserPost>> GetAsync(string sectionFilter, int skip, int limit, SortingOrderType sortingOrderType)
        {
            var cacheKey = $"Feed_{sectionFilter}_{skip}_{limit}_{sortingOrderType}";

            if (inMemoryCache.Contains(cacheKey))
            {
                return inMemoryCache.Get<IEnumerable<UserPost>>(cacheKey);
            }

            Func<UserPost, bool> filter = _ => true;
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
