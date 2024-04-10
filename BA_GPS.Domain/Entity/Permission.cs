using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   03/04/2024 Created
/// </Modified>
namespace BA_GPS.Domain.Entity
{
    public enum PermissionId : byte
    {
        ADMIN = 1,
        USER = 2,
        GUEST = 3
    }

    [Table("Permission")]
    public class Permission
    {
        public Permission()
        {
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public byte PermissionId { get; set; }

        [NotNull]
        [MaxLength(20, ErrorMessage = "Tối đa 20 ký tự")]
        public PermissionId PermissionName { get; set; }

 

    }
}
