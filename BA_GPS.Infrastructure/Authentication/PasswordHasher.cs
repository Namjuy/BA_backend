using System;
using System.Security.Cryptography;
using BA_GPS.Application.Core.Interface;

namespace BA_GPS.Infrastructure.Authentication
{
    /// <summary>
    /// 
    /// </summary>
    /// <Modified>
    /// Name    Date        Comments
    /// Duypn   14/01/2024  Created
    /// </Modified>
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 128 / 8;
        private const int KeySize = 256 / 8;
        private const int Iteration = 10000;
        private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
        private const char Delimiter = ';';

        /// <summary>
        /// Mã hoá mật khẩu 1 chiều
        /// </summary>
        /// <param name="password"></param>
        /// <returns>Chuỗi ký tự đã được mã hoá</returns>
        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iteration, _hashAlgorithmName, KeySize);
            return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        /// <summary>
        /// Xác nhận token của mật khẩu đã được mã hoá so với mật khẩu mình đã nhập có đúng hay không
        /// </summary>
        /// <param name="passwordHash"></param>
        /// <param name="inputPassword"></param>
        /// <returns> Kết quả xác nhận password đúng hay không </returns>
        public bool Verify(string passwordHash, string inputPassword)
        {
            var elements = passwordHash.Split(Delimiter);
            var salt = Convert.FromBase64String(elements[0]);
            var hash = Convert.FromBase64String(elements[1]);

            var hashInput = Rfc2898DeriveBytes.Pbkdf2(inputPassword, salt, Iteration, _hashAlgorithmName, KeySize);
            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}
