using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BA_GPS.Common
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/01/2024 Created
    /// </Modified>
    public class BaseEntity
	{
        public BaseEntity() { }

        public BaseEntity(Guid id, DateTime createDate, DateTime lastModifyDate, bool isDeleted, DateTime? deletedDate)
        {
            Id = id;
            CreateDate = createDate;
            LastModifyDate = lastModifyDate;
            IsDeleted = isDeleted;
            DeletedDate = deletedDate;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [NotNull]
        public Guid Id { get; set; }

		public DateTime CreateDate { get; set; }

		public DateTime LastModifyDate { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? DeletedDate { get; set; }
	}
}

