using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MusicPortal.Filters;
using MusicPortal.Models;
using MusicPortal.Repositories;

namespace MusicPortal.Controllers
{
    [Culture]
    public class SongController : Controller
    {
        private readonly IRepository<Song> _songRepository;
        private readonly IRepository<Genre> _genreRepository;
        private readonly IWebHostEnvironment _environment;

        public SongController(IRepository<Song> songRepository,
                            IRepository<Genre> genreRepository,
                            IWebHostEnvironment environment)
        {
            _songRepository = songRepository;
            _genreRepository = genreRepository;
            _environment = environment;
        }

        public async Task<IActionResult> Create()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Login", "Account");

            HttpContext.Session.SetString("path", Request.Path);

            var model = new SongViewModel
            {
                Genres = (await _genreRepository.GetAllAsync())
                        .Select(g => new SelectListItem
                        {
                            Value = g.Id.ToString(),
                            Text = g.Name
                        }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SongViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Genres = (await _genreRepository.GetAllAsync())
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    }).ToList();
                return View(model);
            }

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
                return RedirectToAction("Login", "Account");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.MusicFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.MusicFile.CopyToAsync(stream);
            }

            var song = new Song
            {
                Title = model.Title,
                Artist = model.Artist,
                GenreId = model.GenreId,
                FilePath = $"/uploads/{fileName}",
                Description = model.Description,
                UserId = int.Parse(HttpContext.Session.GetString("UserId")),
                UploadDate = DateTime.UtcNow,
            };

            await _songRepository.AddAsync(song);
            TempData["SuccessMessage"] = "Song uploaded successfully!";
            return RedirectToAction("Index", "Home");
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
    }
}