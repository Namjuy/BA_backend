using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;


/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   03/04/2024 Created
/// </Modified>
namespace BA_GPS.Domain.Entity
{
  
    [Table("Permission")]
    public class Permission
    {
        public Permission()
        {
        }

        [Key]
        public byte PermissionId { get; set; }

        [NotNull]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public string PermissionName { get; set; }

 

    }
}
