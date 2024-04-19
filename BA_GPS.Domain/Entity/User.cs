using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using BA_GPS.Common;

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   11/01/2024 Created
/// </Modified>
namespace BA_GPS.Domain.Entity
{
    
    [Table("User")]
    public class User : BaseEntity
    {
        public User() : base()
        {
        }

        public User(Guid id, DateTime createDate, DateTime lastModifyDate, bool isDeleted, DateTime? deletedDate)
           : base(id, createDate, lastModifyDate, isDeleted, deletedDate)
        {
        }

        [NotNull]
        [MaxLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Chỉ được phép viết liền không dấu")]
        public string UserName { get; set; }

        [NotNull]
        public string PassWordHash { get; set; }

        [NotNull]
        [MaxLength(200, ErrorMessage = "Tên người dùng không được dài quá 200 ký tự")]
        public string FullName { get; set; }

        [NotNull]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [NotNull]
        public byte? IsMale { get; set; }

        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại không chính xác")]
        public string PhoneNumber { get; set; }

        [NotNull]
        public byte PermissionId { get;set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@gmail\.com$", ErrorMessage = "Email phải đúng định dạng.")]
        public string Email { get; set; }

        public string Address { get; set; }

        public int CompanyId { get; set; }


    }

}

