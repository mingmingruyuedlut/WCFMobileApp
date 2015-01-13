
namespace Interactive.DBManager.Entity
{
    public class LoginEntity
    {
        public bool IsEmailExist { get; set; }
        public bool IsPwdValid { get; set; }
        public string Token { get; set; }

        public bool IsCustomerExistInMDB { get; set; }
        public bool IsCustomerExistInFP { get; set; }
        public string CustomerMail { get; set; }

    }
}
