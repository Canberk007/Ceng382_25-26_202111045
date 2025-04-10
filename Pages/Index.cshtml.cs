using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Models;

namespace MyApp.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> Classes { get; set; } = new();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new();

        [BindProperty]
        public bool IsEditMode { get; set; } = false;

        // ✔️ Filtre & Sayfalama için
        [BindProperty(SupportsGet = true)]
        public string? ClassNameFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int TotalPages { get; set; }
        public List<ClassInformationTable> FilteredData { get; set; } = new();

        public void OnGet()
        {
            // Sadece ilk kez sahte veri oluştur
            if (!Classes.Any())
            {
                Classes = GenerateSampleData();
            }

            var data = Classes;

            // 🔍 Filtreleme
            if (!string.IsNullOrEmpty(ClassNameFilter))
            {
                data = data.Where(x => x.ClassName.Contains(ClassNameFilter, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            // 📄 Sayfalama
            int pageSize = 10;
            TotalPages = (int)Math.Ceiling(data.Count / (double)pageSize);

            var pagedData = data
                .Skip((CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // ID gizli tutulacak modelle dönüştür
            FilteredData = pagedData.Select(x => new ClassInformationTable
            {
                Id = x.Id,
                ClassName = x.ClassName,
                StudentCount = x.StudentCount,
                Description = x.Description
            }).ToList();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewClass.Id = Classes.Count + 1;
            Classes.Add(NewClass);
            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToRemove = Classes.FirstOrDefault(c => c.Id == id);
            if (classToRemove != null)
            {
                Classes.Remove(classToRemove);
            }
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            var classToEdit = Classes.FirstOrDefault(c => c.Id == NewClass.Id);
            if (classToEdit != null)
            {
                classToEdit.ClassName = NewClass.ClassName;
                classToEdit.StudentCount = NewClass.StudentCount;
                classToEdit.Description = NewClass.Description;
            }
            IsEditMode = false;
            return RedirectToPage();
        }

        public IActionResult OnPostEditSelect(int id)
        {
            var classToEdit = Classes.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                NewClass = new ClassInformationModel
                {
                    Id = classToEdit.Id,
                    ClassName = classToEdit.ClassName,
                    StudentCount = classToEdit.StudentCount,
                    Description = classToEdit.Description
                };
                IsEditMode = true;
            }
            return Page();
        }

        private List<ClassInformationModel> GenerateSampleData()
        {
            var list = new List<ClassInformationModel>();
            for (int i = 1; i <= 100; i++)
            {
                list.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = "Class " + i,
                    StudentCount = 20 + (i % 15),
                    Description = (i % 2 == 0) ? "Lab Section" : "Theory Section"
                });
            }
            return list;
        }
    }
}
