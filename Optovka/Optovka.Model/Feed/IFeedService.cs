namespace Optovka.Model
{
    public interface IFeedService
    {
        Task<IEnumerable<UserPost>> GetAsync(string sectionFilter, int skip, int limit, SortingOrderType sortingOrderType);
    }
}
