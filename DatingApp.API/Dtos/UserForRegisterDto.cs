using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        [StringLength(8, MinimumLength=3,ErrorMessage="Username must be between 3 and 8 characters")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}