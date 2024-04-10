using BA_GPS.Application.Interfaces;
using BA_GPS.Domain.Entity;
using Microsoft.Extensions.Logging;
using BA_GPS.Common.Authentication;
using BA_GPS.Infrastructure.Repositories;
using BA_GPS.Domain.DTO;
using BA_GPS.Common.Modal;
using Microsoft.Extensions.Caching.Memory;


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
        private readonly UserRepository _userRepository;
        private readonly IMemoryCache _cache;
        private readonly string cacheKey = "productsCacheKey";
    
        public UserService(ILogger<UserService> logger, PasswordHasher passwordHasher, GenericRepository<User> repository, UserRepository userRepository, IMemoryCache cache)
        {
            _passwordHasher = passwordHasher;
            _logger = logger;
            _repository = repository;
            _userRepository = userRepository;
            _cache = cache;
        }

        /// <summary>
        /// Lấy danh sách thông tin người dùng theo phân trang
        /// </summary>
        /// <param name="pageIndex">vị trí trang</param>
        /// <param name="pageSize">kích thước trang</param>
        /// <returns>Danh sách người dùng đã được phân trang</returns>
        public async Task<DataListResponse<User>> Get(int pageIndex, int pageSize)
        {
            var userDataResponse = new DataListResponse<User>();
            try
            {
                userDataResponse = await _repository.Get(pageIndex, pageSize);
                userDataResponse.TotalPage = (int)Math.Ceiling((double)userDataResponse.TotalPage / pageSize);
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
        public async Task<User> GetById(Guid id)
        {
            try
            {
                return await _repository.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lấy thông tin người dùng lỗi");
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
            //Mã hoá mật khẩu
            var hashedPassword = _passwordHasher.HashPassword(newUser.PassWordHash);

            //Tạo thông tin người dùng
            newUser.Id = Guid.NewGuid();
            newUser.PassWordHash = hashedPassword;
            newUser.CreateDate = DateTime.UtcNow;
            newUser.LastModifyDate = DateTime.UtcNow;
            newUser.DeletedDate = null;
            newUser.IsDeleted = false;
            try
            {
                await _repository.Create(newUser);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tạo người dùng");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái isDeleted cho người dùng
        /// </summary>
        /// <param name="id">Mã người dùng</param>
        /// <returns>Giá trị true/false</returns>
        public async Task<bool> Delete(Guid id)
        {
            try
            {
                return await _repository.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xoá người dùng");
                return false;
            }
        }

        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="userToUpdate">Thông tin người dùng được cập nhật</param>
        /// <returns>Giá trị True/False</returns>
        /// <exception cref="ArgumentException">Lỗi không tìm thấy người dùng</exception>
        public async Task<bool> Update(User userToUpdate)
        {
            try
            {
                userToUpdate.LastModifyDate = DateTime.UtcNow;
                return await _repository.Update(userToUpdate);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cập nhật thông tin người dùng lỗi");
                return false;
            }
        }

        /// <summary>
        /// Tìm kiếm theo thông tin nhập vào và phân trang
        /// </summary>
        /// <param name="searchRequest">Thông tin tìm kiếm</param>
        /// <returns>Danh sách người dùng tìm kiếm</returns>
        public async Task<DataListResponse<User>> Search(SearchRequest searchRequest)
        {
            var cacheKey = $"UserSearch_{searchRequest.PageIndex}_{searchRequest.PageSize}";

            if (_cache.TryGetValue(cacheKey, out DataListResponse<User> cachedResult))
            {
                _logger.LogInformation("Dữ liệu được lấy từ cache.");
                return cachedResult;
            }

            var userDataResponse = new DataListResponse<User>();

            try
            {
                userDataResponse = await _userRepository.Search(searchRequest);
                userDataResponse.TotalPage = (int)Math.Ceiling((double)userDataResponse.TotalPage / searchRequest.PageSize);

                _cache.Set(cacheKey, userDataResponse, TimeSpan.FromHours(24));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi tìm kiếm người dùng");
            }
            return userDataResponse;
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
                return await _userRepository.CheckUserNameExist(userName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy thông tin người dùng");
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

    }
}