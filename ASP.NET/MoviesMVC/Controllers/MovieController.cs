using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesMVC.Models;

namespace MoviesMVC.Controllers
{
    public class MovieController : Controller
    {
        MovieContext db;
        IWebHostEnvironment ae;
        public MovieController(MovieContext context, IWebHostEnvironment appEnvironment)
        {
            db = context;
            ae = appEnvironment;
        }
        public async Task<IActionResult> Index() => View(await db.Movies.ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            Movie movie = await db.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            Movie movie = await db.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await db.Movies.FindAsync(id);
            if (movie != null)
            {
                db.Movies.Remove(movie);
            }

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1000000000)]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Title,Genre,ReleaseYear,Director,Desc,Poster")] Movie movie)
        {
            if (file == null)
            {
                ModelState.AddModelError("", "Poster is required");
            }

            else if (file.Length > 10485760)
            {
                ModelState.AddModelError("", "Poster size must be less than 10 MB");
            }
            else if (!file.FileName.EndsWith(".png") && !file.FileName.EndsWith(".jpg") && !file.FileName.EndsWith(".gif"))
            {
                ModelState.AddModelError("", "Invalid format of a poster");
            }

            if (ModelState.IsValid)
            {
                string path = "\\img\\" + file.FileName;

                using (var fileStream = new FileStream(ae.WebRootPath + path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                movie.Poster = path;

                db.Add(movie);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            Movie movie = await db.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1000000000)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,ReleaseYear,Director,Desc,Poster")] Movie movie, IFormFile file)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (file == null)
            {
                ModelState.Remove("file");
            }
            else if (file.Length > 10485760)
            {
                ModelState.AddModelError("", "Poster size must be less than 10 MB");
            }
            else if (!file.FileName.EndsWith(".png") && !file.FileName.EndsWith(".jpg") && !file.FileName.EndsWith(".gif"))
            {
                ModelState.AddModelError("", "Invalid format of a poster");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null && file.Length > 0)
                    {
                        string path = "\\img\\" + file.FileName;

                        using (var fileStream = new FileStream(Path.Combine(ae.WebRootPath, path.TrimStart('\\')), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        movie.Poster = path;
                    }
                    db.Update(movie);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }
        private bool MovieExists(int id)
        {
            return db.Movies.Any(e => e.Id == id);
        }
    }
}
