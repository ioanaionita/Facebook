using Facebook.Models;
using FacebookDAW.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Facebook.Controllers
{
    public class AlbumController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Album
        public ActionResult Index(string userId)
        {
            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var albums = db.Albums.Where(a => a.UserId == userId);
            ViewBag.albums = albums;
            return View();
        }
        public ActionResult New(string userId)
        {
            if(User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            Album album = new Album();
            album.UserId = userId;
            return View(album);
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult New(Album album)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Albums.Add(album);
                    db.SaveChanges();
                    TempData["message"] = "Albumul a fost adaugat!";
                    return RedirectToAction("Index", "Album");
                }
                else
                {
                    return View(album);
                }
            }
            catch (Exception e)
            {
                return View(album);
            }
        }

        public ActionResult Show (int id)
        {
            Album album = db.Albums.Find(id);
            var photos = db.Photos.Where(p => p.AlbumId == id);
            ViewBag.photos = photos;
            return View(album);
        }
        [HttpPost]
        public ActionResult AddPicture(int albumId, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Images"),
                                               Path.GetFileName(file.FileName));
                    Photo photo = new Photo();
                    photo.Description = path;
                    
                    photo.AlbumId = albumId;
                    db.Photos.Add(photo);
                    db.SaveChanges();
                    file.SaveAs(path);
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return RedirectToAction("Show", new { id = albumId });
        }
    }
}