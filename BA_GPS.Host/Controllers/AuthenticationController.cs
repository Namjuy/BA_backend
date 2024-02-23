using System;
using System.Threading.Tasks;
using BA_GPS.Application.Core.Interface;
using BA_GPS.Application.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BA_GPS.Host.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/01/2024 Created
    /// </Modified>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("Username and password are required.");
            }

            var result = await _authService.Login(username, password);

            if (result == null || string.IsNullOrWhiteSpace(result.Token))
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(new { Token = result.Token });
        }
    }
}
