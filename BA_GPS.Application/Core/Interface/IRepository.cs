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
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Lấy ra thông tin đối tượng theo id
        /// </summary>
        /// <param name="id">Mã của đối tượng</param>
        /// <returns>1 Đối tượng.</returns>
        //IEnumerable<TEntity> GetById(Guid id);

      /// <summary>
      /// Xoá đối tượng
      /// </summary>
      /// <param name="entity">Thông tin đối tượng</param>
        void Delete(TEntity entity);

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
        void Update(TEntity entity);


    }
}

