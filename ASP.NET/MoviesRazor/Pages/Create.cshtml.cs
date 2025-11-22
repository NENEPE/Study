using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoviesRazor.Models;

namespace MoviesRazor.Pages
{
    public class CreateModel : PageModel
    {
        private readonly MoviesRazor.Models.MovieContext _context;
        private readonly IWebHostEnvironment ae;

        public CreateModel(MoviesRazor.Models.MovieContext context, IWebHostEnvironment web)
        {
            _context = context;
            ae = web;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Movie Movie { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile file, [Bind("Title,Genre,ReleaseYear,Director,Desc,Poster")] Movie movie)
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

                _context.Add(movie);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./MoviesIndex");
        }
    }
}
