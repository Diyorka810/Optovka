using Microsoft.EntityFrameworkCore;

namespace Optovka.Model
{
    public class UserPostsService(ApplicationDbContext context) : IUserPostsService
    {
        public async Task AddAsync(UserPostModel userPostModel, string authorId)
        {
            var userPost = new UserPost(userPostModel, authorId);

            var posts = context.UserPosts;
            posts.Add(userPost);
            await context.SaveChangesAsync();
        }

        public async Task TryUpdateAsync(UserPostModel userPostModel, UserPost userPost)
        {
            userPost.Title = userPostModel.Title;
            userPost.Section = userPostModel.Section;
            userPost.Description = userPostModel.Description;
            userPost.RequiredQuantity = userPostModel.RequiredQuantity;

            context.UserPosts.Update(userPost);
            await context.SaveChangesAsync();
        }

        public async Task<UserPost?> TryGetByIdAsync(int userPostId)
        {
            var users = context.Users.ToList();
            return await context.UserPosts.FirstOrDefaultAsync(post => post.Id == userPostId);
        }

        public bool HasFreeQuantity(UserPost userPost, int desiredQuantity)
        {
            if (userPost.TakenQuantity + desiredQuantity > userPost.RequiredQuantity)
                return false;

            return true;
        }

        public async Task TakePartAsync(int desiredQuantity, UserPost userPost, string userId)
        {
            userPost.TakenQuantity += desiredQuantity;
            context.UserPosts.Update(userPost);

            var applicationUserUserPost = new ApplicationUserUserPost() { 
                ParticipatedUserPostId = userPost.Id, 
                ParticipatingUserId = userId, 
                TakenQuantity = desiredQuantity 
            };
            context.ApplicationUserUserPosts.Add(applicationUserUserPost);
            await context.SaveChangesAsync();
        }

        public async Task<List<UserPost>> GetAllAsync()
        {
            var userPosts = await context.UserPosts.ToListAsync();
            return userPosts;
        }

        public async Task<UserPost?> TryGetByTitleAsync(string title)
        {
            var userPost = await context.UserPosts.FirstOrDefaultAsync(x => x.Title == title);
            return userPost;
        }
    }
}