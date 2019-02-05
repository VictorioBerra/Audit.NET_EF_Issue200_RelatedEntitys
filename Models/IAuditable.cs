using System;

namespace EFCore.Models
{  
    public interface IAuditable
    {        
        string CreatedByWUPeopleId { get; set; }

        string CreatedByDisplayName { get; set; }

        DateTime CreatedOnUtc { get; set; }

        string UpdatedByWUPeopleId { get; set; }

        string UpdatedByDisplayName { get; set; }

        DateTime UpdatedOnUtc { get; set; }
    }
}