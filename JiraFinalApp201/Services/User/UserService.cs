using JiraFinalApp201.Models.Database;
using Microsoft.EntityFrameworkCore;
using UserModel = JiraFinalApp201.Models.Authentication.User;

namespace JiraFinalApp201.Services.User
{
    public class UserService : IUserService
    {
        private readonly JiraFinalApp201Db _context;

        public UserService(JiraFinalApp201Db context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserModel>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<UserModel?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<UserModel?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<UserModel> CreateUserAsync(UserModel user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> AuthenticateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            return user != null;
        }
    }
}
