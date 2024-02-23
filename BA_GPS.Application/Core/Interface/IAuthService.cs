using System;
using BA_GPS.Application.Model;
using BA_GPS.Domain.Entity;

namespace BA_GPS.Application.Core.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/01/2024 Created
    /// </Modified>
	public interface IAuthService
	{
        Task<LoginRequest> Login(string username, string password);
    }
    
}



