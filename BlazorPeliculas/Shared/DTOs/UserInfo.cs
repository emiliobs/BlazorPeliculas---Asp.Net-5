using System.ComponentModel.DataAnnotations;

namespace BlazorPeliculas.Shared.DTOs
{
    public class UserInfo
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
