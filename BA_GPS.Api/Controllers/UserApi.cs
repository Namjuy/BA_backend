using System;
using BA_GPS.Domain.DTO;
using BA_GPS.Domain.Entity;
using BA_GPS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date        Comments
/// Duypn   11/03/2024  Created
/// </Modified>
namespace BA_GPS.Api.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class UserApi : ControllerBase
    {
        private readonly UserService _service;
        private readonly ILogger<UserApi> _logger;

        public UserApi(UserService service, ILogger<UserApi> logger)
        {
            _service = service;
            _logger = logger;

        }
        /// <summary>
        /// Lấy danh sách người dùng
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "0")]
        [HttpGet(Name = "GetUser")]
        public async Task<IActionResult> GetUser(int pageIndex, int pageSize)
        {
            var userDataResponse = new DataListResponse<User>();
            try
            {
                userDataResponse = await _service.Get(pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, "Lỗi khi lấy danh sách người dùng");
                userDataResponse = await _service.Get(1, 5);
                return new StatusCodeResult(500);
            }
            return new OkObjectResult(userDataResponse);
        }

        /// <summary>
        /// Lấy thông tin người dùng theo Id
        /// </summary>
        /// <param name="id">Mã người dùng</param>
        /// <returns>trạng thái yêu cầu</returns>
        [Authorize(Roles = "0" )]
        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<IActionResult> GetUserById(Guid id)
        {

            if (id == Guid.Empty)
            {
                return BadRequest("Id người dùng không hợp lệ");
            }
            try
            {
                if (await _service.GetById(id) == null)

                    return NotFound("Không tìm thấy người dùng");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, "Lỗi lấy thông tin người dùng theo Id");

                return new StatusCodeResult(500);
            }
            return new OkObjectResult(id);
        }

        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="userToUpdate">Thông tin người dùng</param>
        /// <returns>Trạng thái yêu cầu</returns>
        [Authorize(Roles = "0")]
        [HttpPut("{id}", Name = "PutUser")]
        public async Task<IActionResult> UpdateUser(User userToUpdate)
        {
            if (userToUpdate.Id == Guid.Empty || !_service.CheckUserValid(userToUpdate))
            {
                return BadRequest("Thông tin cập nhật không chính xác");
            }

            try
            {
                if(await _service.GetById(userToUpdate.Id) == null)
                {
                    return NotFound("Không tìm thấy người dùng phù hợp");
                }

                await _service.Update(userToUpdate);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, "Lỗi cập nhật người dùng");
                return new StatusCodeResult(500);
            }

            return Ok(userToUpdate);
        }

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="newUser">Thông tin người dùng tạo mới</param>
        /// <returns>Trạng thái yêu cầu</returns>
        [HttpPost(Name = "CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] User newUser)
        {
            if (!_service.CheckUserValid(newUser))
            {
                return BadRequest("Thông tin người dùng không hợp lệ");
            }
            try
            {
                await _service.Create(newUser);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while create user.");
                return new StatusCodeResult(500);
            }
            return Ok(newUser);
        }

        /// <summary>
        /// Xoá người dùng
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Trạng thái yêu cầu</returns>
        [HttpPut("ban/{id}", Name = "Delete User")]
        public async Task<IActionResult> RemoveUser(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Id không hợp lệ");
            }

            try
            {
                var user = await _service.GetById(id);

                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                user.IsDeleted = true;
                user.LastModifyDate = DateTime.UtcNow;

                await _service.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "Lỗi khi xoá người dùng.");
                return new StatusCodeResult(500);
            }

            return Ok(id);
        }

        /// <summary>
        /// Tìm kiếm
        /// </summary>
        /// <param name="searchRequest">Thông tin tìm kiếm</param>
        /// <returns></returns>
        [HttpPost("search", Name = "SearchUsers")]
        public async Task<IActionResult> SearchUsers([FromBody] SearchRequest? searchRequest)
        {
            try
            {
                 await _service.Search(searchRequest);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, "An error occurred while create user.");
                return new StatusCodeResult(500);
            }

            return Ok(searchRequest);
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
                    check = await _service.CheckUserNameExist(value);
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

