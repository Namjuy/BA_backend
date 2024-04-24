using System;

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   19/04/2024 Created
/// </Modified>
namespace BA_GPS.Common
{
	public class CommonService
	{
		public CommonService()
		{
			
		}

		/// <summary>
		/// Kiểm tra thông tin trang có hợp lệ hay không
		/// </summary>
		/// <param name="pageIndex">Chỉ số trang</param>
		/// <param name="pageSize">Kích thước trang</param>
		/// <returns>Kết quả true/false</returns>
		public bool CheckPaginatedItemValid(int pageIndex, int pageSize)
		{
			if(pageIndex ==0 || pageSize == 0)
			{
				return false;
			}
			return true;
		}

    }
}

