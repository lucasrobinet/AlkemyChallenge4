using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeAlkemy4.Models.DTO
{
    public class CharacterDTO
    {
        [Required]
        public string Name { get; set; }

        public int Age { get; set; }

        public int Weight { get; set; }

        public string History { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public List<int> MovieId { get; set; }
    }
}
