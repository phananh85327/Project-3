using EC.DTO_Model;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace EC.Model
{
    public class UserDataModel
    {
        public UserDataModel()
        {
            this.UserID = string.Empty;
            this.UserName = string.Empty;
            this.Email = string.Empty;
            this.Phone = string.Empty;
            this.Salt = string.Empty;
            this.Pass = string.Empty;
            this.RefreshToken = string.Empty;
            this.UserRole = string.Empty;
            this.IsLocked = false;
        }

        public UserDataModel(UserDataDTORequestModel userData)
        {
            this.UserID = Guid.NewGuid().ToString();
            this.UserName = userData.UserName;
            this.Email = userData.Email;
            this.Phone = userData.Phone;
            this.Salt = GenerateSalt();
            this.Pass = HashPassword(userData.Pass ?? string.Empty, this.Salt);
            this.RefreshToken = string.Empty;
            this.UserRole = Constants.DEFAULT_USER_ROLE;
            this.IsLocked = false;
        }

        public static string GenerateSalt(int saltSize = 16)
        {
            var saltBytes = new byte[saltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            var saltBytes = Convert.FromBase64String(salt);
            using (var sha256 = SHA256.Create())
            {
                var passwordBytes = Encoding.UTF8.GetBytes(password);
                var saltedPassword = new byte[passwordBytes.Length + saltBytes.Length];
                Buffer.BlockCopy(passwordBytes, 0, saltedPassword, 0, passwordBytes.Length);
                Buffer.BlockCopy(saltBytes, 0, saltedPassword, passwordBytes.Length, saltBytes.Length);

                var hashedBytes = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Salt { get; set; }
        public string Pass { get; set; }
        public string RefreshToken { get; set; }
        public string UserRole { get; set; }
        public bool IsLocked { get; set; }
    }
}
