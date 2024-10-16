using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class ItemDataDTORequestModel
    {
        public ItemDataDTORequestModel()
        {
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
            this.CreatedBy = string.Empty;
            this.LastChange = string.Empty;
        }

        [Required]
        public string ItemName { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public string CategoryID { get; set; }
        public string? ListID { get; set; }
        public int? ListItem { get; set; }
        [Required]
        public bool DigitalFlg { get; set; }
        public string? DigitalFile { get; set; }
        public string Location { get; set; }
        public int Quantity { get; set; }
        public string Memo { get; set; }
        public string CreatedBy { get; set; }
        public string LastChange { get; set; }
    }
}
