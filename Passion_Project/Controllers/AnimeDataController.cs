using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
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
                ReleaseDate = a.ReleaseDate,
                AnimeHasPic = a.AnimeHasPic,
                PicExtension = a.PicExtension,
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
                ReleaseDate = Anime.ReleaseDate,
                AnimeHasPic = Anime.AnimeHasPic,
                PicExtension = Anime.PicExtension,
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
        [Authorize]
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
            // Picture update is handled by another method
            db.Entry(anime).Property(a => a.AnimeHasPic).IsModified = false;
            db.Entry(anime).Property(a => a.PicExtension).IsModified = false;

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
        /// Receives anime picture data, uploads it to the webserver and updates the anime's HasPic option
        /// </summary>
        /// <param name="id">the anime id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// curl -F animepic=@file.jpg "https://localhost:xx/api/animedata/uploadanimepic/2"
        /// POST: api/animeData/UpdateanimePic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UploadAnimePic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var animePic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (animePic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(animePic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/animes/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Animes/"), fn);

                                //save the file
                                animePic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the anime haspic and picextension fields in the database
                                Anime Selectedanime = db.Animes.Find(id);
                                Selectedanime.AnimeHasPic = haspic;
                                Selectedanime.PicExtension = extension;
                                db.Entry(Selectedanime).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("Anime Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }

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
        [Authorize]
        public IHttpActionResult DeleteAnime(int id)
        {
            Anime anime = db.Animes.Find(id);
            if (anime == null)
            {
                return NotFound();
            }

            if (anime.AnimeHasPic && anime.PicExtension != "")
            {
                //also delete image from path
                string path = HttpContext.Current.Server.MapPath("~/Content/Players/" + id + "." + anime.PicExtension);
                if (System.IO.File.Exists(path))
                {
                    Debug.WriteLine("File exists... preparing to delete!");
                    System.IO.File.Delete(path);
                }
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