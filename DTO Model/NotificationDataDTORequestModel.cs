using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class NotificationDataDTORequestModel
    {
        public NotificationDataDTORequestModel()
        {
            this.ItemID = string.Empty;
            this.ItemNo = 1;
            this.StartTime = null;
            this.EndTime = null;
            this.CustomerID = string.Empty;
            this.NotificationTime = null;
            this.NotificationSubject = string.Empty;
            this.NotificationBody = string.Empty;
            this.SendFlg = false;
        }

        public string ItemID { get; set; }
        public int ItemNo { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [Required]
        public string CustomerID { get; set; }
        public DateTime? NotificationTime { get; set; }
        public string NotificationSubject { get; set; }
        public string NotificationBody { get; set; }
        public bool SendFlg { get; set; }
    }
}
