using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BA_GPS.Common.Authentication;
using BA_GPS.Common.Modal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;


namespace BA_GPS.Common.Service
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
       
        private readonly ILogger<AuthService> _logger;
        private readonly PasswordHasher _hash;

        public AuthService(PasswordHasher passwordHasher, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _hash = passwordHasher;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger;
        }

        private string GenerateJwtToken(string username, byte? role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role?.ToString() ?? "1")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Auth0:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Auth0:Issuer"],
                audience: _configuration["Auth0:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );  

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateToken(LoginRequest loginRequest, string hashedPassword, byte? role)
        {
            var token = "";
            try
            {
                if (_hash.Verify(hashedPassword, loginRequest.Password))
                {
                    token = GenerateJwtToken(loginRequest.Username, role);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating token for user: {username}", loginRequest.Username);
             
            }
            return token;
        }
    }
}
