namespace MusicPortal.Models
{
    public class SortViewModel
    {
        public SortState TitleSort { get; set; }
        public SortState ArtistSort { get; set; }
        public SortState GenreSort { get; set; }
        public SortState DateSort { get; set; }
        public SortState Current { get; set; }
        public bool Up { get; set; }

        public SortViewModel(SortState sortOrder)
        {
            // значения по умолчанию
            TitleSort = SortState.TitleAsc;
            ArtistSort = SortState.ArtistAsc;
            GenreSort = SortState.GenreAsc;
            DateSort = SortState.DateAsc;

            TitleSort = sortOrder == SortState.TitleAsc ? SortState.TitleDesc : SortState.TitleAsc;
            ArtistSort = sortOrder == SortState.ArtistAsc ? SortState.ArtistDesc : SortState.ArtistAsc;
            GenreSort = sortOrder == SortState.GenreAsc ? SortState.GenreDesc : SortState.GenreAsc;
            DateSort = sortOrder == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;
            Current = sortOrder;
        }
    }
}
