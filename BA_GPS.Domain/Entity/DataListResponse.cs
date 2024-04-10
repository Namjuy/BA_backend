using System;

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date        Comments
/// Duypn   21/03/2024  Created
/// </Modified>
namespace BA_GPS.Domain.DTO
{
    public class DataListResponse<T>
	{
		public DataListResponse()
		{
		}

		public List<T> DataList { get; set; }

		public int TotalPage { get; set; }


	}
}

