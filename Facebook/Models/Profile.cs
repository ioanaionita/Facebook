using Facebook.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacebookDAW.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
        public Boolean ProfileVisibility { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<Profile> Friends { get; set; }
        //se retine profilul persoanei careia i-am trimis cerere de prietenie
        public virtual ICollection<Profile> SentFriendRequests { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
    }
}