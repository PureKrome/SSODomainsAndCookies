using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _thisCookieName = ConfigurationManager.AppSettings["ThisCookieName"];
        private readonly string _otherCookieName = ConfigurationManager.AppSettings["OtherCookieName"];
        private readonly string _cookieDomain = ConfigurationManager.AppSettings["CookieDomain"];
        private readonly string _cookieSecret = ConfigurationManager.AppSettings["CookieSecret"];
        private readonly CryptographyManager _crypto;

        public HomeController()
        {
            _crypto = new CryptographyManager(); // SHA1/DES
        }

        [HttpGet]
        public ActionResult Index()
        {
            var indexViewModel = new IndexViewModel
                                     {
                                         CookieName = _thisCookieName,
                                     };

            if (TempData["IndexViewModelMessage"] != null)
            {
                indexViewModel.Message = TempData["IndexViewModelMessage"] as string;
            }

            // Search for cookie set by site two for this site.
            var cookie = Request.Cookies[_thisCookieName];
            if (cookie != null)
            {
                indexViewModel.Cookie = cookie;
            }

            // Search for cookie set by this site for site two.
            var otherCookie = Request.Cookies[_otherCookieName];
            if (otherCookie != null)
            {
                indexViewModel.OtherCookieRaw = otherCookie.Value;
                indexViewModel.OtherCookieDecrypted = _crypto.Decrypt(otherCookie.Value, _cookieSecret);
            }

            return View(indexViewModel);
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Create(string cookieData)
        {
            // Hash cookie value using HMACSHA1 (http://msdn.microsoft.com/en-us/library/system.security.cryptography.hmacsha1.aspx)
            var hashed = _crypto.Encrypt(cookieData, _cookieSecret);

            // Create their cookie.
            var cookie = new HttpCookie(_otherCookieName, hashed)
                             {
                                 Expires = DateTime.Now.AddMonths(1),
                                 Path = "/",
                                 Domain = _cookieDomain,
                                 HttpOnly = true
                             };
            
            // Note: If the cookie already exists (by name), it's updated/overwritten.
            Response.SetCookie(cookie);

            TempData["IndexViewModelMessage"] = "Cookie Saved";
            return RedirectToAction("Index");
        }
    }
}