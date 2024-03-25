using System;
namespace BA_GPS.Domain.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date        Comments
    /// Duypn   21/03/2024  Created
    /// </Modified>
    public class DataListResponse<T>
	{
		public DataListResponse()
		{
		}

		public List<T> DataList { get; set; }

		public int TotalPage { get; set; }


	}
}

