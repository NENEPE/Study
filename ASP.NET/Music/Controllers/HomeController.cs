using Microsoft.AspNetCore.Mvc;
using MusicPortal.Filters;
using MusicPortal.Models;
using MusicPortal.Repositories;
using MusicPortal.Services;

namespace MusicPortal.Controllers
{
    [Culture]
    public class HomeController : Controller
    {
        private readonly IRepository<Song> _songRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly ILangRead _langRead;
        private readonly IAuthService _authService;

        public HomeController(IRepository<Song> songRepository,
                            IRepository<Genre> genreRepository,
                            IRepository<User> userRepository,
                            IWebHostEnvironment environment,
                            ILangRead langRead,
                            IAuthService authService
                            )
        {
            _songRepository = songRepository;
            _genreRepository = genreRepository;
            _userRepository = userRepository;
            _environment = environment;
            _langRead = langRead;
            _authService = authService;
        }

        public async Task<IActionResult> Index(int? genre, int page = 1, SortState sortOrder = SortState.DateDesc)
        {
            int pageSize = 2;

            if (Request.Cookies["login"] != null && string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
            {
                string username = Request.Cookies["login"]!;
                var user = await _userRepository.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    _authService.UserSet(user);
                }
            }
            ;

            HttpContext.Session.SetString("path", Request.Path);

            var songs = await _songRepository.GetAllAsync();
            var genres = await _genreRepository.GetAllAsync();

            if (genre != null && genre != 0)
            {
                songs = songs.Where(p => p.GenreId == genre);
            }

            songs = sortOrder switch
            {
                SortState.TitleAsc => songs.OrderBy(s => s.Title),
                SortState.TitleDesc => songs.OrderByDescending(s => s.Title),
                SortState.ArtistAsc => songs.OrderBy(s => s.Artist),
                SortState.ArtistDesc => songs.OrderByDescending(s => s.Artist),
                SortState.GenreAsc => songs.OrderBy(s => s.Genre.Name),
                SortState.GenreDesc => songs.OrderByDescending(s => s.Genre.Name),
                SortState.DateAsc => songs.OrderBy(s => s.UploadDate),
                SortState.DateDesc => songs.OrderByDescending(s => s.UploadDate),
                _ => songs.OrderByDescending(s => s.UploadDate),
            };

            var count = songs.Count();
            var items = songs.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var filterViewModel = new FilterViewModel(genres.ToList(), genre ?? 0);
            var sortViewModel = new SortViewModel(sortOrder);
            var pageViewModel = new PageViewModel(count, page, pageSize);


            var viewModel = new IndexViewModel(
                items,
                pageViewModel,
                filterViewModel,
                sortViewModel
            );

            return View(viewModel);
        }

        public async Task<IActionResult> Download(int id)
        {
            var song = await _songRepository.GetByIdAsync(id);
            if (song == null)
                return NotFound();

            var filePath = Path.Combine(_environment.WebRootPath, song.FilePath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

            return File(fileBytes, "audio/mpeg", song.Title + ".mp3");
        }

        public ActionResult ChangeCulture(string lang)
        {
            string? returnUrl = HttpContext.Session.GetString("path") ?? "/Home/Index";

            List<string> cultures = _langRead.languageList().Select(t => t.ShortName).ToList()!;
            if (!cultures.Contains(lang))
            {
                lang = "uk";
            }

            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(10);
            Response.Cookies.Append("lang", lang, option);
            return Redirect(returnUrl);
        }
    }
}