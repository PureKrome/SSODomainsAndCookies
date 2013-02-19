using System.Web;

namespace WebApplication.Models
{
    public class IndexViewModel
    {
        public string Message { get; set; }
        public string CookieName { get; set; }
        public HttpCookie Cookie { get; set; }
    }
}