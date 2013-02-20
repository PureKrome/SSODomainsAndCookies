using System.Web;

namespace WebApplication.Models
{
    public class IndexViewModel
    {
        public string Message { get; set; }
        public string CookieName { get; set; }
        public HttpCookie Cookie { get; set; }
        public string OtherCookieRaw { get; set; }
        public string OtherCookieDecrypted { get; set; }
    }
}