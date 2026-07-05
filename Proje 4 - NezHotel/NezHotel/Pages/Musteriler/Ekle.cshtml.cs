using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages.Musteriler
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class EkleModel : PageModel
    {
        [BindProperty] public string AdSoyad { get; set; }
        [BindProperty] public string Telefon { get; set; }
        [BindProperty] public string Email { get; set; }
        public string Mesaj { get; set; }

        string connectionString = DatabaseHelper.ConnectionString;

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(AdSoyad))
            {
                Mesaj = "Müşteri adı ve soyadı alanı zorunludur.";
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Musteriler (AdSoyad, Telefon, Email) VALUES (@AdSoyad, @Telefon, @Email)";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@AdSoyad", AdSoyad);
                        cmd.Parameters.AddWithValue("@Telefon", Telefon ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", Email ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToPage("/Musteriler/Index");
            }
            catch (Exception ex)
            {
                Mesaj = "Kaydedilirken bir hata oluştu: " + ex.Message;
                return Page();
            }
        }
    }
}