using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Passion_Project.Models
{
    public class Anime
    {
        [Key]
        public int AnimeID { get; set; }
        public string AnimeName { get; set; }
        public DateTime ReleaseDate { get; set; }
        [AllowHtml]
        public string Description { get; set; }
        public ICollection<Genre> Genres { get; set; }
        public bool AnimeHasPic { get; set; }
        public string PicExtension { get; set; }
    }
    public class AnimeDto
    {
        public int AnimeID { get; set; }
        public string AnimeName { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string FormattedDate
        {
            get
            {
                return ReleaseDate.ToString("yyyy-MM-dd");
            }
        }
        [AllowHtml]
        public string Description { get; set; }
        public bool AnimeHasPic { get; set; }
        public string PicExtension { get; set; }
    }
}