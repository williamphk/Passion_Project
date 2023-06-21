using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Passion_Project.Models;

namespace Passion_Project.Controllers
{
    public class AnimeDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all animes in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all animes in the database.
        /// </returns>
        /// <example>
        /// GET: api/AnimeData/ListAnimes
        /// </example>
        [HttpGet]
        [ResponseType(typeof(AnimeDto))]
        public IEnumerable<AnimeDto> ListAnimes()
        {
            List<Anime> Animes = db.Animes.ToList();
            List<AnimeDto> AnimeDtos = new List<AnimeDto>();

            Animes.ForEach(a => AnimeDtos.Add(new AnimeDto()
            {
                AnimeID = a.AnimeID,
                AnimeName = a.AnimeName,
                Description = a.Description,
                ReleaseDate = a.ReleaseDate
            }));

            return AnimeDtos;
        }

        /// <summary>
        /// Returns all Genres in the system associated with a particular anime.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Genres in the database associated with a particular anime
        /// </returns>
        /// <param name="id">Anime Primary Key</param>
        /// <example>
        /// GET: api/GenreData/ListGenresForAnime/1
        /// </example>
        [HttpGet]
        public IEnumerable<AnimeDto> ListAnimesForGenre(int id)
        {
            List<Anime> Animes = db.Animes.Where(a => a.Genres.Any(g => g.GenreID == id)).ToList();
            List<AnimeDto> AnimeDtos = new List<AnimeDto>();

            Animes.ForEach(a => AnimeDtos.Add(new AnimeDto()
            {
                AnimeID = a.AnimeID,
                AnimeName = a.AnimeName,
            }));

            return AnimeDtos;
        }

        /// <summary>
        /// Returns all Genres in the system available for a particular anime.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all Genres in the database available for a particular anime
        /// </returns>
        /// <param name="id">Anime Primary Key</param>
        /// <example>
        /// GET: api/GenreData/ListGenresForAnime/1
        /// </example>
        [HttpGet]
        public IEnumerable<AnimeDto> ListAnimesAvailableForGenre(int id)
        {
            List<Anime> Animes = db.Animes.Where(a => !a.Genres.Any(g => g.GenreID == id)).ToList();
            List<AnimeDto> AnimeDtos = new List<AnimeDto>();

            Animes.ForEach(a => AnimeDtos.Add(new AnimeDto()
            {
                AnimeID = a.AnimeID,
                AnimeName = a.AnimeName,
            }));

            return AnimeDtos;
        }

        /// <summary>
        /// Returns a particular anime in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: An anime in the system matching up to the anime ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the anime</param>
        /// <example>
        /// GET: api/AnimeData/FindAnime/5
        /// </example>
        [ResponseType(typeof(AnimeDto))]
        [HttpGet]
        public IHttpActionResult FindAnime(int id)
        {
            Anime Anime = db.Animes.Find(id);
            AnimeDto AnimeDto = new AnimeDto()
            {
                AnimeID = Anime.AnimeID,
                AnimeName = Anime.AnimeName,
                Description = Anime.Description,
                ReleaseDate = Anime.ReleaseDate
            };
            if (Anime == null)
            {
                return NotFound();
            }

            return Ok(AnimeDto);
        }

        /// <summary>
        /// Updates a particular anime in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the Anime ID primary key</param>
        /// <param name="anime">JSON FORM DATA of an anime</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/AnimeData/UpdateAnime/5
        /// FORM DATA: Anime JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateAnime(int id, Anime anime)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != anime.AnimeID)
            {
                return BadRequest();
            }

            db.Entry(anime).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimeExists(id))
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
        /// Adds an anime to the system
        /// </summary>
        /// <param name="anime">JSON FORM DATA of an anime</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Anime ID, Anime Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/AnimeData/AddAnime
        /// FORM DATA: Anime JSON Object
        /// </example>
        [ResponseType(typeof(Anime))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddAnime(Anime anime)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Animes.Add(anime);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = anime.AnimeID }, anime);
        }

        /// <summary>
        /// Deletes an anime from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the anime</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/AnimeData/DeleteAnime/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Anime))]
        [HttpPost]
        public IHttpActionResult DeleteAnime(int id)
        {
            Anime anime = db.Animes.Find(id);
            if (anime == null)
            {
                return NotFound();
            }

            db.Animes.Remove(anime);
            db.SaveChanges();

            return Ok(anime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AnimeExists(int id)
        {
            return db.Animes.Count(e => e.AnimeID == id) > 0;
        }
    }
}