using Passion_Project.Models;
using Passion_Project.Models.ViewModels;
using System;
using System.Collections.Generic;
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
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44338/api/");

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
        public ActionResult Associate(int id, int AnimeID)
        {
            string url = "genredata/associategenrewithanime/" + id + "/" + AnimeID;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, content).Result;
            return RedirectToAction("Details/" + id);
        }

        //GET: Genre/UnAssociate/{genreid}
        [HttpGet]
        public ActionResult UnAssociate(int id, int AnimeID)
        {
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
        public ActionResult New()
        {
            return View();
        }

        // POST: Genre/Create
        [HttpPost]
        public ActionResult Create(Genre genre)
        {
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
        public ActionResult Edit(int id)
        {
            string url = "genredata/findgenre/" + id;
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            GenreDto SelectedGenre = Response.Content.ReadAsAsync<GenreDto>().Result;

            return View(SelectedGenre);
        }

        // POST: Genre/Update/5
        [HttpPost]
        public ActionResult Update(int id, Genre genre)
        {
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
        public ActionResult DeleteConfirm(int id)
        {
            string url = "genredata/findgenre/" + id;

            HttpResponseMessage Response = Client.GetAsync(url).Result;
            GenreDto SelectedGenre = Response.Content.ReadAsAsync<GenreDto>().Result;

            return View(SelectedGenre);
        }

        // POST: Genre/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
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
