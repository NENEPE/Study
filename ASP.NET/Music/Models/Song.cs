using System.ComponentModel.DataAnnotations;

namespace MusicPortal.Models
{
    public class Song
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Artist is required")]
        [StringLength(100, ErrorMessage = "Artist name cannot exceed 100 characters")]
        public string Artist { get; set; }

        [Required(ErrorMessage = "Please select a genre")]
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }

        [Required(ErrorMessage = "Please select a file")]
        public string FilePath { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public string FileName => Path.GetFileName(FilePath);
    }
}