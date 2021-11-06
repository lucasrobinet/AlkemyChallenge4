using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeAlkemy4.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public DateTime CreationDate { get; set; }
        public float Rate { get; set; }
        public string ImagePath { get; set; }
        public  ICollection<Genre> Genres { get; set; }

        public  ICollection<Character> Characters { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
