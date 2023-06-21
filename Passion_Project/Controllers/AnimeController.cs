using Passion_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Passion_Project.Models.ViewModels;
using System.Diagnostics;

namespace Passion_Project.Controllers
{
    public class AnimeController : Controller
    {
        private static readonly HttpClient Client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AnimeController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            Client = new HttpClient(handler);
            Client.BaseAddress = new Uri("https://localhost:44338/api/");

        }
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            Client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") Client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }
        // GET: Anime/List
        public ActionResult List()
        {
            //Objective: communicate with anime data api to retrieve a list of animes
            //curl: https://localhost:44338/api/animedata/listanimes

            string url = "animedata/listanimes";
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            IEnumerable<AnimeDto> Animes = Response.Content.ReadAsAsync<IEnumerable<AnimeDto>>().Result;

            return View(Animes);
        }

        // GET: Anime/Details/5
        public ActionResult Details(int id)
        {
            DetailsAnime ViewModel = new DetailsAnime();
            //Objective: communicate with our anime data api to retrieve one anime
            //curl: https://localhost:44338/api/animedata/findanime/{id}

            string url = "animedata/findanime/" + id;
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            AnimeDto SelectedAnime = Response.Content.ReadAsAsync<AnimeDto>().Result;
            ViewModel.SelectedAnime = SelectedAnime;

            url = "genredata/listgenresforanime/" + id;
            Response = Client.GetAsync(url).Result;
            IEnumerable<GenreDto> AssociatedGenres = Response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;
            ViewModel.AssociatedGenres = AssociatedGenres;

            url = "genredata/listgenresavailableforanime/" + id;
            Response = Client.GetAsync(url).Result;
            IEnumerable<GenreDto> AvailableGenres = Response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;
            ViewModel.AvailableGenres = AvailableGenres;


            return View(ViewModel);
        }

        //POST: Anime/Associate/{animeid}
        [HttpPost]
        public ActionResult Associate(int id, int GenreID)
        {
            string url = "genredata/associategenrewithanime/" + GenreID + "/" + id;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, content).Result;
            return RedirectToAction("Details/" + id);
        }

        //GET: Anime/UnAssociate/{animeid}
        [HttpGet]
        public ActionResult UnAssociate(int id, int GenreID)
        {
            string url = "genredata/unassociategenrewithanime/" + GenreID + "/" + id;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, content).Result;
            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Anime/New
        [Authorize]
        public ActionResult New()
        {
            return View();
        }

        // POST: Anime/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Anime anime)
        {
            GetApplicationCookie();
            //Objective: add a new anime into our system using the API
            //curl -H "Content-Type:application/json" -d @anime.json https://localhost:44338/api/animedata/addanime
            string url = "animedata/addanime";

            string jsonpayload = jss.Serialize(anime);

            HttpContent httpContent = new StringContent(jsonpayload);
            httpContent.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, httpContent).Result;
            if (Response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }

        }

        // GET: Anime/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "animedata/findanime/" + id;
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            AnimeDto SelectedAnime = Response.Content.ReadAsAsync<AnimeDto>().Result;

            return View(SelectedAnime);
        }

        // POST: Anime/Update/5
        [HttpPost]
        public ActionResult Update(int id, Anime anime)
        {
            string url = "animedata/updateanime/" + id;

            string jsonpayload = jss.Serialize(anime);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, content).Result;
            if (Response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Anime/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "animedata/findanime/" + id;

            HttpResponseMessage Response = Client.GetAsync(url).Result;
            AnimeDto SelectedAnime = Response.Content.ReadAsAsync<AnimeDto>().Result;

            return View(SelectedAnime);
        }

        // POST: Anime/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "animedata/deleteanime/" + id;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, content).Result;
            if (Response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
