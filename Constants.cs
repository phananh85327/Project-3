namespace EC
{
    public static class Constants
    {
        public static int TOKEN_VALID_HOURS = 1;
        public static int TOKEN_VALID_DAYS = 1;
        public static int MIN_PASS_LENGTH = 7;
        public static int MAIL_SEND_DELAY_TIME = 1;
        public static int MEMBERSHIP_VALID_DAYS = 365;
        public const string SUPER_USER_ROLE = "SUPER ADMIN";
        public const string DEFAULT_USER_ROLE = "ADMIN";
        public const string ITEM_SEPARATOR = "|";
        public const string ITEM_NO_SEPARATOR = ";";
        public const string DEFAULT_FILE_CONTENT_TYPE = "application/pdf";
        public const string DEFAULT_FILE_EXTENSION = ".pdf";
        public const string REGEX_EMAIL = "^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Za-z]{2,4}$";
        public const string REGEX_PHONE = "^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\\s\\./0-9]*$";
    }
}
