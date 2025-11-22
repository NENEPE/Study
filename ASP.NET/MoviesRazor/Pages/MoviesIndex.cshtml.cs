using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MoviesRazor.Models;
using System.Threading.Tasks;

namespace MoviesRazor.Pages
{
    public class MoviesIndexModel : PageModel
    {
        private readonly MovieContext _context;
        public MoviesIndexModel(MovieContext context)
        {
            _context = context;
        }
        public IList<Movie> Movie { get; set; } = default!;
        public async Task OnGetAsync()
        {
            if (_context.Movies != null)
            {
                Movie = await _context.Movies.ToListAsync();
            }
        }
    }
}
