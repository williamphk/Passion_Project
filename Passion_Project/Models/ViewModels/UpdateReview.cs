using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_Project.Models.ViewModels
{
    public class UpdateReview
    {
        //This viewmodel is a class which stores information that we need to present to /Review/Update/{}

        //the existing animal information

        public ReviewDto SelectedReview { get; set; }

        // all species to choose from when updating this animal

        public IEnumerable<AnimeDto> AnimeOptions { get; set; }
        public IEnumerable<MemberDto> MemberOptions { get; set; }

    }
}