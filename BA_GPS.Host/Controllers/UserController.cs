

using BA_GPS.Application.Core.Interface;
using BA_GPS.Application.Interface;
using BA_GPS.Domain.Entity;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BA_GPS.Host.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   16/01/2024 Created
    /// </Modified>
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _config;

        public UserController(IUserService userService, IPasswordHasher passwordHasher, ILogger<UserController> logger, IConfiguration config)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        /// <returns></returns>
        //[Authorize]
        [HttpGet(Name = "GetUser")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usersFromService = await _userService.GetAllUsersAsync();
                Log.Information("User Information => {@Result}", usersFromService);
                return Ok(usersFromService);


            }

            catch (Exception ex)
            {
                return BadRequest($"Error getting user: {ex.Message}");
            }
            //return BadRequest(_config.GetConnectionString("BAconnection"));
        }

        /// <summary>
        /// Tạo danh sách người dùng
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost(Name = "PostUser")]
        public async Task<IActionResult> PostUser(User user)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(user);
                return CreatedAtAction(nameof(Get), new { id = createdUser.UserId }, createdUser);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return BadRequest($"Error creating user: {ex.Message}");
            }
        }


        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updatedUser"></param>
        /// <returns></returns>
        [HttpPut("{id}", Name = "PutUser")]
        public async Task<IActionResult> PutUser(Guid id, User updatedUser)
        {
            try
            {
                var existingUser = await _userService.GetUserByIdAsync(id);

                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                // Update user properties with the values from updatedUser
                existingUser.UserName = updatedUser.UserName;
                existingUser.FullName = updatedUser.FullName;
                existingUser.DateOfBirth = updatedUser.DateOfBirth;
                existingUser.IsMale = updatedUser.IsMale;
                existingUser.PhoneNumber = updatedUser.PhoneNumber;
                existingUser.Email = updatedUser.Email;
                existingUser.Address = updatedUser.Address;
                existingUser.LastModifyDate = DateTime.Now;
                existingUser.IsDeleted = updatedUser.IsDeleted;
                await _userService.UpdateUserAsync(existingUser);

                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return BadRequest($"Error updating user: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật thông tin xoá của người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("ban/{id}", Name = "Delete User")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            try
            {
                var existingUser = await _userService.GetUserByIdAsync(id);

                if (existingUser == null)
                {
                    return NotFound($"User with ID {id} not found");
                }

                existingUser.IsDeleted = true;
                existingUser.LastModifyDate = DateTime.Now;

                await _userService.DeleteUserAsync(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error delete user: {ex.Message}");
            }
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
        [HttpGet("search", Name = "SearchUsers")]
        public async Task<IActionResult> SearchUsers(
            [FromQuery] string? input,
            [FromQuery] string? type,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] byte? gender)
        {
            try
            {
                var searchResults = await _userService.SearchUserAsync(input, type, startDate, endDate, gender);
                return Ok(searchResults);
            }
            catch (Exception ex)
            {

                return BadRequest($"Error searching users: {ex.Message}");
            }
        }

        /// <summary>
        /// Thay đổi mật khẩu
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="confirmPassword"></param>
        /// <returns></returns>
        [HttpPut("changePassword/{id}", Name = "Change Password")]
        public async Task<IActionResult> ChangePassword(Guid id, string oldPassword, string newPassword, string confirmPassword)
        {
            try
            {

                if (newPassword == null || id == Guid.Empty)
                {
                    return BadRequest("Invalid input data");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Assuming you have a service to handle user-related operations
                var user = await _userService.GetUserByIdAsync(id);

                if (user == null)
                {
                    return NotFound($"User with id {id} not found");
                }

                await _userService.ChangePassword(id, oldPassword, newPassword, confirmPassword);
                return Ok(newPassword);
            }
            catch(Exception ex)
            {
                return BadRequest($"Error change password: {ex.Message}");
            }

        }



    }
}

