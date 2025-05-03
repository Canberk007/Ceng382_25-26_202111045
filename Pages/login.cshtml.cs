using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using MyApp.Models;

namespace MyApp.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string InputUsername { get; set; } = "";

        [BindProperty]
        public string InputPassword { get; set; } = "";

        public string? SessionId { get; set; }
        public string? Token { get; set; }
        public bool LoginBasarili { get; set; } = false;

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            var usersJson = System.IO.File.ReadAllText("wwwroot/data/users.json");
            var users = JsonSerializer.Deserialize<List<User>>(usersJson) ?? new List<User>();

            var user = users.FirstOrDefault(u => 
                u.Username == InputUsername && 
                u.Password == InputPassword &&
                u.IsActive);

            if (user != null)
            {
                var token = Guid.NewGuid().ToString();

                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

                LoginBasarili = true;
                SessionId = HttpContext.Session.Id;
                Token = token;

                return Page(); // Giriş başarılı → tekrar bu sayfada kal
            }
            else
            {
                LoginBasarili = false;
                return Page();
            }
        }
    }
}



