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
using Passion_Project.Models;

namespace Passion_Project.Controllers
{
    public class ReviewDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all reviews in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all reviews in the database.
        /// </returns>
        /// <example>
        /// GET: api/ReviewData/ListReviews
        /// </example>
        [HttpGet]
        [ResponseType(typeof(ReviewDto))]
        public IEnumerable<ReviewDto> ListReviews()
        {
            List<Review> Reviews = db.Reviews.ToList();
            List<ReviewDto> ReviewDtos = new List<ReviewDto>();

            Reviews.ForEach(r => ReviewDtos.Add(new ReviewDto()
            {
                ReviewID = r.ReviewID,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate,
                AnimeID = r.Anime.AnimeID,
                AnimeName = r.Anime.AnimeName,
                MemberID = r.Member.MemberID,
                FirstName = r.Member.FirstName,
                LastName = r.Member.LastName,
            }));

            return ReviewDtos;
        }

        /// <summary>
        /// Returns a particular review in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An review in the system matching up to the review ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the review</param>
        /// <example>
        /// GET: api/ReviewData/FindReview/5
        /// </example>
        [ResponseType(typeof(ReviewDto))]
        [HttpGet]
        public IHttpActionResult FindReview(int id)
        {
            Review Review = db.Reviews.Find(id);
            ReviewDto ReviewDto = new ReviewDto()
            {
                ReviewID = Review.ReviewID,
                Rating = Review.Rating,
                Comment = Review.Comment,
                ReviewDate = Review.ReviewDate,
                AnimeID = Review.Anime.AnimeID,
                AnimeName = Review.Anime.AnimeName,
                MemberID = Review.Member.MemberID,
                FirstName = Review.Member.FirstName,
                LastName = Review.Member.LastName,
            };
            if (Review == null)
            {
                return NotFound();
            }

            return Ok(ReviewDto);
        }

        /// <summary>
        /// Updates a particular review in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Review ID primary key</param>
        /// <param name="review">JSON FORM DATA of an review</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/ReviewData/UpdateReview/5
        /// FORM DATA: Review JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateReview(int id, Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != review.ReviewID)
            {
                return BadRequest();
            }

            db.Entry(review).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
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
        /// Adds an review to the system
        /// </summary>
        /// <param name="review">JSON FORM DATA of an review</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Review ID, Review Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/ReviewData/AddReview
        /// FORM DATA: Review JSON Object
        /// </example>
        [ResponseType(typeof(Review))]
        [HttpPost]
        public IHttpActionResult AddReview(Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Reviews.Add(review);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = review.ReviewID }, review);
        }

        /// <summary>
        /// Deletes an review from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the review</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/ReviewData/DeleteReview/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Review))]
        [HttpPost]
        public IHttpActionResult DeleteReview(int id)
        {
            Review review = db.Reviews.Find(id);
            if (review == null)
            {
                return NotFound();
            }

            db.Reviews.Remove(review);
            db.SaveChanges();

            return Ok(review);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReviewExists(int id)
        {
            return db.Reviews.Count(e => e.ReviewID == id) > 0;
        }
    }
}