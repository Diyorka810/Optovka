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
        Task AddAsync(UserPostDto dto, string userId);
        Task<List<UserPost>> GetAllAsync();
        Task<UserPost?> TryGetByTitleAsync(string title);
        Task<UserPost?> TryGetByIdAsync(int userPostId);
        Task TryUpdateAsync(UserPostDto dto, UserPost userPost);
        Task TakePartAsync(int desiredQuantity, UserPost userPost, string userId);
        Task<bool> HasFreeQuantity(UserPost userPost, int desiredQuantity);
    }
}
