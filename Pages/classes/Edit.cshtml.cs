using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;

namespace MyApp.Pages.Classes
{
    public class EditModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public EditModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class Class { get; set; } = new();

        // GET: sayfayı açarken var olan Class’ı getiriyoruz
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var existing = await _context.Classes.FindAsync(id);
            if (existing == null)
                return NotFound();

            Class = existing;
            return Page();
        }

        // POST: form gönderildiğinde çalışır
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Geçici key hatasını önlemek için ID’nin doğru geldiğini garanti ettik (hidden input).
            // Ayrıca IsActive’ın edit ekranında değişmeden kalmasını sağlıyoruz:
            var original = await _context.Classes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == Class.Id);
            if (original == null)
                return NotFound();

            Class.IsActive = original.IsActive;

            // Güncelle
            _context.Update(Class);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
