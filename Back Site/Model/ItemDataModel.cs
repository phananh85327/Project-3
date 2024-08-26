using System.ComponentModel.DataAnnotations;

namespace Back_Site.Model
{
    public class ItemDataModel
    {
        public string ItemID { get; set; }
        public string ItemNo { get; set; }
        public string CategoryID { get; set; }
        public string ListID { get; set; }
        public string Quantity { get; set; }
        public string Memo { get; set; }
        public string UserID { get; set; }
    }
}
