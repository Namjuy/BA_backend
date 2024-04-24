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
        /// Lấy tất cả danh sách đối tượng trong database
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetAll() => _dbContext.Set<TEntity>().AsNoTracking().AsQueryable();

        /// <summary>
        /// Lấy tất cả danh sách theo điều kiện có sẵn
        /// </summary>
        /// <param name="expression">Điều kiện</param>
        /// <returns>Kết quả lệnh truy vấn</returns>
        public IQueryable<TEntity> GetAll(Func<TEntity, bool> expression) => _dbContext.Set<TEntity>().AsNoTracking().Where(expression).AsQueryable();

        /// <summary>
        /// Tìm kiếm theo Id
        /// </summary>
        /// <param name="id">Id đối tượng</param>
        /// <returns>Đối tượng có id</returns>
        public IEnumerable<TEntity> GetById(Guid id) => _dbContext.Set<TEntity>().AsNoTracking().AsQueryable().Where(u => u.Id == id).AsEnumerable();

        /// <summary>
        /// Tạo đối tượng mới
        /// </summary>
        /// <param name="entity">Đối tượng mới</param>
        /// <returns>Kết quả true/false</returns>
        public async Task<bool> Create(TEntity entity)
        {
            await _dbContext.Set<TEntity>().AddAsync(entity);
           return await _dbContext.SaveChangesAsync()>0;
        }

        /// <summary>
        /// Cập nhật đối tượng
        /// </summary>
        /// <param name="entity">Thông tin cập nhật</param>
        public void Update(TEntity entity)
        {
            _dbContext.Set<TEntity>().Update(entity);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Gắn 'cờ' xoá đối tượng
        /// </summary>
        /// <param name="entity">Đối tượng cần xoá</param>
        public void Delete(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            _dbContext.SaveChanges();
        }



    }
}
