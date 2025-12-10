using Microsoft.AspNetCore.Mvc.Rendering;

namespace MusicPortal.Models
{
    public class FilterViewModel
    {
        public FilterViewModel(List<Genre> genres, int genre)
        {
            genres.Insert(0, new Genre { Name = "All", Id = 0 });
            Genres = new SelectList(genres, "Id", "Name", genre);
            SelectedGenre = genre;
        }
        public SelectList Genres { get; }
        public int SelectedGenre { get; }
    }
}
