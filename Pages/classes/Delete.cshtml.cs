using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;

namespace MyApp.Pages.Classes
{
    public class DeleteModel : PageModel
    {
        private readonly SchoolDbContext _context;
        public DeleteModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class Class { get; set; } = new();

        // OnGet ile silinecek kaydı getiriyoruz (yalnızca görüntüleme için)
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Class = await _context.Classes.FindAsync(id);
            if (Class == null)
                return NotFound();
            return Page();
        }

        // OnPost ile gerçek silme yerine IsActive = false yapıyoruz
        public async Task<IActionResult> OnPostAsync(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item == null)
                return NotFound();

            // Soft delete:
            item.IsActive = false;
            // EF Core'a güncellendiğini bildir:
            _context.Entry(item).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            // Ana liste sayfasına dön:
            return RedirectToPage("/Index");
        }
    }
}
