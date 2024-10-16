using Azure.Core;
using EC.Model;

namespace EC.DTO_Model
{
    public class UserDataDTOResponseModel
    {
        public UserDataDTOResponseModel(string error = "")
        {
            this.UserID = string.Empty;
            this.UserName = string.Empty;
            this.Email = string.Empty;
            this.Phone = string.Empty;
            this.UserRole = string.Empty;
            this.AccessToken = string.Empty;
            this.RefreshToken = string.Empty;
            this.Error = error;
            this.IsLocked = false;
        }

        public UserDataDTOResponseModel(UserDataModel user, string accessToken = "", string refreshToken = "", string error = "")
        {
            this.UserID = user.UserID;
            this.UserName = user.UserName;
            this.Email = user.Email;
            this.Phone = user.Phone;
            this.UserRole = user.UserRole;
            this.AccessToken = accessToken;
            this.RefreshToken = refreshToken;
            this.Error = error;
            this.IsLocked = user.IsLocked;
        }

        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserRole { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Error { get; set; }
        public bool IsLocked { get; set; }
    }
}
