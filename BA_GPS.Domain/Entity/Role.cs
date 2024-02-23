
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BA_GPS.Domain.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/01/2024 Created
    /// </Modified>
    [Table("Roles")]
	public class Role
	{

	[Key]
	public int RoleId { get; set; }

	[NotNull]
    public string RoleName { get; set; }

	}
}

