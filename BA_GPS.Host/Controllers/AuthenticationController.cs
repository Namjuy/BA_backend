using System;
using System.Threading.Tasks;
using BA_GPS.API.Apis;

using BA_GPS.Common.Modal;
using BA_GPS.Common.Service;
using BA_GPS.Domain.Entity;
using BA_GPS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BA_GPS.Host.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/03/2024 Created
    /// </Modified>

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ILogger<AuthenticationController> _logger;
        private readonly UserServices _service;
        public AuthenticationController(AuthService authService, ILogger<AuthenticationController> logger, UserServices services)
        {
            _authService = authService;
            _logger = logger;
            _service = services;
        }

        /// <summary>
        /// Đăng nhập thông tin tài khoản và mật khẩu
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
     
            var token = "";
            
            if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Username and password are required.");
            }

            try
            {
                User user = await _service.GetUserByLoginInput(loginRequest.Username);

                if(user is null)
                {
                    return BadRequest("User is not exist");
                }

                
                token = _authService.GenerateToken(loginRequest, user.PassWordHash, user.UserType);

                if (string.IsNullOrWhiteSpace(token))
                {
                    return BadRequest("Invalid username or password.");
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing login request.");
                return new StatusCodeResult(500);
            }
            return Ok(token);
        }

    }
}
