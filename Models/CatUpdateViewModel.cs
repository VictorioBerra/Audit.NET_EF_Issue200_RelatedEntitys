using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EFCore.Models
{
    public class CatUpdateViewModel
    {

        public CatUpdateViewModel()
        {
            CatBreedIds = new List<int>();
        }

        public int Id { get; set; }

        public int MeowLoudness { get; set; }

        public List<int> CatBreedIds { get; set; }
    }
}
