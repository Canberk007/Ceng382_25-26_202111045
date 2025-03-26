using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyApp.Models;

namespace MyApp.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();
        
        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new();

        [BindProperty]
        public bool IsEditMode { get; set; } = false;

        public void OnGet() { }

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
    }
}