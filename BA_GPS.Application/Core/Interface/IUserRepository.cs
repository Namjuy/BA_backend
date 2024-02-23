
using BA_GPS.Domain.Entity;

namespace BA_GPS.Application.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/01/2024 Created
    /// </Modified>
    public interface IUserRepository
    {
        Task<User> CreateUserAsync(User user);

        Task<List<User>> GetAllUsersAsync();

        Task UpdateUserAsync(User user);

        Task<User> GetUserByIdAsync(Guid id);

        Task<User> LoginAsync(string username, string password);

        Task<List<User>> SearchUserAsync(string input, string type, DateTime startDate, DateTime endDate, byte? gender);

        Task DeleteUserAsync(Guid userId);

        Task ChangePassword(Guid id,string oldPassword,string newPassword);

    }
}
