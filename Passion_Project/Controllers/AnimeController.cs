﻿using Passion_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Passion_Project.Models.ViewModels;

namespace Passion_Project.Controllers
{
    public class AnimeController : Controller
    {
        private static readonly HttpClient Client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static AnimeController()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44338/api/");

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
            string url = "animedata/associateanimewithgenre/" + id + "/" + GenreID;

            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, content).Result;
            return RedirectToAction("Details/" + id);
        }

        //GET: Anime/UnAssociate/{animeid}
        [HttpGet]
        public ActionResult UnAssociate(int id, int GenreID)
        {
            string url = "animedata/unassociateanimewithgenre/" + id + "/" + GenreID;

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
        public ActionResult New()
        {
            return View();
        }

        // POST: Anime/Create
        [HttpPost]
        public ActionResult Create(Anime anime)
        {
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