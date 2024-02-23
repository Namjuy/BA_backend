using BA_GPS.Application.Core.Interface;
using BA_GPS.Application.Interface;
using BA_GPS.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace BA_GPS.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   16/01/2024 Created
    /// </Modified>
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _userDbContext;
        private readonly IPasswordHasher _passwordHasher;

        public UserRepository(UserDbContext userDbContext, IPasswordHasher passwordHasher)
        {
            this._userDbContext = userDbContext;
            this._passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                _userDbContext.Users.Add(user);
                await _userDbContext.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate
                throw new Exception("Error creating user", ex);
            }
        }

        /// <summary>
        /// Lấy ra danh sách người dùng mà không bị đánh dấu 'xoá' và được sắp xếp theo thời gian chỉnh sửa 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await _userDbContext.Users
                    .Where(u => !u.IsDeleted)
                    .OrderByDescending(u => u.LastModifyDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate
                throw new Exception("Error getting all users", ex);
            }
        }

        /// <summary>
        /// Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task UpdateUserAsync(User user)
        {
            try
            {
                User existingUser = await _userDbContext.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);

                if (existingUser != null)
                {
                    existingUser.FullName = user.FullName;
                    existingUser.DateOfBirth = user.DateOfBirth;
                    existingUser.IsMale = user.IsMale;
                    existingUser.PhoneNumber = user.PhoneNumber;
                    existingUser.Email = user.Email;
                    existingUser.Address = user.Address;
                    existingUser.LastModifyDate = DateTime.UtcNow;
                    await _userDbContext.SaveChangesAsync();
                }
                else
                {
                    throw new ArgumentException("User not found", nameof(user.UserId));
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate
                throw new Exception("Error updating user", ex);
            }
        }


        /// <summary>
        /// Tìm kiếm người dùng theo Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trả về người dùng</returns>
        /// <exception cref="Exception">Báo lỗi lấy người dùng theo id</exception>
        public async Task<User> GetUserByIdAsync(Guid id)
        {
            try
            {
                return await _userDbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate
                throw new Exception($"Error getting user by id: {id}", ex);
            }
        }

        /// <summary>
        /// Kiểm tra đăng nhập
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>người dùng</returns>
        /// <exception cref="Exception"></exception>
        public async Task<User> LoginAsync(string username, string password)
        {
            try
            {
                return await _userDbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate
                throw new Exception("Error logging in", ex);
            }
        }

        /// <summary>
        /// Tìm kiếm người dùng theo giá trị nhập vào 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="type"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="gender"></param>
        /// <returns>danh sách người dùng</returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<User>> SearchUserAsync(string input, string type, DateTime startDate, DateTime endDate, byte? gender)
        {
            try
            {
                IQueryable<User> query = _userDbContext.Users.Where(u => !u.IsDeleted);

                if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(input))
                {
                    switch (type.ToLower())
                    {
                        case "username":
                            query = query.Where(u => u.UserName.Contains(input));
                            break;

                        case "fullname":
                            query = query.Where(u => u.FullName.Contains(input));
                            break;

                        case "phonenumber":
                            query = query.Where(u => u.PhoneNumber.Contains(input));
                            break;

                        case "email":
                            query = query.Where(u => u.Email.Contains(input));
                            break;

                        default:
                            break;
                    }
                }

                if (startDate != default(DateTime) || endDate != default(DateTime))
                {
                    query = query.Where(u =>
                        (startDate == default(DateTime) || u.LastModifyDate >= startDate) &&
                        (endDate == default(DateTime) || u.LastModifyDate <= endDate)
                    );
                }

                if (gender.HasValue)
                {
                    query = query.Where(u => u.IsMale == gender.Value);
                }

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate
                throw new Exception("Error searching user", ex);
            }
        }

        /// <summary>
        /// 'Xoá' người dùng khỏi danh sách
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task DeleteUserAsync(Guid userId)
        {
            try
            {
                User existingUser = await _userDbContext.Users.FirstOrDefaultAsync(u => u.UserId == userId);

                if (existingUser != null)
                {
                    existingUser.IsDeleted = true;
                    existingUser.LastModifyDate = DateTime.UtcNow;
                    existingUser.DeletedDate = DateTime.UtcNow;
                    await _userDbContext.SaveChangesAsync();
                }
                else
                {
                    throw new ArgumentException("UserId not found", nameof(userId));
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate
                throw new Exception($"Error deleting user with id: {userId}", ex);
            }
        }


        /// <summary>
        /// Thay đổi mật khẩu cho tài khoản người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task ChangePassword(Guid id, string oldPassword, string newPassword)
        {
            try
            {
                User existingUser = await _userDbContext.Users.FirstOrDefaultAsync(u => u.UserId == id);

                if (existingUser != null)
                {
                    if (_passwordHasher.Verify(existingUser.PassWordHash, oldPassword))
                    {
                        existingUser.PassWordHash = newPassword;
                        await _userDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw new ArgumentException("Password is not true");
                    }
                }
                else
                {
                    throw new ArgumentException("UserId not found", nameof(id));
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as appropriate
                throw new Exception($"Error changing password for user with id: {id}", ex);
            }
        }
    }
}
