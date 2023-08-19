using Passion_Project.Models;
using Passion_Project.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Passion_Project.Controllers
{
    public class GenreController : Controller
    {
        private static readonly HttpClient Client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static GenreController()
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

        // GET: Genre/List
        public ActionResult List()
        {
            //Objective: communicate with genre data api to retrieve a list of genres
            //curl: https://localhost:44338/api/genredata/listgenres

            string url = "genredata/listgenres";
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            IEnumerable<GenreDto> Genres = Response.Content.ReadAsAsync<IEnumerable<GenreDto>>().Result;

            return View(Genres);
        }

        // GET: Genre/Details/5
        public ActionResult Details(int id)
        {
            DetailsGenre ViewModel = new DetailsGenre();
            //Objective: communicate with our genre data api to retrieve one genre
            //curl: https://localhost:44338/api/genredata/findgenre/{id}

            string url = "genredata/findgenre/" + id;
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            GenreDto SelectedGenre = Response.Content.ReadAsAsync<GenreDto>().Result;
            ViewModel.SelectedGenre = SelectedGenre;

            url = "animedata/listanimesforgenre/" + id;
            Response = Client.GetAsync(url).Result;
            IEnumerable<AnimeDto> AssociatedAnimes = Response.Content.ReadAsAsync<IEnumerable<AnimeDto>>().Result;
            ViewModel.AssociatedAnimes = AssociatedAnimes;

            url = "animedata/listanimesavailableforgenre/" + id;
            Response = Client.GetAsync(url).Result;
            IEnumerable<AnimeDto> AvailableAnimes = Response.Content.ReadAsAsync<IEnumerable<AnimeDto>>().Result;
            ViewModel.AvailableAnimes = AvailableAnimes;


            return View(ViewModel);
        }

        //POST: Genre/Associate/{genreid}
        [HttpPost]
        [Authorize]
        public ActionResult Associate(int id, int AnimeID)
        {
            GetApplicationCookie();
            string url = "genredata/associategenrewithanime/" + id + "/" + AnimeID;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, content).Result;
            return RedirectToAction("Details/" + id);
        }

        //GET: Genre/UnAssociate/{genreid}
        [HttpGet]
        [Authorize]
        public ActionResult UnAssociate(int id, int AnimeID)
        {
            GetApplicationCookie();
            string url = "genredata/unassociategenrewithanime/" + id + "/" + AnimeID;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, content).Result;
            return RedirectToAction("Details/" + id);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Genre/New
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        // POST: Genre/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Genre genre)
        {
            GetApplicationCookie();
            //Objective: add a new genre into our system using the API
            //curl -H "Content-Type:application/json" -d @genre.json https://localhost:44338/api/genredata/addgenre
            string url = "genredata/addgenre";

            string jsonpayload = jss.Serialize(genre);

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

        // GET: Genre/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            string url = "genredata/findgenre/" + id;
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            GenreDto SelectedGenre = Response.Content.ReadAsAsync<GenreDto>().Result;

            return View(SelectedGenre);
        }

        // POST: Genre/Update/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Update(int id, Genre genre)
        {
            GetApplicationCookie();
            string url = "genredata/updategenre/" + id;

            string jsonpayload = jss.Serialize(genre);

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

        // GET: Genre/DeleteConfirm/5
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "genredata/findgenre/" + id;

            HttpResponseMessage Response = Client.GetAsync(url).Result;
            GenreDto SelectedGenre = Response.Content.ReadAsAsync<GenreDto>().Result;

            return View(SelectedGenre);
        }

        // POST: Genre/Delete/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "genredata/deletegenre/" + id;

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
