using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passion_Project.Models.ViewModels
{
    public class NewReview
    {
        public IEnumerable<AnimeDto> AnimeOptions { get; set; }
        public IEnumerable<MemberDto> MemberOptions { get; set; }
    }
}