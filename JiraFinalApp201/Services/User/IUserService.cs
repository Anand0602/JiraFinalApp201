using System.Collections.Generic;
using System.Threading.Tasks;
using UserModel = JiraFinalApp201.Models.Authentication.User;

namespace JiraFinalApp201.Services.User
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetAllUsersAsync();
        Task<UserModel?> GetUserByIdAsync(int id);
        Task<UserModel?> GetUserByUsernameAsync(string username);
        Task<UserModel> CreateUserAsync(UserModel user);
        Task<bool> AuthenticateUserAsync(string username, string password);
    }
}
