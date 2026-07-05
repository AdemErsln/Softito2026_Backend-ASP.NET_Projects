using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NezHotel.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Lütfen tüm alanları doldurun.";
                return Page();
            }

            string dbPassword = null;
            using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
            {
                await conn.OpenAsync();
                string sql = "SELECT Password FROM YoneticiUsers WHERE Username = @Username";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", Username);
                    var val = await cmd.ExecuteScalarAsync();
                    if (val != null) dbPassword = val.ToString();
                }
            }

            if (dbPassword != null && dbPassword == Password)
            {
                var claims = new List<Claim> { new Claim(ClaimTypes.Name, Username) };
                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal);
                return RedirectToPage("/Index");
            }

            ErrorMessage = "Geçersiz kullanıcı adı veya şifre.";
            return Page();
        }
    }
}
