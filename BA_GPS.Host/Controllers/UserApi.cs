using BA_GPS.Domain.Entity;
using BA_GPS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BA_GPS.API.Apis
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date        Comments
    /// Duypn   11/03/2024  Created
    /// </Modified>
    [ApiController]
    [Route("[controller]")]
    public class UserApi : ControllerBase
    {
        private readonly UserServices _service;
        private readonly ILogger<UserApi> _logger;

        public UserApi(UserServices service, ILogger<UserApi> logger)
        {
            _service = service;
            _logger = logger;

        }

        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles ="1")]
        [HttpGet(Name = "GetUser")]
        
        public async Task<IActionResult> GetAllUsers(int pageIndex, int pageSize)
        {
            var userDataResponse = new DataListResponse<User>();
            try
            {
                userDataResponse = await _service.GetAll(pageIndex, pageSize);

            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex.Message, "An error occurred while retrieving users.");
                userDataResponse = await _service.GetAll(1, 5);
                return new StatusCodeResult(500);
            }
            return new OkObjectResult(userDataResponse);
        }

        /// <summary>
        /// Lấy thông tin người dùng theo Id
        /// </summary>
        /// <param name="id">Mã người dùng</param>
        /// <returns>trạng thái yêu cầu</returns>
        [HttpGet("{id}", Name = "GetUserById")]
        //[Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            // Create a default user with predefined values
            var user = new User();
            if (id == Guid.Empty)
            {
                return BadRequest("User id is not valid");
            }
            try
            {
                user = await _service.GetById(id);
                if (user == null)
                    return BadRequest("User not found");

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, "An error occurred while retrieving user by ID.");

                return new StatusCodeResult(500);
            }
            return new OkObjectResult(user);
        }


        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="id">Mã người dùng</param>
        /// <param name="userToUpdate">Thông tin cần cập nhật</param>
        /// <returns>trạng thái yêu cầu </returns>
        [HttpPut("{id}", Name = "PutUser")]
      
        public async Task<IActionResult> UpdateUser(Guid id, User userToUpdate)
        {
            // Create a default user with predefined values
            var user = new User();
            if (id == Guid.Empty || userToUpdate is null)
            {
                return BadRequest("Id or userToUpdate is not valid");
            }
            try
            {
                user = await _service.GetById(id);

                if (userToUpdate is null)
                {
                    return BadRequest("User not found");
                }

                if (userToUpdate.DateOfBirth == null || userToUpdate.DateOfBirth > DateTime.Now.AddYears(-18))
                {
                    return BadRequest("The age must be at least 18");
                }


                // Update user properties with the values from updatedUser
                user.FullName = userToUpdate.FullName;
                user.PhoneNumber = userToUpdate.PhoneNumber;
                user.Email = userToUpdate.Email;
                user.IsMale = userToUpdate.IsMale;
                user.DateOfBirth = userToUpdate.DateOfBirth;
                user.LastModifyDate = DateTime.UtcNow;

                await _service.Update(user);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, "An error occurred while update user.");
                return new StatusCodeResult(500);
            }

            return Ok(user);
        }

        /// <summary>
        /// Tạo người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns>trạng thái yêu cầu</returns>

        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody]User newUser)
        {
            
            // Create a default user with predefined values
            var user = new User();
            
            if (newUser is null)
            {
                BadRequest("User is not valid");
            }
            try
            {
                user = await _service.Create(newUser);

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while create user.");
                return new StatusCodeResult(500);
            }
            return Ok(user);
        }

        /// <summary>
        /// Xoá người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trạng thái yêu cầu</returns>
        [HttpPut("ban/{id}", Name = "Delete User")]
        public async Task<IActionResult> RemoveUser(Guid id)
        {

            var user = new User();

            if (id == Guid.Empty)
            {
                return BadRequest("Identity is not valid");
            }

            try
            {
                user = await _service.GetById(id);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                user.IsDeleted = true;
                user.LastModifyDate = DateTime.Now;

                await _service.Delete(id);

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while create user.");
                return new StatusCodeResult(500);
            }

            return Ok(user);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Dữ liệu nhập vào</param>
        /// <param name="type">Kiểu dữ liệu muốn tìm kiếm</param>
        /// <param name="startDate">ngày bắt đầu</param>
        /// <param name="endDate">ngày cuối </param>
        /// <param name="gender">Giới tính</param>
        /// <returns>trạng thái yêu cầu</returns>
        [HttpPost("search", Name = "SearchUsers")]
        public async Task<IActionResult> SearchUsers([FromBody] SearchRequest? searchRequest)          
        {

            var userDataResponse = new DataListResponse<User>();
            try
            {
                userDataResponse = await _service.Search(searchRequest);

            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while create user.");
                return new StatusCodeResult(500);
            }

            return Ok(userDataResponse);
        }


        /// <summary>
        /// Kiểm tra người dùng tồn tại hay không
        /// </summary>
        /// <param name="value">Giá trị kiểm tra</param>
        /// <returns>Trạng thái yêu cầu</returns>
        [HttpGet("checkExist", Name = "CheckExist")]
        public async Task<IActionResult> CheckExist(string? value)
        {
            var check = true;
            try
            {
                if (value is null)
                {
                    check = false;
                }
                else
                {
                    check = await _service.CheckExist(value);
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, "An error occurred while checking user.");
                return new StatusCodeResult(500);
            }
            return new OkObjectResult(check);
        }




    }
}
