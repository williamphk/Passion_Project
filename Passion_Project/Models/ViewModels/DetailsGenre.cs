using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_Project.Models.ViewModels
{
    public class DetailsGenre
    {
        public GenreDto SelectedGenre { get; set; }
        public IEnumerable<AnimeDto> AssociatedAnimes { get; set; }
        public IEnumerable<AnimeDto> AvailableAnimes { get; set; }
    }
}