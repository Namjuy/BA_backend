using System;
namespace BA_GPS.Application.Core.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   14/01/2024 Created
    /// </Modified>
    public interface IPasswordHasher
	{
		string Hash(string password);

		bool Verify(string hashPassword, string password);
	}
}

