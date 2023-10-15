using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace IdentityMicroservice.Models
{


        [Index(nameof(Email), IsUnique = true)]
        public class Admin
        {

            [Key]
            public Guid AId { get; set; }

            public string? Email { get; set; }

            public string? Password { get; set; }

            public string? Name { get; set; }

        
    }
}
