using BA_GPS.Application.Interfaces;
using BA_GPS.Domain.Entity;
using Microsoft.Extensions.Logging;
using BA_GPS.Common.Authentication;
using BA_GPS.Infrastructure.Repositories;
using BA_GPS.Domain.DTO;
using BA_GPS.Common.Modal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;


/// <summary>
/// 
/// </summary>
/// <Modified>
/// Name    Date    Comments
/// Duypn   05/04/2024 Created
/// </Modified>
namespace BA_GPS.Infrastructure.Services
{
    public class UserService : IGenericService<User>
    {

        private readonly ILogger<UserService> _logger;
        private readonly PasswordHasher _passwordHasher;
        private readonly GenericRepository<User> _repository;
        private readonly IMemoryCache _cache;
        private readonly IDistributedCache _cache1;
        //private readonly string _cacheKey = "productsCacheKey";

        public UserService(ILogger<UserService> logger
            , PasswordHasher passwordHasher
            , GenericRepository<User> repository
            , IMemoryCache cache
            , IDistributedCache cache1)
        {
            _passwordHasher = passwordHasher;
            _logger = logger;
            _repository = repository;
            _cache = cache;
            _cache1 = cache1;
        }

        /// <summary>
        /// Lấy danh sách thông tin người dùng theo phân trang
        /// </summary>
        /// <param name="pageIndex">vị trí trang</param>
        /// <param name="pageSize">kích thước trang</param>
        /// <returns>Danh sách người dùng đã được phân trang</returns>
        public DataListResponse<User> GetPage(int pageIndex, int pageSize)
        {
            var userDataResponse = new DataListResponse<User>();
            try
            {
                userDataResponse.DataList = _repository.GetAll()
                    .Where(u => !u.IsDeleted)
                    .Skip(pageSize * (pageIndex - 1))
                    .Take(pageSize)
                    .OrderByDescending(u => u.LastModifyDate)
                    .ToList();
                userDataResponse.TotalPage = (int)Math.Ceiling((double)_repository.GetAll().Count() / pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy danh sách người dùng");
            }
            return userDataResponse;
        }

        /// <summary>
        /// Lấy người dùng theo Id
        /// </summary>
        /// <param name="id">Mã của người dùng</param>
        /// <returns>Thông tin người dùng</returns>
        public User? GetById(Guid id)
        {
            try
            {
                return _repository.GetById(id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by Id");
                return null;
            }
        }

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="newUser">Thông tin người dùng mới</param>
        /// <returns>Người dùng mới</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<bool> Create(User newUser)
        {
            var hashedPassword = _passwordHasher.HashPassword(newUser.PassWordHash);
            newUser.Id = Guid.NewGuid();
            newUser.PassWordHash = hashedPassword;
            newUser.CreateDate = DateTime.UtcNow;
            newUser.LastModifyDate = DateTime.UtcNow;
            newUser.DeletedDate = null;
            newUser.IsDeleted = false;
            try
            {
               return await _repository.Create(newUser);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái isDeleted cho người dùng
        /// </summary>
        /// <param name="id">Mã người dùng</param>
        /// <returns>Giá trị true/false</returns>
        public bool Delete(Guid id)
        {
            try
            {
                var userToDelete = GetById(id);
                if (userToDelete != null)
                {
                    userToDelete.IsDeleted = true;
                    userToDelete.DeletedDate = DateTime.UtcNow;
                    _repository.Update(userToDelete);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="userToUpdate">Thông tin người dùng được cập nhật</param>
        /// <returns>Giá trị True/False</returns>
        /// <exception cref="ArgumentException">Lỗi không tìm thấy người dùng</exception>
        public bool Update(User userToUpdate)
        {
            try
            {
                userToUpdate.LastModifyDate = DateTime.UtcNow;
                _repository.Update(userToUpdate);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user information");
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm theo thông tin nhập vào và phân trang
        /// </summary>
        /// <param name="searchRequest">Thông tin tìm kiếm</param>
        /// <returns>Danh sách người dùng tìm kiếm</returns>
        public DataListResponse<User> Search(SearchRequest searchRequest)
        {
            var cacheKey = GenerateCacheKey(searchRequest);

            if (_cache.TryGetValue(cacheKey, out SearchRequest cachedSearchRequest))
            {
                if (CheckSearchRequestChanged(searchRequest, cachedSearchRequest))
                {

                    if (_cache.TryGetValue(cacheKey, out DataListResponse<User> cachedResult))
                    {
                        _logger.LogInformation("Dữ liệu được lấy từ cache.");
                        return cachedResult;
                    }
                }
            }

            var userDataResponse = new DataListResponse<User>();

            try
            {
                var query = ApplySearchFilters(_repository.GetAll(), searchRequest);

                userDataResponse.DataList = query.ToList();
                userDataResponse.TotalPage = (int)Math.Ceiling((double)query.Count() / searchRequest.PageSize);

                _cache.Set(cacheKey, userDataResponse, TimeSpan.FromMinutes(10));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tìm kiếm người dùng");
            }
            return userDataResponse;
        }

        /// <summary>
        /// Tạo ra cache key
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        private string GenerateCacheKey(SearchRequest searchRequest)
        {
            return $"UserSearch_{searchRequest.InputValue}_{searchRequest.Type}_{searchRequest.StartDate}_{searchRequest.EndDate}_{searchRequest.PageIndex}_{searchRequest.PageSize}";
        }

        /// <summary>
        /// Lọc các thông tin theo yêu cầu tìm kiếm
        /// </summary>
        /// <param name="query">Câu truy vấn</param>
        /// <param name="searchRequest">Thông tin tìm kiếm</param>
        /// <returns>Câu truy vấn</returns>
        private IQueryable<User> ApplySearchFilters(IQueryable<User> query, SearchRequest searchRequest)
        {
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

            if (searchRequest.Gender != null)
            {
                query = query.Where(u => u.IsMale == searchRequest.Gender);
            }

            return query;
        }


        /// <summary>
        /// Kiểm tra tên đăng nhập của người dùng tồn tại
        /// </summary>
        /// <param name="userName">Tên đăng nhập</param>
        /// <returns>Kết quả True/ False</returns>
        public async Task<bool> CheckUserNameExist(string userName)
        {
            try
            {
                return await _repository.GetAll().FirstOrDefaultAsync(item => item.UserName == userName) is null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy thông tin người dùng");
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra số điện thoại của người dùng tồn tại
        /// </summary>
        /// <param name="phone">Số điện thoại người dùng</param>
        /// <returns></returns>
        public async Task<bool> CheckPhoneExist(string phone)
        {
            try
            {
                return await _repository.GetAll().FirstOrDefaultAsync(item => item.PhoneNumber == phone) is null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kiểm tra thông tin người dùng");
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra dữ liệu người dùng hợp lệ hay không
        /// </summary>
        /// <param name="user">Thông tin người dùng</param>
        /// <returns>Kết quả True/False</returns>
        public bool CheckUserValid(User user)
        {
            return !(
                     string.IsNullOrEmpty(user.FullName) ||
                     string.IsNullOrEmpty(user.UserName) ||
                     string.IsNullOrEmpty(user.PassWordHash) ||
                     user.DateOfBirth == null ||
                     user.DateOfBirth > DateTime.UtcNow.AddYears(-18));
        }

        /// <summary>
        /// Kiểm tra thông tin tìm kiếm có hợp lệ hay không
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns>Kết quả true/false</returns>
        public bool CheckSearchValid(SearchRequest searchRequest)
        {
            if (string.IsNullOrEmpty(searchRequest.Type) || searchRequest.PageIndex <= 0 || searchRequest.PageSize <= 0)
            {
                return false;
            }

            return true;
        }


        /// <summary>
        /// Kiểm tra số điện thoại nhập vào đã được đăng ký hay không
        /// </summary>
        /// <param name="phone">số điện thoại</param>
        /// <returns>Kết quả true/false</returns>
        public bool CheckPhoneValid(string phone)
        {
            return !(string.IsNullOrEmpty(phone) || phone.Length != 10);
        }

        /// <summary>
        /// Kiểm tra request tìm kiếm mới có trùng lặp với request cũ đã được lưu ở cache hay không
        /// </summary>
        /// <param name="searchRequest">Thông tin tìm kiếm mới</param>
        /// <param name="cachedSearchRequest">Thông tin tìm kiếm đã được lưu trong cache</param>
        /// <returns>Trả về kết quả true/false</returns>
        private bool CheckSearchRequestChanged(SearchRequest searchRequest, SearchRequest cachedSearchRequest)
        {
            return searchRequest.InputValue == cachedSearchRequest.InputValue &&
                   searchRequest.Type == cachedSearchRequest.Type &&
                   searchRequest.StartDate == cachedSearchRequest.StartDate &&
                   searchRequest.EndDate == cachedSearchRequest.EndDate &&
                   searchRequest.Gender == cachedSearchRequest.Gender &&
                   searchRequest.PageIndex == cachedSearchRequest.PageIndex &&
                   searchRequest.PageSize == cachedSearchRequest.PageSize;
        }


     


    }
}