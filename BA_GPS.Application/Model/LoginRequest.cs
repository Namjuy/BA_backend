using System;
using System.ComponentModel.DataAnnotations;

namespace BA_GPS.Application.Model
{
    public class LoginRequest
	{
		[Required]
		public string Username { get; set; }

		[Required]
		public string Password { get; set; }

		public string Token { get; set; }

	}
}

