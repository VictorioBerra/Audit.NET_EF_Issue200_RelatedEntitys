﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFCore.Models
{
    public class Cat : IAuditable, IEntity
    {

        public Cat()
        {
            CatBreedLine = new HashSet<CatBreedLine>();
        }

        public int Id { get; set; }

        public int MeowLoudness { get; set; }

        public ICollection<CatBreedLine> CatBreedLine { get; set; }


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
