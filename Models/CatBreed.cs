using System;
using System.Collections.Generic;

namespace EFCore.Models
{
    public class CatBreed : IAuditable
    {
        public CatBreed()
        {
            Cats = new HashSet<Cat>();
        }

        public int Id { get; set; }

        public string BreedName { get; set; }

        public ICollection<Cat> Cats { get; }

        public string CreatedByWUPeopleId { get; set; }
        public string CreatedByDisplayName { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public string UpdatedByWUPeopleId { get; set; }
        public string UpdatedByDisplayName { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
    }
}
