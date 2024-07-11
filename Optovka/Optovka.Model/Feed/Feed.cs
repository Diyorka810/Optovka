namespace Optovka.Model
{
    public class Feed
    {
        public List<UserPost> Posts { get; set; }
        public List<UserPost> FilteredPosts { get; set; }
        private readonly ApplicationDbContext _context;

        public Feed(ApplicationDbContext context)
        {
            _context = context;
            Posts = _context.UserPosts.ToList();
        }

        public List<UserPost> FilterBy(string key, string SectionName)
        {
            FilteredPosts = Posts.OrderBy(x => x.СreatedAt).Where(x => x.Section == SectionName).ToList();
            return FilteredPosts;
        }
    }
}