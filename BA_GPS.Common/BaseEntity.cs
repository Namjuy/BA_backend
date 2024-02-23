using System;
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
        public BaseEntity(string? creatorUserId, string? lastModifyUserId, DateTime createDate, DateTime lastModifyDate, bool isDeleted, DateTime? deletedDate)
        {
            CreatorUserId = creatorUserId;
            LastModifyUserId = lastModifyUserId;
            CreateDate = createDate;
            LastModifyDate = lastModifyDate;
            IsDeleted = isDeleted;
            DeletedDate = deletedDate;
        }

        public string? CreatorUserId { get; set; }

		public string? LastModifyUserId { get; set; }

		public DateTime CreateDate { get; set; }

		public DateTime LastModifyDate { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? DeletedDate { get; set; }
	}
}

