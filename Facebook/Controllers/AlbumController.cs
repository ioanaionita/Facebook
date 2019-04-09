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
        public ActionResult Index(int id)
        {
            if (User.Identity.GetUserId() == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string userId = db.Profiles.SingleOrDefault(p => p.Id == id).UserId;
            var albums = db.Albums.Where(a => a.UserId == userId);
            ViewBag.albums = albums;
            Profile profile = db.Profiles.Find(id);
            ViewBag.FirstName = profile.FirstName;
            ViewBag.LastName = profile.LastName;
            string currentUser = User.Identity.GetUserId();
            ViewBag.Delete = false;
            if(User.IsInRole("Administrator") || currentUser == userId)
            {
                ViewBag.Delete = true;
            }
            ViewBag.Pictures = db.Photos.ToList();
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
                    string userId = User.Identity.GetUserId();
                    album.UserId = userId;
                    db.Albums.Add(album);
                    string userProfile = album.UserId;
                    int profileId = db.Profiles.SingleOrDefault(p => p.UserId == userProfile).Id;
                    db.SaveChanges();
                    TempData["newAlbum"] = "The album has been added!";
                    return RedirectToAction("Index", new { id = profileId });
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
            string userId = album.UserId;
            ViewBag.allowLike = false;
            if(userId != User.Identity.GetUserId())
            {
                ViewBag.allowLike = true;
            }
            string currentUserId = User.Identity.GetUserId();
            Profile userProfile = db.Profiles.SingleOrDefault(u => u.UserId == currentUserId);
            ViewBag.userProfile = userProfile;
            ViewBag.currentProfile = db.Profiles.SingleOrDefault(p => p.UserId == userId);
            ViewBag.allowDelete = false;
            if(User.IsInRole("Administrator") || userId == User.Identity.GetUserId())
            {
                ViewBag.allowDelete = true;
            }
            return View(album);
        }
        //[HttpPost]
        public ActionResult AddPicture(int albumId, HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    string path = Path.Combine(Server.MapPath("~/Images"),
                                               Path.GetFileName(file.FileName));
                    Photo photo = new Photo();
                    photo.Description = "~/Images/" + file.FileName;
                    
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

        public ActionResult Like(int id)
        {
            Photo currentPhoto = db.Photos.Find(id);
            Album currentAlbum = db.Albums.SingleOrDefault(a => a.Id == currentPhoto.AlbumId);
            Profile currentProfile = db.Profiles.SingleOrDefault(p => p.UserId == currentAlbum.UserId);
            string currentUser = User.Identity.GetUserId();
            Profile userProfile = db.Profiles.SingleOrDefault(p => p.UserId == currentUser);
            //Tuple<Photo, Profile> tuplu = new Tuple<Photo, Profile>(currentPhoto, userProfile);
            if(currentPhoto.PeopleThatLiked == null)
            {
                currentPhoto.PeopleThatLiked = new List<Profile>();
            }
            if (!currentPhoto.PeopleThatLiked.Contains(userProfile))
            {
                currentPhoto.PeopleThatLiked.Add(userProfile);
            }
            if(userProfile.LikedPhotos == null)
            {
                userProfile.LikedPhotos = new List<Photo>();
            }
            if (!userProfile.LikedPhotos.Contains(currentPhoto))
            {
                userProfile.LikedPhotos.Add(currentPhoto);
            }
            currentPhoto.Likes = currentPhoto.PeopleThatLiked.ToList().Count();
            db.SaveChanges();
            return RedirectToAction("Show", new { id = currentAlbum.Id });
        }
        public ActionResult DeletePhoto(int id)
        {
            Photo currentPhoto = db.Photos.Find(id);
            Album currentAlbum = db.Albums.SingleOrDefault(a => a.Id == currentPhoto.AlbumId);
            db.Photos.Remove(currentPhoto);
            db.SaveChanges();
            return RedirectToAction("Show", new { id = currentAlbum.Id });
        }
        public ActionResult DeleteAlbum(int id)
        {
            Album deletedAlbum = db.Albums.Find(id);
            int profileId = db.Profiles.SingleOrDefault(p => p.UserId == deletedAlbum.UserId).Id;
            int albumId = deletedAlbum.Id;
            foreach(Photo p in db.Photos)
            {
                if(p.AlbumId == albumId)
                {
                    db.Photos.Remove(p);
                }
            }
            db.Albums.Remove(deletedAlbum);
            db.SaveChanges();
            return RedirectToAction("Index", new { id = profileId});
        }
    }
}