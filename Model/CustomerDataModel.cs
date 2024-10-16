using EC.DTO_Model;
using System.ComponentModel.DataAnnotations;

namespace EC.Model
{
    public class CustomerDataModel
    {
        public CustomerDataModel() 
        {
            this.CustomerID = string.Empty;
            this.CustomerName = string.Empty;
            this.Email = string.Empty;
            this.Phone = string.Empty;
            this.Address = string.Empty;
            this.Items = string.Empty;
            this.MembershipValidUntil = DateTime.Now;
            this.LateFee = 0;
        }

        public CustomerDataModel(CustomerDataDTORequestModel customerData)
        {
            this.CustomerID = Guid.NewGuid().ToString();
            this.CustomerName = customerData.CustomerName;
            this.Email = customerData.Email;
            this.Phone = customerData.Phone;
            this.Address = customerData.Address;
            this.Items = string.Empty;
            this.MembershipValidUntil = DateTime.Now.AddDays(Constants.MEMBERSHIP_VALID_DAYS);
            this.LateFee = 0;
        }

        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Items { get; set; }
        public DateTime MembershipValidUntil { get; set; }
        public int LateFee { get; set; }
    }
}
