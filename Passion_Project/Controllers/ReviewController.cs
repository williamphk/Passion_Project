using Passion_Project.Models;
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
    public class ReviewController : Controller
    {
        private static readonly HttpClient Client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ReviewController()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44338/api/");

        }
        // GET: Review/List
        public ActionResult List()
        {
            //Objective: communicate with review data api to retrieve a list of reviews
            //curl: https://localhost:44338/api/reviewdata/listreviews

            string url = "reviewdata/listreviews";
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            IEnumerable<ReviewDto> Reviews = Response.Content.ReadAsAsync<IEnumerable<ReviewDto>>().Result;

            return View(Reviews);
        }

        // GET: Review/Details/5
        public ActionResult Details(int id)
        {
            //Objective: communicate with our review data api to retrieve one review
            //curl: https://localhost:44338/api/reviewdata/findreview/{id}

            string url = "reviewdata/findreview/" + id;
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            ReviewDto SelectedReview = Response.Content.ReadAsAsync<ReviewDto>().Result;

            return View(SelectedReview);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Review/New
        public ActionResult New()
        {
            NewReview ViewModel = new NewReview();
            string url = "animedata/listanimes";
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            IEnumerable<AnimeDto> AnimeOptions = Response.Content.ReadAsAsync<IEnumerable<AnimeDto>>().Result;
            ViewModel.AnimeOptions = AnimeOptions;

            url = "memberdata/listmembers";
            Response = Client.GetAsync(url).Result;
            IEnumerable<MemberDto> MemberOptions = Response.Content.ReadAsAsync<IEnumerable<MemberDto>>().Result;
            ViewModel.MemberOptions = MemberOptions;

            return View(ViewModel);
        }

        // POST: Review/Create
        [HttpPost]
        public ActionResult Create(Review review)
        {
            //Objective: add a new review into our system using the API
            //curl -H "Content-Type:application/json" -d @review.json https://localhost:44338/api/reviewdata/addreview
            string url = "reviewdata/addreview";

            string jsonpayload = jss.Serialize(review);

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

        // GET: Review/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateReview ViewModel = new UpdateReview();
            string url = "reviewdata/findreview/" + id;
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            ReviewDto SelectedReview = Response.Content.ReadAsAsync<ReviewDto>().Result;
            ViewModel.SelectedReview = SelectedReview;

            url = "animedata/listanimes";
            Response = Client.GetAsync(url).Result;
            IEnumerable<AnimeDto> AnimeOptions = Response.Content.ReadAsAsync<IEnumerable<AnimeDto>>().Result;
            ViewModel.AnimeOptions = AnimeOptions;

            url = "memberdata/listmembers";
            Response = Client.GetAsync(url).Result;
            IEnumerable<MemberDto> MemberOptions = Response.Content.ReadAsAsync<IEnumerable<MemberDto>>().Result;
            ViewModel.MemberOptions = MemberOptions;

            return View(ViewModel);
        }

        // POST: Review/Update/5
        [HttpPost]
        public ActionResult Update(int id, Review review)
        {
            string url = "reviewdata/updatereview/" + id;

            string jsonpayload = jss.Serialize(review);

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

        // GET: Review/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "reviewdata/findreview/" + id;

            HttpResponseMessage Response = Client.GetAsync(url).Result;
            ReviewDto SelectedReview = Response.Content.ReadAsAsync<ReviewDto>().Result;

            return View(SelectedReview);
        }

        // POST: Review/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "reviewdata/deletereview/" + id;

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
