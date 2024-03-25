using System;
namespace BA_GPS.Domain.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date        Comments
    /// Duypn   20/03/2024  Created
    /// </Modified>
    public class SearchRequest
	{
		public SearchRequest() { }
		

		public string? InputValue { get; set; }

		public string? Type { get; set; }

		public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public byte? Gender { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

    }
	
}

