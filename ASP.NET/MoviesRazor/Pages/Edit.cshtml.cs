using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MoviesRazor.Models;

namespace MoviesRazor.Pages
{
    public class EditModel : PageModel
    {
        private readonly MoviesRazor.Models.MovieContext _context;
        private readonly IWebHostEnvironment ae;

        public EditModel(MoviesRazor.Models.MovieContext context, IWebHostEnvironment web)
        {
            _context = context;
            ae = web;
        }

        [BindProperty]
        public Movie Movie { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var movie =  await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            Movie = movie;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id, [Bind("Id,Title,Genre,ReleaseYear,Director,Desc,Poster")] Movie movie, IFormFile file)
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
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
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
            }
            return RedirectToPage("./MoviesIndex");
        }


        private bool MovieExists(int id)
        {
            return _context.Movies.Any(e => e.Id == id);
        }
    }
}
