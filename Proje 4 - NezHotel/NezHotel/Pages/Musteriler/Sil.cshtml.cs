using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages.Musteriler
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class SilModel : PageModel
    {
        string connectionString = DatabaseHelper.ConnectionString;

        public IActionResult OnGet(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Musteriler WHERE MusteriID = @id";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // NOT: Eğer bu müşteri bir rezervasyona bağlıysa SQL yabancı anahtar (FK) hatası verecektir. 
                // Gerçek projelerde bu hatayı yakalayıp ekrana "Müşterinin rezervasyonu var, silinemez!" uyarısı basılabilir.
            }

            return RedirectToPage("/Musteriler/Index");
        }
    }
}