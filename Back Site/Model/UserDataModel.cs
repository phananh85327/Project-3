using System.ComponentModel.DataAnnotations;

namespace Back_Site.Model
{
    public class UserDataModel
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Pass { get; set; }
        public string Token { get; set; }
        public DateTime TokenExp { get; set; }
    }
}
