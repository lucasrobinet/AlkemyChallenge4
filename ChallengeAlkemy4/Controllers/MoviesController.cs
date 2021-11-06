using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChallengeAlkemy4.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ChallengeAlkemy4.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ChallengeAlkemy4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly MovieContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MoviesController(MovieContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovie(string title, int? idGenre, string order)
        {

            if (title != null)
            {
                var movie = _context.Movie.Where(x => x.Title.Contains(title));

                return Ok(movie);
            }
            if (idGenre != null)
            {
                var movie  = _context.Movie.Where(x => x.Genres.Any(x => x.Id == idGenre));

                return Ok(movie);
            }
            if (order != null)
            {
                if (order == "asc" || order == "ASC")
                {
                    var movie = _context.Movie.OrderBy(x => x.Title);

                    return Ok(movie);
                }
                if (order == "desc" || order == "DESC")
                {
                    var movie = _context.Movie.OrderByDescending(x => x.CreationDate);

                    return Ok(movie);
                }
                else
                {
                    return NotFound();
                }
            }
            else
                return await _context.Movie
                    .Select(p => new Movie
                    {
                        Title = p.Title,
                        ImagePath = p.ImagePath, //Aqui iria imageFile pero como no puedo ver si la devuelve pongo imagepath
                        CreationDate = p.CreationDate
                    })
                    .ToListAsync();
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {

            var movie = _context.Movie
                .Include(x => x.Characters)
                .Include(x => x.Genres)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (movie == null)
            {
                return NotFound();
            }

            return await movie;
        }

        // PUT: api/Movies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovie(int id, [FromForm] Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }

            _context.Entry(movie).State = EntityState.Modified;
                try
                {
                    if (movie.ImageFile == null)
                        await _context.SaveChangesAsync();
                    else
                    {
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(movie.ImageFile.FileName);
                        string extension = Path.GetExtension(movie.ImageFile.FileName);
                        movie.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {

                            await movie.ImageFile.CopyToAsync(fileStream);
                        }
                        _context.Update(movie);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            
            return NoContent();
        }

        // POST: api/Movies
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostMovie([FromForm] MovieDTO movieRecipent)
        {
            try
            {
                Movie movie = new(); 
                movie.CreationDate = movieRecipent.CreationDate;
                movie.Title = movieRecipent.Title;
                movie.Rate = movieRecipent.Rate;
                movie.Genres = new List<Genre>();
                movie.Characters = new List<Character>();
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(movieRecipent.ImageFile.FileName);
                string extension = Path.GetExtension(movieRecipent.ImageFile.FileName);
                movie.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {

                    await movieRecipent.ImageFile.CopyToAsync(fileStream);
                }

                foreach(var characterIds in movieRecipent.CharacterId)
                {
                    var character = _context.Character.Find(characterIds);
                    if (character != null)
                    {
                        movie.Characters.Add(character);
                    }
                }
                
                foreach (var genreIds in movieRecipent.GenreId)
                {
                    var genre = _context.Genre.Find(genreIds);
                    if (genre != null)
                    {
                        movie.Genres.Add(genre);
                    }

                }

                _context.Movie.Add(movie);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }

            _context.Movie.Remove(movie);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
