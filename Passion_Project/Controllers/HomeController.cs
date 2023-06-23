using Passion_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace Passion_Project.Controllers
{
    public class HomeController : Controller
    {
        private static readonly HttpClient Client;

        static HomeController()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44338/api/");
        }

        // GET: /
        public ActionResult Index()
        {
            string url = "animedata/listanimes";
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            IEnumerable<AnimeDto> Animes = Response.Content.ReadAsAsync<IEnumerable<AnimeDto>>().Result;

            return View(Animes);
        }

        // GET: /About
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        // GET: /Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}