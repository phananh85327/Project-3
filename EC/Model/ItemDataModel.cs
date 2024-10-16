using EC.DTO_Model;
using System.ComponentModel.DataAnnotations;

namespace EC.Model
{
    public class ItemDataModel
    {
        public ItemDataModel()
        {
            this.ItemID = string.Empty;
            this.ItemNo = 1;
            this.ItemName = string.Empty;
            this.ImageUrl = null;
            this.CategoryID = string.Empty;
            this.ListID = null;
            this.ListItem = null;
            this.DigitalFlg = false;
            this.DigitalFile = null;
            this.Location = string.Empty;
            this.Quantity = 1;
            this.Memo = string.Empty;
            this.StartTime = null;
            this.EndTime = null;
            this.CustomerID = null;
            this.NotificationTime = null;
            this.NotificationSubject = string.Empty;
            this.NotificationBody = string.Empty;
            this.SendFlg = false;
            this.CreatedBy = string.Empty;
            this.LastChange = string.Empty;
        }

        public ItemDataModel(ItemDataDTORequestModel itemData, int itemNo, string itemID = "")
        {
            if (string.IsNullOrEmpty(itemID))
            {
                this.ItemID = Guid.NewGuid().ToString();
            }
            else
            {
                this.ItemID = itemID;
            }
            this.ItemNo = itemNo;
            this.ItemName = itemData.ItemName;
            this.ImageUrl = itemData.ImageUrl == null ? null : Convert.FromBase64String(itemData.ImageUrl);
            this.CategoryID = itemData.CategoryID;
            this.ListID = itemData.ListID;
            this.ListItem = itemData.ListItem;
            this.DigitalFlg = itemData.DigitalFlg;
            this.DigitalFile = itemData.DigitalFile == null ? null : Convert.FromBase64String(itemData.DigitalFile);
            this.Location = itemData.Location;
            this.Quantity = itemData.Quantity;
            this.Memo = itemData.Memo;
            this.StartTime = null;
            this.EndTime = null;
            this.CustomerID = null;
            this.NotificationTime = null;
            this.NotificationSubject = string.Empty;
            this.NotificationBody = string.Empty;
            this.SendFlg = false;
            this.CreatedBy = itemData.CreatedBy;
            this.LastChange = itemData.LastChange;
        }

        public string ItemID { get; set; }
        public int ItemNo { get; set; }
        public string ItemName { get; set; }
        public byte[]? ImageUrl { get; set; }
        [Required]
        public string CategoryID { get; set; }
        public string? ListID { get; set; }
        public int? ListItem { get; set; }
        public bool DigitalFlg { get; set; }
        public byte[]? DigitalFile { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public string Memo { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? CustomerID {  get; set; }
        public DateTime? NotificationTime { get; set; }
        public string NotificationSubject { get; set; }
        public string NotificationBody { get; set; }
        public bool SendFlg { get; set; }
        public string CreatedBy { get; set; }
        public string LastChange { get; set; }
    }
}
