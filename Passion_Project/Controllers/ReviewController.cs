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
    public class ReviewController : Controller
    {
        private static readonly HttpClient Client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ReviewController()
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

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
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
        [Authorize]
        public ActionResult New()
        {
            NewReview ViewModel = new NewReview();
            string url = "animedata/listanimes";
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            IEnumerable<AnimeDto> AnimeOptions = Response.Content.ReadAsAsync<IEnumerable<AnimeDto>>().Result;
            ViewModel.AnimeOptions = AnimeOptions;

            return View(ViewModel);
        }

        // POST: Review/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Review review)
        {
            GetApplicationCookie();
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
        [Authorize]
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

            return View(ViewModel);
        }

        // POST: Review/Update/5
        [HttpPost]
        [Authorize]
        public ActionResult Update(int id, Review review)
        {
            GetApplicationCookie();
            string url = "reviewdata/updatereview/" + id;

            string jsonpayload = jss.Serialize(review);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage Response = Client.PostAsync(url, content).Result;
            Debug.WriteLine(Response.StatusCode);
            Debug.WriteLine(Response.Content.ReadAsStringAsync().Result);

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
        [Authorize]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "reviewdata/findreview/" + id;

            HttpResponseMessage Response = Client.GetAsync(url).Result;
            ReviewDto SelectedReview = Response.Content.ReadAsAsync<ReviewDto>().Result;

            return View(SelectedReview);
        }

        // POST: Review/Delete/5
        [HttpPost]
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
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
