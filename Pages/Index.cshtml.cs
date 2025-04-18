// Pages/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Models;
using System.Text.Json;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace MyApp.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty]
        public string SelectedColumns { get; set; } = string.Empty;
        [BindProperty]
        public string SelectedRows { get; set; } = string.Empty;

        public List<ClassInformationTable> FilteredData { get; set; } = new();
        public int TotalPages { get; set; }
        private const int PageSize = 10;

        public void OnGet()
        {
            var all = GenerateSampleData();
            var filtered = string.IsNullOrWhiteSpace(SearchTerm)
                ? all
                : all.Where(c =>
                    c.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.Department.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                     .ToList();

            TotalPages = (int)Math.Ceiling(filtered.Count / (double)PageSize);
            CurrentPage = CurrentPage < 1
                ? 1
                : (CurrentPage > TotalPages ? TotalPages : CurrentPage);

            FilteredData = filtered
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public IActionResult OnPostExportJson()
        {
            // 1) POST sırasında sayfalı+filtreli veriyi yeniden oluştur:
            var all       = GenerateSampleData();
            var filtered  = string.IsNullOrWhiteSpace(SearchTerm)
                ? all
                : all.Where(c =>
                    c.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    c.Department.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                     .ToList();
            var paged = filtered
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            // 2) Seçili sütunları ve satır ID'lerini al:
            var cols = string.IsNullOrWhiteSpace(SelectedColumns)
                ? new List<string>()
                : JsonSerializer.Deserialize<List<string>>(SelectedColumns)!;
            var rows = string.IsNullOrWhiteSpace(SelectedRows)
                ? new List<int>()
                : JsonSerializer.Deserialize<List<int>>(SelectedRows)!;

            bool noCols = cols.Count == 0;
            bool noRows = rows.Count == 0;

            // 3) Eğer satırlar seçilmişse, sadece o ID'lere sahip satırları tut:
            if (!noRows)
                paged = paged.Where(x => rows.Contains(x.ID)).ToList();

            // 4) Sonuç sözlük listesini oluştur:
            var result = paged.Select(item =>
            {
                var dict = new Dictionary<string, object?>();
                if (noCols)
                {
                    // tüm alanları ekle
                    dict["Name"]        = item.Name;
                    dict["Description"] = item.Description;
                    dict["CreatedDate"] = item.CreatedDate;
                    dict["Grade"]       = item.Grade;
                    dict["Department"]  = item.Department;
                }
                else
                {
                    // sadece seçilen sütunları ekle
                    foreach (var c in cols)
                    {
                        var prop = typeof(ClassInformationTable).GetProperty(c);
                        if (prop != null)
                            dict[c] = prop.GetValue(item);
                    }
                }
                return dict;
            }).ToList();

            // 5) JSON olarak indirme:
            var json  = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "export.json");
        }

        private List<ClassInformationTable> GenerateSampleData()
        {
            var list = new List<ClassInformationTable>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new ClassInformationTable
                {
                    ID          = i,
                    Name        = $"Student {i}",
                    Description = $"Desc {i}",
                    CreatedDate = DateTime.Now.AddDays(-i),
                    Grade       = i % 101,
                    Department  = i % 2 == 0 ? "Engineering" : "Science"
                });
            }
            return list;
        }
    }
}
