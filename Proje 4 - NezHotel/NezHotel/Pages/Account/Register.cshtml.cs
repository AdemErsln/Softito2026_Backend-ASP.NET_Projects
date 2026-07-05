using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace NezHotel.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

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

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Şifreler eşleşmiyor.";
                return Page();
            }

            try
            {
                using (var conn = new SqlConnection(DatabaseHelper.ConnectionString))
                {
                    await conn.OpenAsync();
                    string insertSql = "INSERT INTO YoneticiUsers (Username, Password) VALUES (@Username, @Password)";
                    using (var cmd = new SqlCommand(insertSql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", Username);
                        cmd.Parameters.AddWithValue("@Password", Password);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return RedirectToPage("/Account/Login");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2601 || ex.Number == 2627)
                {
                    ErrorMessage = "Bu kullanıcı adı zaten alınmış.";
                }
                else
                {
                    ErrorMessage = "Kayıt sırasında bir hata oluştu: " + ex.Message;
                }
                return Page();
            }
        }
    }
}
