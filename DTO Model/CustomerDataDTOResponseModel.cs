using EC.Model;
using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class CustomerDataDTOResponseModel
    {
        public CustomerDataDTOResponseModel(string error)
        {
            this.CustomerID = string.Empty;
            this.CustomerName = string.Empty;
            this.Email = string.Empty;
            this.Phone = string.Empty;
            this.Address = string.Empty;
            this.FeePaid = false;
            this.Items = string.Empty;
            this.MembershipValidUntil = string.Empty;
            this.LateFee = 0;
            this.Error = error;
        }

        public CustomerDataDTOResponseModel(CustomerDataModel customer)
        {
            this.CustomerID = customer.CustomerID;
            this.CustomerName = customer.CustomerName;
            this.Email = customer.Email;
            this.Phone = customer.Phone;
            this.Address = customer.Address;
            this.FeePaid = customer.MembershipValidUntil > DateTime.Now;
            this.Items = customer.Items;
            this.MembershipValidUntil = customer.MembershipValidUntil.ToString("yyyy-MM-ddTHH:mm");
            this.LateFee = customer.LateFee;
            this.Error = string.Empty;
        }

        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public bool FeePaid { get; set; }
        public string Items { get; set; }
        public string MembershipValidUntil { get; set; }
        public int LateFee {  get; set; }
        public string Error { get; set; }
    }
}
