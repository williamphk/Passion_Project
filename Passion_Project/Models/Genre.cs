using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Passion_Project.Models
{
    public class Genre
    {
        [Key]
        public int GenreID { get; set; }
        public string GenreName { get; set; }
        public ICollection<Anime> Animes { get; set; }
    }
    public class GenreDto
    {
        public int GenreID { get; set; }
        public string GenreName { get; set; }
    }
}