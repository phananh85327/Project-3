using EC.DTO_Model;

namespace EC.Model
{
    public class MailDataModel
    {
        public MailDataModel()
        {
            this.MailID = string.Empty;
            this.MailTitle = string.Empty;
            this.MailBody = string.Empty;
        }

        public MailDataModel(MailDataDTORequestModel mailData)
        {
            this.MailID = Guid.NewGuid().ToString();
            this.MailTitle = mailData.MailTitle;
            this.MailBody = mailData.MailBody;
        }

        public string MailID { get; set; }
        public string MailTitle { get; set; }
        public string MailBody { get; set; }
    }
}
