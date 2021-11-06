using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeAlkemy4.Models.DTO
{
    public class GenreDTO
    {
        public string Name { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public List<Movie> MovieId { get; set; }
    }
}
