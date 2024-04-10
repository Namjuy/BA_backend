using System;
using BA_GPS.Common.Authentication;
using BA_GPS.Common.Modal;
using BA_GPS.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   09/04/2024 Created
/// </Modified>
namespace BA_GPS.Infrastructure.Services
{
	public class AuthenService
	{
        private readonly JwtUltis _jwt;
        private readonly UserRepository _userRepository;
        private readonly PasswordHasher _passwordHasher;
        private readonly ILogger<AuthenService> _logger;

        public AuthenService(JwtUltis jwt, UserRepository userRepository, PasswordHasher passwordHasher, ILogger<AuthenService> logger)
		{
            _jwt = jwt;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
		}

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="loginRequest">Thông tin đăng nhập</param>
        /// <returns>Chuỗi token</returns>
        public async Task<string> Login(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userRepository.GetByUserName(loginRequest.Username);
                if (user == null)
                {
                    return "";
                }
                if (_passwordHasher.Verify(user.PassWordHash, loginRequest.Password))
                {
                    return _jwt.GenerateJwtToken(loginRequest.Username, user.PermissionId);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token for user: {username}", loginRequest.Username);

            }
            return "";
        }

        /// <summary>
        /// Kiểm tra thông tin đăng nhập hợp lệ
        /// </summary>
        /// <param name="loginRequest">Thông tin đăng nhập</param>
        /// <returns>Kết quả True/False</returns>
        public bool CheckLoginRequestValid(LoginRequest loginRequest)
        {
           if(string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return false;
            }
            return true;
        }
    }
}

