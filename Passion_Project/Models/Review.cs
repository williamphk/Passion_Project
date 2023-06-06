using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Passion_Project.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        [ForeignKey("Anime")]
        public int AnimeID { get; set; }
        public virtual Anime Anime { get; set; }

        [ForeignKey("Member")]
        public int MemberID { get; set; }
        public virtual Member Member { get; set; }
    }
    public class ReviewDto
    {
        public int ReviewID { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public int AnimeID { get; set; }
        public string AnimeName { get; set; }
        public int MemberID { get; set; }
        public string UserName { get; set; }
    }
}