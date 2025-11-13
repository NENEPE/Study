using System.ComponentModel.DataAnnotations;

namespace MusicPortal.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Genre name is required")]
        [StringLength(50, ErrorMessage = "Genre name cannot exceed 50 characters")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public virtual ICollection<Song> Songs { get; set; } = new List<Song>();
    }
}