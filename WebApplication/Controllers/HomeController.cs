using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var indexViewModel = new IndexViewModel
                                     {
                                         CookieName = ConfigurationManager.AppSettings["ThisCookieName"]
                                     };

            if (TempData["IndexViewModelMessage"] != null)
            {
                indexViewModel.Message = TempData["IndexViewModelMessage"] as string;
            }

            // Search for a particular cookie and dump info.
            var cookie = Request.Cookies[ConfigurationManager.AppSettings["ThisCookieName"]];
            if (cookie != null)
            {
                indexViewModel.Cookie = cookie;
            }

            return View(indexViewModel);
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult Create(string cookieData)
        {
            // Create their cookie.
            var cookie = new HttpCookie(ConfigurationManager.AppSettings["OtherCookieName"], cookieData)
                             {
                                 Expires = DateTime.Now.AddMonths(1),
                                 Path = "/",
                                 Domain = ConfigurationManager.AppSettings["OtherCookieDomain"]
                             };
            
            // Note: If the cookie already exists (by name), it's updated/overwritten.
            Response.SetCookie(cookie);

            TempData["IndexViewModelMessage"] = "Cookie Saved";
            return RedirectToAction("Index");
        }
    }
}