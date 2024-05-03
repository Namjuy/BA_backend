
using BA_GPS.Common.Modal;
using BA_GPS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   11/03/2024 Created
/// </Modified>
namespace BA_GPS.Api.Controllers
{
    [AllowAnonymous]
    [Route("[controller]")]
    [ApiController]
    public class AuthenApi : ControllerBase
    {

        private readonly AuthenService _authenService;
        private readonly ILogger<AuthenApi> _logger;

        public AuthenApi(AuthenService authenService, ILogger<AuthenApi> logger)
        {
            _authenService = authenService;
            _logger = logger;

        }

        /// <summary>
        /// Đăng nhập 
        /// </summary>
        /// <param name="username">Tên tài khoản</param>
        /// <param name="password">Mật khẩu</param>
        /// <returns>Token</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            string? token;
            try
            {
                if (!_authenService.CheckLoginRequestValid(loginRequest))
                {
                    return BadRequest("Tên đăng nhập hoặc mật khẩu không hợp lệ.");
                }
                token = await _authenService.Login(loginRequest);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi đăng nhập");
                return new StatusCodeResult(500);
            }
            return Ok(token);
        }

    }
}

