using System;
using System.ComponentModel.DataAnnotations;

namespace BA_GPS.Common.Modal
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   14/01/2024 Created
    /// </Modified>
    public sealed record class LoginRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }


      
    }
}

