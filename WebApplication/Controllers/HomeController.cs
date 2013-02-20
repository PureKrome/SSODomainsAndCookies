using System;
using System.Configuration;
using System.Security.Cryptography;
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
        private readonly string _cookieSalt = ConfigurationManager.AppSettings["CookieSalt"];
        
        [HttpGet]
        public ActionResult Index()
        {
            var indexViewModel = new IndexViewModel();

            if (TempData["IndexViewModelMessage"] != null)
            {
                indexViewModel.Message = TempData["IndexViewModelMessage"] as string;
            }

            // Search for cookie set by site two for this site.
            var cookie = Request.Cookies[_thisCookieName];
            if (cookie != null)
            {
                try
                {
                    indexViewModel.ThisCookie = new EncryptedCookieViewModel(cookie.Name, cookie.Value, CipherUtility.Decrypt<AesManaged>(cookie.Value, _cookieSecret, _cookieSalt));
                }
                catch (Exception exc)
                {
                    indexViewModel.ThisCookie = new EncryptedCookieViewModel(cookie.Name, cookie.Value, exc.Message);
                }
            }

            // Search for cookie set by this site for site two.
            var otherCookie = Request.Cookies[_otherCookieName];
            if (otherCookie != null)
            {
                try
                {
                    indexViewModel.OtherCookie = new EncryptedCookieViewModel(otherCookie.Name, otherCookie.Value, CipherUtility.Decrypt<AesManaged>(otherCookie.Value, _cookieSecret, _cookieSalt));
                }
                catch (Exception exc)
                {
                    indexViewModel.OtherCookie = new EncryptedCookieViewModel(otherCookie.Name, otherCookie.Value, exc.Message);
                }
            }

            return View(indexViewModel);
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Create(string cookieData)
        {
            // Hash cookie value using SHA1/DES
            var hashed = CipherUtility.Encrypt<AesManaged>(cookieData, _cookieSecret, _cookieSalt);

            // Create their cookie.
            var cookie = new HttpCookie(_otherCookieName, hashed)
                             {
                                 Expires = DateTime.Now.AddMonths(1), // To be defined: if left out, cookie is not persistent.
                                 Path = "/", // So all subpages can access
                                 Domain = _cookieDomain, // must be top level, e.g "mysite.com", not "siteone.mysite.com"
                                 HttpOnly = true // so it can't be access by javascript.
                             };

            // Note: If the cookie already exists (by name), it's updated/overwritten.
            Response.Cookies.Add(cookie);

            TempData["IndexViewModelMessage"] = "Cookie Saved";
            return RedirectToAction("Index");
        }
    }
}