using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Ajax.Utilities;
using Passion_Project.Models;
using Member = Passion_Project.Models.Member;

namespace Passion_Project.Controllers
{
    public class MemberDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all members in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all members in the database.
        /// </returns>
        /// <example>
        /// GET: api/MemberData/ListMembers
        /// </example>
        [HttpGet]
        [ResponseType(typeof(MemberDto))]
        public IEnumerable<MemberDto> ListMembers()
        {
            List<Member> Members = db.Members.ToList();
            List<MemberDto> MemberDtos = new List<MemberDto>();

            Members.ForEach(m => MemberDtos.Add(new MemberDto()
            {
                MemberID = m.MemberID,
                UserName = m.UserName,
                Password = m.Password,
            }));

            return MemberDtos;
        }

        /// <summary>
        /// Returns a particular member in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An member in the system matching up to the member ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the member</param>
        /// <example>
        /// GET: api/MemberData/FindMember/5
        /// </example>
        [ResponseType(typeof(MemberDto))]
        [HttpGet]
        public IHttpActionResult FindMember(int id)
        {
            Member Member = db.Members.Find(id);
            MemberDto MemberDto = new MemberDto()
            {
                MemberID = Member.MemberID,
                UserName = Member.UserName,
                Password = Member.Password,
            };
            if (Member == null)
            {
                return NotFound();
            }

            return Ok(MemberDto);
        }

        /// <summary>
        /// Updates a particular member in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Member ID primary key</param>
        /// <param name="member">JSON FORM DATA of an member</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/MemberData/UpdateMember/5
        /// FORM DATA: Member JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateMember(int id, Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != member.MemberID)
            {
                return BadRequest();
            }

            db.Entry(member).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Adds an member to the system
        /// </summary>
        /// <param name="member">JSON FORM DATA of an member</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Member ID, Member Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/MemberData/AddMember
        /// FORM DATA: Member JSON Object
        /// </example>
        [ResponseType(typeof(Member))]
        [HttpPost]
        public IHttpActionResult AddMember(Member member)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Members.Add(member);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = member.MemberID }, member);
        }

        /// <summary>
        /// Deletes an member from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the member</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/MemberData/DeleteMember/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Member))]
        [HttpPost]
        public IHttpActionResult DeleteMember(int id)
        {
            Member member = db.Members.Find(id);
            if (member == null)
            {
                return NotFound();
            }

            db.Members.Remove(member);
            db.SaveChanges();

            return Ok(member);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MemberExists(int id)
        {
            return db.Members.Count(e => e.MemberID == id) > 0;
        }
    }
}