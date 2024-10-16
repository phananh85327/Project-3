using EC.Model;
using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class ItemDataDTOResponseModel
    {
        public ItemDataDTOResponseModel(ItemDataModel item)
        {
            this.ItemID = item.ItemID;
            this.ItemNo = item.ItemNo;
            this.ItemName = item.ItemName;
            this.ImageUrl = item.ImageUrl == null ? null : Convert.ToBase64String(item.ImageUrl);
            this.CategoryID = item.CategoryID;
            this.ListID = item.ListID;
            this.ListItem = item.ListItem;
            this.DigitalFlg = item.DigitalFlg;
            this.DigitalFile = item.DigitalFile == null ? null : Convert.ToBase64String(item.DigitalFile);
            this.Location = item.Location;
            this.Quantity = item.Quantity;
            this.Memo = item.Memo;
            this.StartTime = item.StartTime.HasValue ? item.StartTime.Value.ToString("yyyy-MM-ddTHH:mm") : string.Empty;
            this.EndTime = item.EndTime.HasValue ? item.EndTime.Value.ToString("yyyy-MM-ddTHH:mm") : string.Empty;
            this.CustomerID = item.CustomerID ?? string.Empty;
            this.NotificationTime = item.NotificationTime.HasValue ? item.NotificationTime.Value.ToString("yyyy-MM-ddTHH:mm") : string.Empty;
            this.NotificationSubject = item.NotificationSubject;
            this.NotificationBody = item.NotificationBody;
            this.SendFlg = item.SendFlg;
            this.CreatedBy = item.CreatedBy;
            this.LastChange = item.LastChange;
        }

        public string ItemID { get; set; }
        public int ItemNo { get; set; }
        public string ItemName { get; set; }
        public string? ImageUrl { get; set; }
        public string CategoryID { get; set; }
        public string? ListID { get; set; }
        public int? ListItem { get; set; }
        public bool DigitalFlg { get; set; }
        public string? DigitalFile { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public string Memo { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public string CustomerID { get; set; }
        public string? NotificationTime { get; set; }
        public string NotificationSubject { get; set; }
        public string NotificationBody { get; set; }
        public bool SendFlg { get; set; }
        public string CreatedBy { get; set; }
        public string LastChange { get; set; }
    }
}
