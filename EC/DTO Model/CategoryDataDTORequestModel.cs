using EC.Model;
using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class CategoryDataDTORequestModel
    {
        public CategoryDataDTORequestModel()
        {
            this.CategoryName = string.Empty;
        }

        [Required]
        public string CategoryName { get; set; }
    }
}
