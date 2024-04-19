using System;
using BA_GPS.Domain.DTO;
using BA_GPS.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   05/04/2024 Created
/// </Modified>
namespace BA_GPS.Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly GenericDbContext _dbContext;

        public UserRepository(GenericDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Tìm kiếm người dùng trên database
        /// </summary>
        /// <param name="searchRequest">Thông tin tìm kiếm</param>
        /// <returns>Người dùng phù hợp</returns>
        public async Task<DataListResponse<User>> Search(SearchRequest searchRequest)
        {
            var dataResponse = new DataListResponse<User>();
            var query = _dbContext.Users.Where(u => !u.IsDeleted);

            switch (searchRequest.Type.ToLower())
            {
                case "username":
                    query = query.Where(u => u.UserName.Contains(searchRequest.InputValue));
                    break;

                case "fullname":
                    query = query.Where(u => u.FullName.Contains(searchRequest.InputValue));
                    break;

                case "phonenumber":
                    query = query.Where(u => u.PhoneNumber.Contains(searchRequest.InputValue));
                    break;

                case "email":
                    query = query.Where(u => u.Email.Contains(searchRequest.InputValue));
                    break;

                default:
                    break;
            }

            if (searchRequest.StartDate != null)
            {
                query = query.Where(u => u.LastModifyDate >= searchRequest.StartDate);
            }

            if (searchRequest.EndDate != null)
            {
                query = query.Where(u => u.LastModifyDate <= searchRequest.EndDate);
            }

            if(searchRequest.Gender != null)
            {
                query = query.Where(u => u.IsMale == searchRequest.Gender);
            }

            dataResponse.TotalPage = await query.CountAsync();

            dataResponse.DataList = await query.OrderByDescending(u => u.LastModifyDate)
                .Skip(searchRequest.PageSize * (searchRequest.PageIndex - 1))
                .Take(searchRequest.PageSize)
                .ToListAsync();

            return dataResponse;
        }

        /// <summary>
        /// Kiểm tra trong database xem tên đăng nhập của người dùng có tồn tại hay không
        /// </summary>
        /// <param name="userName">Tên đăng nhập</param>
        /// <returns>Kết quả True/ False</returns>
        public async Task<bool> CheckUserNameExist(string userName)
        {
            return await _dbContext.Users.Where(u => !u.IsDeleted).FirstOrDefaultAsync(item => item.UserName == userName) is null;
        }

        public async Task<bool> CheckPhoneExist(string phone)
        {
            return await _dbContext.Users.Where(u => !u.IsDeleted).FirstOrDefaultAsync(item => item.PhoneNumber == phone) is null;
        }

        /// <summary>
        /// Tìm kiếm người dùng trong database theo tên đăng nhập
        /// </summary>
        /// <param name="username">Tên đăng nhập</param>
        /// <returns>Người dùng cần tìm kiếm</returns>
        public async Task<User> GetByUserName(string username)
        {
            return await _dbContext.Users.Where(u => !u.IsDeleted).FirstOrDefaultAsync(user => user.UserName == username);
        }
    }
}

