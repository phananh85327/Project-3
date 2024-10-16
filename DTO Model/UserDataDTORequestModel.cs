using EC.Model;
using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class UserDataDTORequestModel
    {
        public UserDataDTORequestModel()
        {
            this.UserID = string.Empty;
            this.UserName = string.Empty;
            this.Email = string.Empty;
            this.Phone = string.Empty;
            this.OldPass = string.Empty;
            this.Pass = string.Empty;
            this.IsLocked = null;
        }

        public string? UserID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public string? OldPass { get; set; }
        public string? Pass { get; set; }
        public bool? IsLocked { get; set; }
    }
}
