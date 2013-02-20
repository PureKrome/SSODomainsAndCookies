namespace WebApplication.Models
{
    public class IndexViewModel
    {
        public string Message { get; set; }
        public EncryptedCookieViewModel ThisCookie { get; set; }
        public EncryptedCookieViewModel OtherCookie { get; set; }
    }
}