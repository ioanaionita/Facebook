﻿using Facebook.Models;
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
            var profiles = db.Profiles;
            ViewBag.Profiles = profiles;

            return View();
        }
        public ActionResult Show(int id)
        {
            Profile profile = db.Profiles.Find(id);
            ViewBag.Profile = profile;
            ViewBag.DateOfBirth = profile.DateOfBirth.Date.ToString("dd.MM.yyyy");
            return View();
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

        //verificare ca un user sa nu-si creeze mai multe profiluri !!!
        [HttpPost]
        [Authorize(Roles = "Editor,Administrator")]
        public ActionResult New(Profile profile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Profiles.Add(profile);
                    db.SaveChanges();
                    TempData["message"] = "Profilul a fost adaugat!";
                    return RedirectToAction("Index");
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
    }
}