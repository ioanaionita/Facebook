using Facebook.Models;
using FacebookDAW.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Facebook.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        public ActionResult Index()
        {
            if(User.Identity.GetUserId()!= null)
            {
                string userId = User.Identity.GetUserId();
                Profile currentProfile = db.Profiles.SingleOrDefault(p => p.UserId == userId);
                Notification notifications = db.Notifications.SingleOrDefault(n => n.ReceiverId == currentProfile.Id);
                 
                ViewBag.notifications = notifications;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}