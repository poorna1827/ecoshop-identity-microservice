using System.ComponentModel.DataAnnotations;

namespace IdentityMicroservice.Dto
{
    public class AdminRegisterDto
    {

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }


        [Required]
        [MaxLength(15)]
        public string? Name { get; set; }
    }
}
