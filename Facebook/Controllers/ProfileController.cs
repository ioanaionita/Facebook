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
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Profile
        public ActionResult Index()
        {
            // preluam lista de categorii din metoda GetAllCategories()
            //profile.Groups = GetAllGroups(profile);
            //profile.Friends = GetAllFriends(profile);
            //profile.UserId = User.Identity.GetUserId();
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }
            //transmitem catre Index view toate profilurile, in afara de cel al utilizatorului curent (People you may know);
            var currentUserId = User.Identity.GetUserId();
            var profiles = db.Profiles.Where(p => p.UserId!=currentUserId);
            ViewBag.Profiles = profiles;

            return View();
        }
        public ActionResult Show(int id)
        {
            Profile profile = db.Profiles.Find(id);
            ViewBag.Profile = profile;
            ViewBag.DateOfBirth = profile.DateOfBirth.Date.ToString("dd.MM.yyyy");
            ViewBag.allowEdit = false;
            ViewBag.currentUser = User.Identity.GetUserId();
            ViewBag.currentProfile = profile.Id;
            if(profile.UserId == User.Identity.GetUserId() && (User.IsInRole("Administrator") || User.IsInRole("Editor")))
            {
                ViewBag.allowEdit = true;
            }
            if (User.IsInRole("Administrator"))
            {
                ViewBag.allowEdit = true;
            }
            if (TempData.ContainsKey("update"))
            {
                ViewBag.update = TempData["update"].ToString();
            }
            return View(profile);
        }


        public ActionResult New()
        {
            Profile profile = new Profile();
            profile.UserId = User.Identity.GetUserId();
            return View(profile);
        }

        [NonAction] //pentru aceasta metoda si dropdown (Vezi curs Pag 16-18)
        public List<Group> GetAllGroups(Profile profile)
        {
            // generam o lista goala
            var selectList = new List<Group>();
            // Extragem toate grupurile din baza de date
            var groups = from g in db.Groups select g;
            // iteram prin grupuri

            foreach (var g in groups)
            {
                //Caut grupurile care contin profilul cautat in lista lor de profile
                if (g.Profiles.Contains(profile))
                {
                    selectList.Add(g);
                }
            }
            // returnam lista de grupuri
            return selectList;
        }
        [NonAction] //pentru aceasta metoda si dropdown (Vezi curs Pag 16-18)
        public List<Profile> GetAllFriends(Profile profile)
        {
            // generam o lista goala
            var selectList = new List<Profile>();
            // Extragem toate profilurile din baza de date
            var friends = from f in db.Profiles select f;
            // iteram prin profiluri
            foreach (var f in friends)
            {
                //Caut profilurile prietene cu profilul meu
                if (f.Friends.Contains(profile))
                {
                    selectList.Add(f);
                }
            }
            // returnam lista de prieteni
            return selectList;
        }

        [Authorize(Roles = "User, Editor, Administrator")]
        public ActionResult MyProfile(string userId)
        {
            int ok = 0; //spune daca am gasit profilul ce apartine Userului cu id-ul userId
            Profile myProfile = new Profile();
            foreach (var profile in db.Profiles)
            {
                if (profile.UserId == userId)
                {
                    ok = 1;
                    myProfile = profile;
                }
            }
            if (ok == 1)
                return RedirectToAction("Show", new { id = myProfile.Id });
            else
             return RedirectToAction("New");
        }

        //verificare ca un user sa nu-si creeze mai multe profiluri !!!
        [HttpPost]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult New(Profile profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost adaugat!";
                    return RedirectToAction("Index", "Profile");
                }
                else
                {
                    return View(profile);
                }
            }
            catch (Exception e)
            {
                return View(profile);
            }
        }
        [Authorize(Roles = "User, Editor,Administrator")]
        public ActionResult Edit(int id)
        {
            Profile profile = db.Profiles.Find(id);
            if(profile.UserId != User.Identity.GetUserId())
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.Profile = profile;
            return View(profile);
        }

        [HttpPut]
        public ActionResult Edit(int id, Profile requestProfile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Profile profile = db.Profiles.Find(id);
                    if (TryUpdateModel(profile))
                    {
                        profile.FirstName = requestProfile.FirstName;
                        profile.LastName = requestProfile.LastName;
                        profile.DateOfBirth = requestProfile.DateOfBirth;
                        profile.City = requestProfile.City;
                        profile.Country = requestProfile.Country;
                        profile.ProfileVisibility = requestProfile.ProfileVisibility;
                        db.SaveChanges();
                        TempData["update"] = "Profilul a fost modificat!";
                        
                    }
                    return RedirectToAction("Show", new { id = profile.Id });
                }
                else
                {
                    return View();
                }

            }
            catch(Exception e)
            {
                return View();
            }
        }
        [HttpPost]
        public void AddFriend(Profile profile)
        {
            
        }

        public ActionResult FriendsAndGroups(int id)
        {
            //int id = int.Parse(profileId);
            Profile profile = db.Profiles.Find(id);
            ViewBag.profile = profile;
            ViewBag.numberOfFriends = profile.Friends.Count();
            ViewBag.friends = profile.Friends;
            ViewBag.numberOfGroups = profile.Groups.Count();
            ViewBag.groups = profile.Groups;
            return View();
        }
    }
}