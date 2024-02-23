using System;
namespace BA_GPS.Application.Model
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/01/2024 Created
    /// </Modified>
    public record AuthenticationResponse
	{
        Guid userId;

        string Email;

        string Role;

        string Token;

    }
}

