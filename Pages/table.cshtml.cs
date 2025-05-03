using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using MyApp.Models;

namespace MyApp.Pages
{
    public class TableModel : PageModel
    {
        public List<User> KullaniciListesi { get; set; } = new();

        public void OnGet()
        {
            var usersJson = System.IO.File.ReadAllText("wwwroot/data/users.json");
            KullaniciListesi = JsonSerializer.Deserialize<List<User>>(usersJson) ?? new List<User>();
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            var sessionUsername = HttpContext.Session.GetString("username");
            var sessionToken = HttpContext.Session.GetString("token");
            var sessionId = HttpContext.Session.GetString("session_id");

            var cookieUsername = HttpContext.Request.Cookies["username"];
            var cookieToken = HttpContext.Request.Cookies["token"];
            var cookieSessionId = HttpContext.Request.Cookies["session_id"];

            if (string.IsNullOrEmpty(sessionUsername) || string.IsNullOrEmpty(sessionToken) || string.IsNullOrEmpty(sessionId)
                || string.IsNullOrEmpty(cookieUsername) || string.IsNullOrEmpty(cookieToken) || string.IsNullOrEmpty(cookieSessionId)
                || sessionUsername != cookieUsername
                || sessionToken != cookieToken
                || sessionId != cookieSessionId)
            {
                context.Result = RedirectToPage("/Login");
            }

            base.OnPageHandlerExecuting(context);
        }
    }
}
