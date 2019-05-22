using Facebook.Models;
using FacebookDAW.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VaderSharp;

namespace Facebook.Controllers
{
    public class CommentController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Comment
        public ActionResult Index(int id)
        {
            //iau url-ul pozei pentru a o afisa
            Photo currentPhoto = db.Photos.Find(id);
            ViewBag.currentPhoto = currentPhoto.Description;
            ViewBag.currentPhotoId = currentPhoto.Id;
            List<Comment> comments = new List<Comment>();
            foreach(var comment in db.Comments)
            {
                if(comment.PhotoId == id)
                {
                    comments.Add(comment);
                }
            }
            ViewBag.comments = comments;
            Album currentAlbum = db.Albums.Find(currentPhoto.AlbumId);
            ViewBag.albumId = currentAlbum.Id;
            ViewBag.albumName = currentAlbum.Name;
            ViewBag.allowDelete = false;
            if (User.IsInRole("Administrator"))
            {
                ViewBag.allowDelete = true;
            }
            return View();
        }
        [HttpPost]
        public ActionResult New(Comment comment, int id)
        {
            var commentedPhoto = db.Photos.FirstOrDefault(p => p.Id == id);

            SentimentIntensityAnalyzer analyzer = new SentimentIntensityAnalyzer();
            var results = analyzer.PolarityScores(comment.Content);
            if (results.Neutral > results.Positive && results.Neutral > results.Negative)
            {
                comment.CommentType = 0;
                commentedPhoto.NrNeutralComments++;
            }
            else if (results.Negative > results.Positive && results.Negative > results.Neutral)
            {
                comment.CommentType = -1;
                commentedPhoto.NrNegativeComments++;
            }
            else
            {
                comment.CommentType = 1;
                commentedPhoto.NrPositiveComments++;
            }

            if (commentedPhoto.NrPositiveComments > commentedPhoto.NrNeutralComments && commentedPhoto.NrPositiveComments > commentedPhoto.NrNegativeComments)
                commentedPhoto.PhotoImpact = 1;
            else if (commentedPhoto.NrNegativeComments > commentedPhoto.NrPositiveComments && commentedPhoto.NrNegativeComments > commentedPhoto.NrNeutralComments)
                commentedPhoto.PhotoImpact = -1;
            else commentedPhoto.PhotoImpact = 0;

            comment.PhotoId = id;
            string currentUserId = User.Identity.GetUserId();
            Profile profile = db.Profiles.SingleOrDefault(p => p.UserId == currentUserId);
            comment.ProfileId = profile.Id;
            comment.FirstNameUser = profile.FirstName;
            comment.LastNameUser = profile.LastName;
            comment.DateCreated = DateTime.Now;
            comment.UserId = currentUserId;
            Photo currentPhoto = db.Photos.Find(id);
            Album currentAlbum = db.Albums.Find(currentPhoto.AlbumId);
            //Iau profilul curent si vreau sa il compar cu cel care a adaugat acest comentariu.
            Profile ownerProfile = db.Profiles.SingleOrDefault(p => p.UserId == currentAlbum.UserId);
            //Daca cel care are poza adauga un comentariu, statusul comentariului va deveni direct 1
            //status = 1 -> comentariu acceptat, ce va fi afisat
            //status = 0 -> comentariu pending, asteapta sa fie acceptat sau refuzat de cel ce are poza
            //status = -1 -> comentariu refuzat, nu va fi niciodata afisat
            if(profile == ownerProfile)
            {
                comment.AcceptedStatus = 1;    
            }
            else
            {
                comment.AcceptedStatus = 0;
                TempData["pending"] = "Comentariul va fi acceptat/refuzat de catre proprietarul pozei.";
            }
            db.Comments.Add(comment);
            db.SaveChanges();

            return RedirectToAction("Index", new { id = comment.PhotoId });
        }

        //[HttpDelete]
        public ActionResult Delete(int id)
        {
            Comment comment = db.Comments.Find(id);
            int photoId = comment.PhotoId;
            db.Comments.Remove(comment);
            db.SaveChanges();
            TempData["message"] = "Comentariul a fost sters!";
            return RedirectToAction("Index", new { id = photoId });
        }
        public ActionResult AcceptComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            comment.AcceptedStatus = 1;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult RejectComment(int id)
        {
            Comment comment = db.Comments.Find(id);
            comment.AcceptedStatus = -1;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}