using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Optovka;

namespace Optovka.Model
{
    public interface IUserPostsService
    {
        Task AddAsync(UserPostModel userPostModel, string authorId);
        Task<List<UserPost>> GetAllAsync();
        Task<UserPost?> TryGetByTitleAsync(string title);
        Task<UserPost?> TryGetByIdAsync(int userPostId);
        Task TryUpdateAsync(UserPostModel dto, UserPost userPost);
        Task TakePartAsync(int desiredQuantity, UserPost userPost, string userId);
        bool HasFreeQuantity(UserPost userPost, int desiredQuantity);
    }
}
