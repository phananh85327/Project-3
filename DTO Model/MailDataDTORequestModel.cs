using System.ComponentModel.DataAnnotations;

namespace EC.DTO_Model
{
    public class MailDataDTORequestModel
    {
        public MailDataDTORequestModel()
        {
            this.MailTitle = string.Empty;
            this.MailBody = string.Empty;
        }

        [Required]
        public string MailTitle { get; set; }
        [Required]
        public string MailBody { get; set; }
    }
}
