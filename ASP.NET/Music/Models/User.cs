using System.ComponentModel.DataAnnotations;

namespace MusicPortal.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
        public string Salt { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsAdmin { get; set; } = false;
        public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}