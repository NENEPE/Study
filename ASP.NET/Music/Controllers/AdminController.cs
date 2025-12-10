using Microsoft.AspNetCore.Mvc;
using MusicPortal.Filters;
using MusicPortal.Models;
using MusicPortal.Repositories;

namespace MusicPortal.Controllers
{
    [Culture]
    public class AdminController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IRepository<Song> _songRepository;

        public AdminController(IRepository<User> userRepository,
                             IRepository<Genre> genreRepository,
                             IRepository<Song> songRepository)
        {
            _userRepository = userRepository;
            _genreRepository = genreRepository;
            _songRepository = songRepository;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("IsAdmin") == "True";
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            HttpContext.Session.SetString("path", Request.Path);

            var stats = new
            {
                TotalUsers = await _userRepository.CountAsync(),
                TotalGenres = await _genreRepository.CountAsync(),
                TotalSongs = await _songRepository.CountAsync(),
                PendingUsers = (await _userRepository.GetWhereAsync(u => !u.IsActive)).Count()
            };

            return View(stats);
        }
        public async Task<IActionResult> Users()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            HttpContext.Session.SetString("path", Request.Path);

            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ActivateUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = await _userRepository.GetByIdAsync(id);
            if (user != null)
            {
                user.IsActive = true;
                await _userRepository.UpdateAsync(user);
                TempData["SuccessMessage"] = "User activated successfully!";
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var user = await _userRepository.GetByIdAsync(id);
            if (user != null && user.Id != 1) 
            {
                await _userRepository.DeleteAsync(user);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }

            return RedirectToAction("Users");
        }
        public async Task<IActionResult> Genres()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            HttpContext.Session.SetString("path", Request.Path);

            var genres = await _genreRepository.GetAllAsync();
            return View(genres);
        }
        public IActionResult CreateGenre()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            HttpContext.Session.SetString("path", Request.Path);
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGenre(Genre genre)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(genre);

            await _genreRepository.AddAsync(genre);
            TempData["SuccessMessage"] = "Genre created successfully!";
            return RedirectToAction("Genres");
        }
        public async Task<IActionResult> EditGenre(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            HttpContext.Session.SetString("path", Request.Path);

            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null) return NotFound();

            return View(genre);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGenre(Genre genre)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
                return View(genre);

            await _genreRepository.UpdateAsync(genre);
            TempData["SuccessMessage"] = "Genre updated successfully!";
            return RedirectToAction("Genres");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre != null)
            {
                var songsInGenre = await _songRepository.GetWhereAsync(s => s.GenreId == id);
                if (songsInGenre.Any())
                {
                    TempData["ErrorMessage"] = "Cannot delete genre that has songs. Please delete or move the songs first.";
                    return RedirectToAction("Genres");
                }

                await _genreRepository.DeleteAsync(genre);
                TempData["SuccessMessage"] = "Genre deleted successfully!";
            }

            return RedirectToAction("Genres");
        }
        public async Task<IActionResult> Songs(int? genre, int page = 1, SortState sortOrder = SortState.DateDesc)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            int pageSize = 5;

            var songsQuery = await _songRepository.GetAllAsync();
            var genres = await _genreRepository.GetAllAsync();

            if (genre != null && genre != 0)
            {
                songsQuery = songsQuery.Where(p => p.GenreId == genre);
            }

            // 4. Sort
            songsQuery = sortOrder switch
            {
                SortState.TitleAsc => songsQuery.OrderBy(s => s.Title),
                SortState.TitleDesc => songsQuery.OrderByDescending(s => s.Title),
                SortState.ArtistAsc => songsQuery.OrderBy(s => s.Artist),
                SortState.ArtistDesc => songsQuery.OrderByDescending(s => s.Artist),
                SortState.GenreAsc => songsQuery.OrderBy(s => s.Genre.Name),
                SortState.GenreDesc => songsQuery.OrderByDescending(s => s.Genre.Name),
                SortState.DateAsc => songsQuery.OrderBy(s => s.UploadDate),
                _ => songsQuery.OrderByDescending(s => s.UploadDate),
            };

            var count = songsQuery.Count();
            var items = songsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var pageViewModel = new PageViewModel(count, page, pageSize);
            var filterViewModel = new FilterViewModel(genres.ToList(), genre ?? 0);
            var sortViewModel = new SortViewModel(sortOrder);

            var viewModel = new IndexViewModel(items, pageViewModel, filterViewModel, sortViewModel);

            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSong(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");

            var song = await _songRepository.GetByIdAsync(id);
            if (song != null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", song.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);

                await _songRepository.DeleteAsync(song);
                TempData["SuccessMessage"] = "Song deleted successfully!";
            }

            return RedirectToAction("Songs");
        }
    }
}