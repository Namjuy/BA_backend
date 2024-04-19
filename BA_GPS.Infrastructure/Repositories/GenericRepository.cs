using BA_GPS.Application.Core.Interface;
using BA_GPS.Common;
using BA_GPS.Domain.DTO;
using BA_GPS.Domain.Entity;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   03/04/2024 Created
/// </Modified>
namespace BA_GPS.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly GenericDbContext _dbContext;

        public GenericRepository(GenericDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Lấy danh sách đối tượng trên database
        /// </summary>
        /// <param name="pageIndex">Thứ tự trang</param>
        /// <param name="pageSize">Số lượng người dùng hiển thị trong 1 trang</param>
        /// <returns>Danh sách dữ liệu người dùng</returns>
        public async Task<DataListResponse<TEntity>> Get(int pageIndex, int pageSize)
        {
            var dataResponse = new DataListResponse<TEntity>();
            var query = _dbContext.Set<TEntity>()
                            .Where(u => !u.IsDeleted);
            dataResponse.TotalPage = await query.CountAsync();
            dataResponse.DataList = await query.Skip(pageSize * (pageIndex - 1))
                                              .Take(pageSize)
                                              .OrderByDescending(u => u.LastModifyDate)
                                              .ToListAsync();
            return dataResponse;
        }

        /// <summary>
        /// Tìm kiếm đối tượng theo Id
        /// </summary>
        /// <param name="id">Id của đối tượng</param>
        /// <returns>Đối tượng cần tìm kiếm</returns>
        public async Task<TEntity> GetById(Guid id)
        {
            return await _dbContext.Set<TEntity>().FirstOrDefaultAsync(item => item.Id == id);
        }

        /// <summary>
        /// Tạo đối tượng mới
        /// </summary>
        /// <param name="entity">Đối tượng mới</param>
        /// <returns>Kết quả True/False</returns>
        public async Task<bool> Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        /// <summary>
        /// Cập nhật người dùng trên database
        /// </summary>
        /// <param name="entity">Đối tượng cập nhật</param>
        /// <returns>Kết quả True/ false</returns>
        public async Task<bool> Update(TEntity entity)
        {
            var existingEntity = await _dbContext.Set<TEntity>().FindAsync(entity.Id);

            if (existingEntity != null)
            {
                // Update the properties of the existing entity with the values of the incoming entity
                _dbContext.Entry(existingEntity).CurrentValues.SetValues(entity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false; 
            }
        }

        /// <summary>
        /// Cập nhật trang thại xoá cho đối tượng
        /// </summary>
        /// <param name="id">Id của đối tượng</param>
        /// <returnsKết quả True/False</returns>
        public async Task<bool> Delete(Guid id)
        {
            var user = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(item => item.Id == id);

            if (user == null)
                return false;

            user.IsDeleted = true;
            user.DeletedDate = DateTime.UtcNow;
            return await _dbContext.SaveChangesAsync() > 0;
        }


    }
}
