using System;
using System.Collections.Generic;

namespace EFCore.Models
{
    public class CatBreedLine : IAuditable
    {

        public virtual int CatId { get; set; }

        public virtual int CatBreedId { get; set; }

        public Cat Cat { get; set; }

        public CatBreed CatBreed { get; set; }

        public string CreatedByWUPeopleId { get; set; }
        public string CreatedByDisplayName { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string UpdatedByWUPeopleId { get; set; }
        public string UpdatedByDisplayName { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
