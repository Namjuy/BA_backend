
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using System.Text;

using BA_GPS.Application.Core.Interface;
using BA_GPS.Application.Interface;
using BA_GPS.Application.Model;
using BA_GPS.Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BA_GPS.Infrastructure.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/01/2024 Created
    /// </Modified>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Tạo ra token
        /// </summary>
        /// <param name="user"></param>
        /// <returns> chuỗi token</returns>
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth0:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Auth0:Issuer"],
                audience: _configuration["Auth0:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Đăng nhập
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>token nếu đúng tài khoản, mật khẩu</returns>
        public async Task<LoginRequest> Login(string username, string password)
        {
            User user = await _userRepository.LoginAsync(username, password);

            if (user != null && _passwordHasher.Verify(user.PassWordHash, password))
            {
                string jwtToken = GenerateJwtToken(user);
                return new LoginRequest
                {
                    Username = user.UserName,
                    Password = user.PassWordHash,
                    Token = jwtToken
                };
            }

            return null;
        }
    }
}
