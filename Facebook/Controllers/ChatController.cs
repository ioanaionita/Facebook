using Facebook.Models;
using FacebookDAW.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Facebook.Controllers
{
    public class ChatController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        // GET: Chat
        public ActionResult Index()
        {
            string userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //caut profilul utilizatorului curent si preiau lista sa de prieteni
            //pentru fiecare prieten, adaug in baza de date un chat cu el 
            var userProfile = db.Profiles.SingleOrDefault(p => p.UserId == userId);
            ViewBag.myFriends = userProfile.Friends;
            return View();
        }
        
        public ActionResult StartNormalChat(int id) //primeste ca param id-ul profilului prietenului 
        {
            
            //ViewBag.friendId = id;
            string myId = User.Identity.GetUserId();
            //ViewBag.myId = myId;

            Profile friendProfile = db.Profiles.Find(id);
            Profile myProfile = db.Profiles.SingleOrDefault(p => p.UserId == myId);
            int chatId = new int(); 

            foreach(var chat in db.Chats)
            {
                //caut in baza de date chatul format doar din profilurile: myProfile si friendProfile
                if(chat.Profiles.Contains(myProfile) && chat.Profiles.Contains(friendProfile) && chat.Profiles.Count() == 2)
                {
                    chatId = chat.Id;

                    //de ce Viewbag.oldMessages e mereu null???????????????
                    if (chat.Messages == null)
                    {
                        ViewBag.oldMessages = "";
                    }
                    else
                    {
                        ViewBag.oldMessages = chat.Messages; // preiau mesajele vechi din conversatia cu profilul friendProfile

                    }
                    break;
                }               
            }

            ViewBag.firstnameFriend = friendProfile.FirstName;
            ViewBag.lastnameFriend = friendProfile.LastName;
            return RedirectToAction("NewMessage", new { chatId = chatId, senderId = myId});
            //return RedirectToAction("MessageBox", "Chat");
        }

        public ActionResult NewMessage(int chatId, string senderId)
        {
            ViewBag.chatId = chatId;
            ViewBag.SenderId = senderId; 
            Message message = new Message();
            return View("MessageBox", message); 
        }

        [HttpPost]
        [Authorize(Roles = "User,Editor,Administrator")]
        public ActionResult NewMessage(Message message)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    /*
                    ViewBag.Campchat = message.ChatId;
                    ViewBag.Campsender = message.SenderId;
                    ViewBag.Campcontent = message.Content;
                    */
                    // !! SendDate are valoarea 01.01.2018 00:00:00 = null in baza de date; modificare explicita 
                    message.SendDate = DateTime.Now;
                    //  ViewBag.Campdate = message.SendDate;
                    db.Messages.Add(message);
                    Chat chat = db.Chats.Find(message.ChatId);
                    chat.Messages.Add(message);
                    ViewBag.oldMessages = chat.Messages;
                    db.SaveChanges();
                   
                    TempData["message"] = "Message was sent!";
                    //return tot catre view-ul MessageBox pt a continua conversatia 
                    
                    return View("MessageBox", message);
                }
                else
                {
                    return View("MessageBox", message);
                }
            }
            catch (Exception e)
            {
                return View("MessageBox", message);
            }
        }
        
    }
}