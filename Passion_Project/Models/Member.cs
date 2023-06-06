using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Passion_Project.Models
{
    public class Member
    {
        [Key]
        public int MemberID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class MemberDto
    {
        public int MemberID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}