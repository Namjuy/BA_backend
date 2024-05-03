using BA_GPS.Application.Interfaces;
using BA_GPS.Domain.Entity;
using Microsoft.Extensions.Logging;
using BA_GPS.Common.Authentication;
using BA_GPS.Infrastructure.Repositories;
using BA_GPS.Domain.DTO;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


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
        private readonly GenericRepository<UserPermission> _userPerRepo;
        private readonly IDistributedCache _cache;


        public UserService(ILogger<UserService> logger
            , PasswordHasher passwordHasher
            , GenericRepository<User> repository

            , IDistributedCache cache
            , GenericRepository<UserPermission> userPerRepo)
        {
            _passwordHasher = passwordHasher;
            _logger = logger;
            _repository = repository;
            _cache = cache;
            //_cache1 = cache1;
            _userPerRepo = userPerRepo;
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
                return _repository.GetAll().Where(u => u.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by Id");
                return null;
            }
        }

        /// <summary>
        /// Tạo mới người dùng
        /// </summary>
        /// <param name="newUser">Người dùng mới</param>
        /// <param name="permissionId">Id quyền</param>
        /// <returns>Kết quả true/false</returns>
        public async Task<bool> Create(User newUser, byte permissionId)
        {
            //Mã hoá mật khẩu
            var hashedPassword = _passwordHasher.HashPassword(newUser.PassWordHash);

            newUser.Id = Guid.NewGuid();
            newUser.PassWordHash = hashedPassword;
            newUser.CreateDate = DateTime.UtcNow;
            newUser.LastModifyDate = DateTime.UtcNow;
            newUser.DeletedDate = null;
            newUser.IsDeleted = false;

            var userPermission = new UserPermission
            {
                UserId = newUser.Id,
                PermissionId = permissionId
            };
            try
            {
                if (await _repository.Create(newUser) && await _userPerRepo.Create(userPermission))
                    return true;

                else return false;

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

            try
            {
                //Lấy ra giá trị tìm kiếm đã được lưu trữ trong cache
                var cachedResult = _cache.Get(cacheKey);
                var cacheSearchRequest = _cache.Get("searchFilter");
                //Nếu tồn tại giá trị trong cache trả về danh sách tìm kiếm đã lưu

                if (cachedResult != null && CheckSearchRequestChanged(searchRequest, JsonSerializer.Deserialize<SearchRequest>(cacheSearchRequest)))
                {
                    _logger.LogInformation("Data retrieved from cache.");
                    return JsonSerializer.Deserialize<DataListResponse<User>>(cachedResult);
                }

                var userDataResponse = new DataListResponse<User>();

                //Lọc điều kiện tìm kiếm
                var query = ApplySearchFilters(_repository.GetAll().Where(u=>!u.IsDeleted), searchRequest);

                userDataResponse.DataList = query
                    .Skip(searchRequest.PageSize * (searchRequest.PageIndex - 1))
                    .Take(searchRequest.PageSize)
                    .OrderByDescending(u => u.LastModifyDate)
                    .ToList(); ;
                userDataResponse.TotalPage = (int)Math.Ceiling((double)query.Count() / searchRequest.PageSize);

                var serializedData = JsonSerializer.SerializeToUtf8Bytes(userDataResponse);
                var serializedSearch = JsonSerializer.SerializeToUtf8Bytes(searchRequest);

                //Lưu kêt quả tìm kiếm vào cache
                _cache.Set(cacheKey, serializedData, new DistributedCacheEntryOptions
                {
                    //Thời hạn lưu trữ 10 phút
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                //Lưu điều kiện tìm kiếm vào cache
                _cache.Set("searchFilter", serializedSearch, new DistributedCacheEntryOptions
                {
                    //Thời hạn lưu trữ 10 phút
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return userDataResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for users");
                return new DataListResponse<User>();
            }
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
                return await _repository.GetAll().Where(u => !u.IsDeleted).FirstOrDefaultAsync(item => item.UserName == userName) is null;
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
                return await _repository.GetAll().Where(u => !u.IsDeleted).FirstOrDefaultAsync(item => item.PhoneNumber == phone) is null;
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