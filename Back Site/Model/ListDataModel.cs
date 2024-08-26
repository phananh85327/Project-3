using System.ComponentModel.DataAnnotations;

namespace Back_Site.Model
{
    public class ListDataModel
    {
        public string ListID { get; set; }
        public string ListName { get; set; }
        public string Items { get; set; }
        public string UserID { get; set; }
    }
}
