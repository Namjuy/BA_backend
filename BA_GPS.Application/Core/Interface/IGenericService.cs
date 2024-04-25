using System;
using BA_GPS.Domain.DTO;
using BA_GPS.Domain.Entity;

namespace BA_GPS.Application.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/03/2024 Created
    /// </Modified>
	public interface IGenericService<TEntity>
    {
        /// <summary>
        /// Lấy dang sách thông tin đối tượng chung theo phân trang
        /// </summary>
        /// <returns>1 tập hợp các đối tượng</returns>
        DataListResponse<User> GetPage(int pageIndex, int pageSize);

        /// <summary>
        /// Lấy ra thông tin đối tượng theo id
        /// </summary>
        /// <param name="id">Mã của đối tượng</param>
        /// <returns>1 Đối tượng.</returns>
        User? GetById(Guid id);

        /// <summary>
        /// Tạo đối tượng mới
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>Tạo đối tượng.</returns>
        Task<bool> Create(TEntity entity, byte permissionId);

        /// <summary>
        /// Cập nhật đối tượng
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>Thông tin đối tượng được cập nhật.</returns>
        bool Update(TEntity entity);

        /// <summary>
        /// Cập nhật trạng thái xoá cho đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng cần xoá.</param>
        /// <returns>True nếu xoá thành công , false nếu xoá thất bại</returns>
        bool Delete(Guid id);

        /// <summary>
        /// Cập nhật trạng thái xoá cho đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng cần xoá.</param>
        /// <returns>True nếu xoá thành công , false nếu xoá thất bại</returns>
        DataListResponse<User> Search(SearchRequest searchRequest);


    }
}

