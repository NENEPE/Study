using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MusicPortal.Models
{
    public class SongViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Artist is required")]
        [StringLength(100, ErrorMessage = "Artist name cannot exceed 100 characters")]
        public string Artist { get; set; }

        [Required(ErrorMessage = "Please select a genre")]
        [Display(Name = "Genre")]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Please select a music file")]
        [Display(Name = "Music File")]
        public IFormFile MusicFile { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public List<SelectListItem> Genres { get; set; } = new List<SelectListItem>();
    }
}