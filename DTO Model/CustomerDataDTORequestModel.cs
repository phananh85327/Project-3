using EC.Model;
using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class CustomerDataDTORequestModel
    {
        public CustomerDataDTORequestModel()
        {
            this.CustomerID = string.Empty;
            this.CustomerName = string.Empty;
            this.Email = string.Empty;
            this.Phone = string.Empty;
            this.Address = string.Empty;
            this.MembershipValidUntil = null;
            this.LateFee = 0;
        }

        public string? CustomerID { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        public DateTime? MembershipValidUntil { get; set; }
        public int LateFee { get; set; }
    }
}
