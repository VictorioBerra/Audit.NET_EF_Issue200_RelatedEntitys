using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFCore.Models
{
    public class Cat : IAuditable
    {

        public int Id { get; set; }

        public int MeowLoudness { get; set; }

        public int? CatBreedId { get; set; }

        public CatBreed Breed { get; set; }

        [Required]
        public string CreatedByWUPeopleId { get; set; }

        [Required]
        public string CreatedByDisplayName { get; set; }
        
        public DateTime CreatedOnUtc { get; set; }

        [Required]
        public string UpdatedByWUPeopleId { get; set; }

        [Required]
        public string UpdatedByDisplayName { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
