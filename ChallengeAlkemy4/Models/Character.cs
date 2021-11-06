using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeAlkemy4.Models
{
    public class Character
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int Age { get; set; }

        public int Weight { get; set; }

        public string History { get; set; }

        public string ImagePath { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }

        public  ICollection<Movie> Movies { get; set; }
    }
}