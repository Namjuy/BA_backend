using System;
using BA_GPS.Domain.DTO;

namespace BA_GPS.Application.Core.Interface
{
    // 4-4
	public interface IRepository<TEntity>
	{
        /// <summary>
        /// Lấy dang sách thông tin đối tượng chung theo phân trang
        /// </summary>
        /// <returns>1 tập hợp các đối tượng</returns>
        Task<DataListResponse<TEntity>> Get(int pageIndex, int pageSize);

        /// <summary>
        /// Lấy ra thông tin đối tượng theo id
        /// </summary>
        /// <param name="id">Mã của đối tượng</param>
        /// <returns>1 Đối tượng.</returns>
        Task<TEntity> GetById(Guid id);

        /// <summary>
        /// Cập nhật trạng thái xoá cho đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng cần xoá.</param>
        /// <returns>True nếu xoá thành công , false nếu xoá thất bại</returns>
        Task<bool> Delete(Guid id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Create(TEntity entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> Update(TEntity entity);


    }
}

