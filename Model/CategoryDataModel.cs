using EC.DTO_Model;
using System.ComponentModel.DataAnnotations;

namespace EC.Model
{
    public class CategoryDataModel
    {
        public CategoryDataModel()
        {
            this.CategoryID = string.Empty;
            this.CategoryName = string.Empty;
        }

        public CategoryDataModel(CategoryDataDTORequestModel categoryData)
        {
            this.CategoryID = Guid.NewGuid().ToString();
            this.CategoryName = categoryData.CategoryName;
        }

        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
    }
}
