using EC.Model;
using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class ListDataDTORequestModel
    {
        public ListDataDTORequestModel()
        {
            this.ListName = string.Empty;
            this.Items = string.Empty;
        }

        [Required]
        public string ListName { get; set; }
        [Required]
        public string Items { get; set; }
    }
}
