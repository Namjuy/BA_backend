using BA_GPS.Application.Interfaces;
using BA_GPS.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BA_GPS.Common.Authentication;
namespace BA_GPS.Infrastructure.Services
{

    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date    Comments
    /// Duypn   11/03/2024 Created
    /// </Modified>
    public class UserServices : IGenericService<User>
    {
        //private readonly PaginationRequest _paginationRequest;
        private readonly UserDbContext _dbContext;
        private readonly ILogger<UserServices> _logger;
        private readonly PasswordHasher _passwordHasher;

        public UserServices(UserDbContext dbContext, ILogger<UserServices> logger, PasswordHasher passwordHasher)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        /// <summary>
        /// Tạo người dùng mới
        /// </summary>
        /// <param name="newUser">Thông tin người dùng mới</param>
        /// <returns>Người dùng mới</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<User> Create(User newUser)
        {
            //Mã hoá mật khẩu
            var hashedPassword = _passwordHasher.HashPassword(newUser.PassWordHash);

            //Kiểm tra người dùng tồn tại
            if (newUser == null)
            {
                throw new ArgumentException(nameof(newUser), "User is null");
            }

            //Tạo thông tin người dùng
            newUser.UserId = Guid.NewGuid();
            newUser.PassWordHash = hashedPassword;
            newUser.CreateDate = DateTime.UtcNow;
            newUser.LastModifyDate = DateTime.UtcNow;
            newUser.DeletedDate = null;
            newUser.IsDeleted = false;
            try
            {

                _dbContext.UserInfor.Add(newUser);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
            }

            return newUser;
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
                var user = await _dbContext.UserInfor.FirstOrDefaultAsync(item => item.UserId == id);

                if (user == null)
                    return false;

                user.IsDeleted = true;
                user.DeletedDate = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xoá người dùng");
              
            }
            return true;
        }

        /// <summary>
        /// Lấy danh sách thông tin người dùng theo phân trang
        /// </summary>
        /// <param name="pageIndex">vị trí trang</param>
        /// <param name="pageSize">kích thước trang</param>
        /// <returns>Danh sách người dùng đã được phân trang</returns>
        public async Task<DataListResponse<User>> GetAll(int pageIndex, int pageSize)
        {
            //itemsOnPage is a list of user
            var userDataResponse = new DataListResponse<User>();
            try
            {
                userDataResponse.TotalPage = (int)Math.Ceiling((double)await _dbContext.UserInfor.Where(u => !u.IsDeleted).CountAsync() / pageSize);
                userDataResponse.DataList = await _dbContext.UserInfor.Where(u => !u.IsDeleted).OrderByDescending(u=>u.LastModifyDate).Skip(pageSize * (pageIndex - 1)).Take(pageSize).ToListAsync();
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
        /// <returns></returns>
        public async Task<User> GetById(Guid id)
        {
            var user = new User();

            try
            {
                 user = await _dbContext.UserInfor.FirstOrDefaultAsync(item => item.UserId == id) ?? throw new Exception("User not found");
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lấy thông tin người dùng lỗi");

            }
            return user;
        }

        /// <summary>
        /// Lấy ra người dùng theo thông tin đăng nhập
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<User> GetUserByLoginInput(string username)
        {
            var user = new User();

            try
            {
                user = await _dbContext.UserInfor.FirstOrDefaultAsync(item => item.UserName == username) ?? throw new Exception("User not found");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lấy thông tin người dùng lỗi");

            }
            return user;
        }

        /// <summary>
        /// Cập nhật người dùng
        /// </summary>
        /// <param name="userToUpdate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<User> Update(User userToUpdate)
        {
            var user = new User();
            try
            {
                //Cập nhật thông tin người dùng
                 user = await _dbContext.UserInfor.FirstOrDefaultAsync(item => item.UserId == userToUpdate.UserId) ?? throw new ArgumentException("User not found");
                user.FullName = userToUpdate.FullName;
                user.IsMale = userToUpdate.IsMale;
                user.DateOfBirth = userToUpdate.DateOfBirth;
                user.Email = userToUpdate.Email;
                user.Address = userToUpdate.Address;
                user.LastModifyDate = DateTime.UtcNow;
                user.CompanyId = userToUpdate.CompanyId;
                user.PhoneNumber = user.PhoneNumber;
               
                await _dbContext.SaveChangesAsync();

                return user; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cập nhật thông tin người dùng lỗi");
              
            }

            return user;
        }

        /// <summary>
        /// Tìm kiếm theo thông tin nhập vào và phân trang
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        public async Task<DataListResponse<User>> Search(SearchRequest searchRequest)
        {
            var userDataResponse = new DataListResponse<User>();
           
            try
            {
                IQueryable<User> query = _dbContext.UserInfor.Where(u => !u.IsDeleted);

                //Lọc dữ liệu theo giá trị nhập vào

                if (!string.IsNullOrEmpty(searchRequest.Type) && !string.IsNullOrEmpty(searchRequest.InputValue))
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
                }

                if(searchRequest.StartDate != null || searchRequest.EndDate != null)
                {
                    query = query.Where(u =>
                        (searchRequest.StartDate == u.LastModifyDate || u.LastModifyDate >= searchRequest.StartDate) ||
                        (searchRequest.EndDate == u.LastModifyDate || u.LastModifyDate <= searchRequest.EndDate)
                    );
                }

                if (searchRequest.Gender.HasValue)
                {
                    query = query.Where(u => u.IsMale == searchRequest.Gender.Value);
                }


                //Tính tổng số trang
                userDataResponse.TotalPage = (int)Math.Ceiling((double)await query.CountAsync() / searchRequest.PageSize);

                //Lấy ra danh sách tìm kiếm đã được phân trang
                userDataResponse.DataList = await query.OrderByDescending(u => u.LastModifyDate)
                    .Skip(searchRequest.PageSize * (searchRequest.PageIndex - 1))
                    .Take(searchRequest.PageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user list");
            }
            return userDataResponse;
        }


        /// <summary>
        /// Kiểm tra đối tượng tồn tại hay không dựa trên thông tin nhập vào
        /// </summary>
        /// <param name="value">Thông tin cần kiểm tra </param>
        /// <returns>kết quả true/false</returns>
        public async Task<bool> CheckExist(string value)
        {
            var check = true;
            var user = new User();
            try
            {
                user = await _dbContext.UserInfor.Where(u => !u.IsDeleted).FirstOrDefaultAsync(item => item.UserName == value);
                if (user is null)
                {

                    return check = false;
                   
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi kiểm tra tồn tại");
            }
            return check;
        }

      
    }
}
