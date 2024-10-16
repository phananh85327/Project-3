using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class LoginDataDTORequestModel
    {
        public LoginDataDTORequestModel()
        {
            this.Email = string.Empty;
            this.Pass = string.Empty;
        }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Pass { get; set; }
    }
}
