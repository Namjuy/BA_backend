using System;


/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   09/04/2024 Created
/// </Modified>
namespace BA_GPS.Domain.Entity
{
	public class UserPermission
	{
		public UserPermission()
		{
		}

		public User User { get; set; }
		public Guid UserId { get; set; }

		public Permission Permission { get; set; }
		public byte PermissionId { get; set; }

	}
}

