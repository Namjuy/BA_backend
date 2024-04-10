using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   09/04/2024 Created
/// </Modified>
namespace BA_GPS.Common.Authentication
{
	public class JwtUltis
	{
		private readonly IConfiguration _configuration;

		public JwtUltis(IConfiguration configuration)
		{
			_configuration = configuration;
		}

        /// <summary>
        /// Tạo token
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <param name="role">Quyền</param>
        /// <returns>Chuỗi ký tự bao gồm thông tin đã được mã hoá</returns>
        public string GenerateJwtToken(string username, byte? permission)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, permission?.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth0:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Auth0:Issuer"],
                audience: _configuration["Auth0:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

