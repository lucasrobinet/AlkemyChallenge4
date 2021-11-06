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
    public class CharactersController : ControllerBase
    {
        private readonly MovieContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CharactersController(MovieContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: api/Characters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharacter(string name, int? age, int? idMovie)
        {
            var character = _context.Character;

            if (name != null)
            {
                var charact = _context.Character.Where(x => x.Name.Contains(name));

                return Ok(charact);
            }
            if (age != null)
            {
                var charact = _context.Character.Where(x => x.Age == age);

                return Ok(charact);
            }
            if(idMovie != null)
            {
                var charact = _context.Character.Where(x => x.Movies.Any(x => x.Id == idMovie));

                return Ok(charact);
            }
            return await _context.Character
                    .Select(p => new Character
                    {
                        Name = p.Name,
                        ImagePath = p.ImagePath,  //Aqui iria imageFile pero como no puedo ver si la devuelve pongo imagepath
                    })
                    .ToListAsync();
        }

        // GET: api/Characters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Character>> GetCharacter(int id)
        {
            var character = _context.Character
                            .Include(x => x.Movies)
                            .FirstOrDefaultAsync(x => x.Id == id);
            if (character == null)
            {
                return NotFound();
            }

            return await character;
        }

        // PUT: api/Characters/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCharacter(int id, [FromForm] Character character)
        {
            if (id != character.Id)
            {
                return BadRequest();
            }

            _context.Entry(character).State = EntityState.Modified;

            try
            {
                if (character.ImageFile == null)
                    await _context.SaveChangesAsync();
                else
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(character.ImageFile.FileName);
                    string extension = Path.GetExtension(character.ImageFile.FileName);
                    character.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {

                        await character.ImageFile.CopyToAsync(fileStream);
                    }
                    _context.Update(character);
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CharacterExists(id))
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

        // POST: api/Characters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Character>> PostCharacter([FromForm] CharacterDTO characterDTO)
        {

            try
            {
                Character character = new();
                character.Name = characterDTO.Name;
                character.Age = characterDTO.Age;
                character.Weight = characterDTO.Weight;
                character.History = characterDTO.History;
                character.Movies = new List<Movie>();
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(characterDTO.ImageFile.FileName);
                string extension = Path.GetExtension(characterDTO.ImageFile.FileName);
                character.ImagePath = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {

                    await characterDTO.ImageFile.CopyToAsync(fileStream);
                }

                _context.Character.Add(character);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetCharacter", new { id = character.Id }, character);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        // DELETE: api/Characters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCharacter(int id)
        {
            var character = await _context.Character.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }

            _context.Character.Remove(character);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CharacterExists(int id)
        {
            return _context.Character.Any(e => e.Id == id);
        }
    }
}
