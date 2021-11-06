using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChallengeAlkemy4.Models.ViewModels
{
    public class MovieDTO { 
    

        public string Title { get; set; }

        public DateTime CreationDate { get; set; }
        public float Rate { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public List<int> GenreId { get; set; }
        public List<int> CharacterId { get; set; }
    }
}
