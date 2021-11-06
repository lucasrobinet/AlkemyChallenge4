using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChallengeAlkemy4.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using ChallengeAlkemy4.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ChallengeAlkemy4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GenresController : ControllerBase
    {
        private readonly MovieContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public GenresController(MovieContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: api/Genres
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Genre>>> GetGenre()
        {
            return await _context.Genre.ToListAsync();
        }

        // GET: api/Genres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Genre>> GetGenre(int id)
        {
            var genre = _context.Genre
                            .Include(x => x.Movies)
                            .FirstOrDefaultAsync(x => x.Id == id);

            if (genre == null)
            {
                return NotFound();
            }

            return await genre;
        }

        // PUT: api/Genres/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGenre(int id, [FromForm] Genre genre)
        {
            if (id != genre.Id)
            {
                return BadRequest();
            }

            _context.Entry(genre).State = EntityState.Modified;

            try
            {
                if (genre.ImageFile == null)
                    await _context.SaveChangesAsync();
                else
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(genre.ImageFile.FileName);
                    string extension = Path.GetExtension(genre.ImageFile.FileName);
                    genre.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {

                        await genre.ImageFile.CopyToAsync(fileStream);
                    }
                    _context.Update(genre);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GenreExists(id))
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

        // POST: api/Genres
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Genre>> PostGenre([FromForm] GenreDTO genreDTO)
        {

            try
            {
                Genre genre = new();
                genre.Name = genreDTO.Name;
                genre.Movies = new List<Movie>();
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(genreDTO.ImageFile.FileName);
                string extension = Path.GetExtension(genreDTO.ImageFile.FileName);
                genre.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {

                    await genreDTO.ImageFile.CopyToAsync(fileStream);
                }

                _context.Genre.Add(genre);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetGenre", new { id = genre.Id }, genre);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        private bool GenreExists(int id)
        {
            return _context.Genre.Any(e => e.Id == id);
        }
    }
}
