using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_Project.Models.ViewModels
{
    public class DetailsAnime
    {
        public AnimeDto SelectedAnime { get; set; }
        public IEnumerable<GenreDto> AssociatedGenres { get; set; }
        public IEnumerable<GenreDto> AvailableGenres { get; set; }
        public IEnumerable<ReviewDto> RelatedReviews { get; set; }
        public string CurrentUserID { get; set; }
    }
}