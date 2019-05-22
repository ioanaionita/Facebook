using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FacebookDAW.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; }
        public int Likes { get; set; }
        public int AlbumId { get; set; }
        public int NrPositiveComments { get; set; }
        public int NrNegativeComments { get; set; }
        public int NrNeutralComments { get; set; }
        public int PhotoImpact { get; set; }
        
        public virtual Album Album { get; set; }
        public virtual ICollection<Comment> Comments{ get; set; }
        public virtual ICollection<Profile> PeopleThatLiked { get; set; }
    }
}