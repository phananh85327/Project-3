using EC.DTO_Model;
using System.ComponentModel.DataAnnotations;

namespace EC.Model
{
    public class ListDataModel
    {
        public ListDataModel()
        {
            this.ListID = string.Empty;
            this.ListName = string.Empty;
            this.Items = string.Empty;
        }

        public ListDataModel(ListDataDTORequestModel listData)
        {
            this.ListID = Guid.NewGuid().ToString();
            this.ListName = listData.ListName;
            this.Items = listData.Items;
        }

        public string ListID { get; set; }
        public string ListName { get; set; }
        public string Items { get; set; }
    }
}
