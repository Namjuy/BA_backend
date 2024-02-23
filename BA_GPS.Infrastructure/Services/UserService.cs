
using BA_GPS.Application.Core.Interface;
using BA_GPS.Application.Interface;
using BA_GPS.Domain.Entity;

namespace BA_GPS.Infrastructure.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date        Comments
    /// Duypn   14/01/2024  Created
    /// </Modified>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        /// <summary>
        /// Tạo người dùng
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<User> CreateUserAsync(User user)
        {
            // Hash password
            user.PassWordHash = _passwordHasher.Hash(user.PassWordHash);

            // Create user in the repository asynchronously
            return await _userRepository.CreateUserAsync(user);
        }

        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
        }

        /// <summary>
        /// Lấy người dùng theo Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        /// <summary>
        /// Tìm kiếm người dùng
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="gender"></param>
        /// <returns></returns>
        public async Task<List<User>> SearchUserAsync(string input, string type, DateTime startDate, DateTime endDate, byte? gender)
        {
            return await _userRepository.SearchUserAsync(input, type, startDate, endDate, gender);
        }

        /// <summary>
        /// Xoá người dùng
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task DeleteUserAsync(Guid userId)
        {
            await _userRepository.DeleteUserAsync(userId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        public async Task ChangePassword(Guid id, string oldPassword ,string newPassword, string confirmPassword)
        {
            
            if(newPassword == confirmPassword)
            {
                await _userRepository.ChangePassword(id, oldPassword,_passwordHasher.Hash(newPassword));
            }
        
        }
    }
}
