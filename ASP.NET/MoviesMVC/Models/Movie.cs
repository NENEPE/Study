using MoviesMVC.Annotations;
using System.ComponentModel.DataAnnotations;

namespace MoviesMVC.Models
{
    public class Movie
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Director's full name is required")]
        public string? Director { get; set; }
        [Required(ErrorMessage = "Release year is required")]
        [Display(Name = "Release Year")]
        [Range(1905, 2025, ErrorMessage = "Enter a valid year (1905-2025)")]
        public int ReleaseYear { get; set; }
        [Required(ErrorMessage = "Genre is required")]
        [AllowedGenres(["Action", "Adventure", "Animation", "Buddy Comedy", "Comedy", "Cyber Thriller", "Dark Comedy", "Docudrama", "Fairy Tale Comedy", "Heist", "Horror", "Body Horror", "Science fiction", "Superhero fiction" ], ErrorMessage = "Invalid genre")]
        public string? Genre { get; set; }
        [Required(ErrorMessage = "Description is required")]
        [MinLength(10, ErrorMessage = "Description must be at least 10 characters long")]
        public string? Desc { get; set; }
        public string? Poster { get; set; }
    }
}
