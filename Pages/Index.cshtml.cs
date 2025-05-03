using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Models;
using System.Text.Json;
using System.Text;

namespace MyApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;
        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty]
        public string SelectedColumns { get; set; } = string.Empty;

        [BindProperty]
        public string SelectedRows { get; set; } = string.Empty;

        [BindProperty]
        public Class NewClass { get; set; } = new();

        public List<Class> FilteredData { get; set; } = new();
        public int TotalPages { get; set; }
        private const int PageSize = 10;

        public async Task OnGetAsync()
        {
            // Sadece aktif kayıtları getir
            var query = _context.Classes
                                .Where(c => c.IsActive)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(SearchTerm) ||
                    c.Description.Contains(SearchTerm));
            }

            int totalCount = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            if (TotalPages > 0)
                CurrentPage = Math.Clamp(CurrentPage, 1, TotalPages);
            else
                CurrentPage = 1;

            FilteredData = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Yeni eklenen her sınıf aktif olsun
            NewClass.IsActive = true;
            _context.Classes.Add(NewClass);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportJsonAsync()
        {
            // JSON export da sadece aktifleri alsın
            var query = _context.Classes
                                .Where(c => c.IsActive)
                                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(SearchTerm) ||
                    c.Description.Contains(SearchTerm));
            }

            var paged = await query
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var cols = string.IsNullOrWhiteSpace(SelectedColumns)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(SelectedColumns)!;

            var rows = string.IsNullOrWhiteSpace(SelectedRows)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(SelectedRows)!;

            if (rows.Count > 0)
                paged = paged.Where(x => rows.Contains(x.Id)).ToList();

            bool noCols = cols.Count == 0;
            var result = paged.Select(item =>
            {
                var dict = new Dictionary<string, object?>();
                if (noCols)
                {
                    dict["Name"]        = item.Name;
                    dict["PersonCount"] = item.PersonCount;
                    dict["Description"] = item.Description;
                    // IsActive artık hepsi true, böylece JSON’da da eklemek gerekmez
                }
                else
                {
                    foreach (var c in cols)
                    {
                        var prop = typeof(Class).GetProperty(c);
                        if (prop != null)
                            dict[c] = prop.GetValue(item);
                    }
                }
                return dict;
            }).ToList();

            var json  = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "export.json");
        }
    }
}
