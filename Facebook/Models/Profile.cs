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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public Boolean ProfileVisibility { get; set; }
        public string UserId { get; set; }


        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<Profile> Friends { get; set; }
        public virtual ICollection<Chat> Chats { get; set; }
    }
}