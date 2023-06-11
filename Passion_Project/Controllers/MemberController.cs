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
    public class MemberController : Controller
    {
        private static readonly HttpClient Client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static MemberController()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44338/api/");

        }
        // GET: Member/List
        public ActionResult List()
        {
            //Objective: communicate with member data api to retrieve a list of members
            //curl: https://localhost:44338/api/memberdata/listmembers

            string url = "memberdata/listmembers";
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            IEnumerable<MemberDto> Members = Response.Content.ReadAsAsync<IEnumerable<MemberDto>>().Result;

            return View(Members);
        }

        // GET: Member/Details/5
        public ActionResult Details(int id)
        {
            //Objective: communicate with our member data api to retrieve one member
            //curl: https://localhost:44338/api/memberdata/findmember/{id}

            string url = "memberdata/findmember/" + id;
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            MemberDto SelectedMember = Response.Content.ReadAsAsync<MemberDto>().Result;

            return View(SelectedMember);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Member/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Member/Create
        [HttpPost]
        public ActionResult Create(Member member)
        {
            //Objective: add a new member into our system using the API
            //curl -H "Content-Type:application/json" -d @member.json https://localhost:44338/api/memberdata/addmember
            string url = "memberdata/addmember";

            string jsonpayload = jss.Serialize(member);

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

        // GET: Member/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "memberdata/findmember/" + id;
            HttpResponseMessage Response = Client.GetAsync(url).Result;
            MemberDto SelectedMember = Response.Content.ReadAsAsync<MemberDto>().Result;

            return View(SelectedMember);
        }

        // POST: Member/Update/5
        [HttpPost]
        public ActionResult Update(int id, Member member)
        {
            string url = "memberdata/updatemember/" + id;

            string jsonpayload = jss.Serialize(member);

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

        // GET: Member/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "memberdata/findmember/" + id;

            HttpResponseMessage Response = Client.GetAsync(url).Result;
            MemberDto SelectedMember = Response.Content.ReadAsAsync<MemberDto>().Result;

            return View(SelectedMember);
        }

        // POST: Member/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "memberdata/deletemember/" + id;

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
