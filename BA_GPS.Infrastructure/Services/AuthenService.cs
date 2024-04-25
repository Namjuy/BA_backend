using System;
using BA_GPS.Common.Authentication;
using BA_GPS.Common.Modal;
using BA_GPS.Domain.Entity;
using BA_GPS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
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
        private readonly GenericRepository<User> _repository;
        private readonly GenericRepository<UserPermission> _repositoryPermission;
        private readonly PasswordHasher _passwordHasher;
        private readonly ILogger<AuthenService> _logger;

        public AuthenService(JwtUltis jwt
            , PasswordHasher passwordHasher
            , ILogger<AuthenService> logger
            , GenericRepository<User> repository
            , GenericRepository<UserPermission> repositoryPermision)
		{
            _jwt = jwt;
            _repository = repository;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _repositoryPermission = repositoryPermision;
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
                var user = await _repository.GetAll().Where(u => !u.IsDeleted).FirstOrDefaultAsync(user => user.UserName == loginRequest.Username);
                if (user == null)
                {
                    return "";
                }
                if (_passwordHasher.Verify(user.PassWordHash, loginRequest.Password))
                {

                    return _jwt.GenerateJwtToken(loginRequest.Username, GetPermissionId(user.Id));
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


        private byte GetPermissionId(Guid id) => _repositoryPermission.GetAll()
            .Where(u => u.UserId == id)
            .FirstOrDefault().PermissionId;
    }
}

