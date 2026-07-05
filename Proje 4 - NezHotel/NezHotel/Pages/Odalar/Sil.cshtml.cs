using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages.Odalar
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class SilModel : PageModel
    {
        string connectionString = DatabaseHelper.ConnectionString;

        // Sayfaya GET isteği atıldığı anda (yani linke tıklandığında) çalışır
        public IActionResult OnGet(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM odalar WHERE OdaID = @OdaID";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@OdaID", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                // İleride buraya hata günlüğü (Log) veya kullanıcıya göstermek için TempData mesajı eklenebilir.
                // Örneğin: TempData["Hata"] = "Bu oda rezervasyonlarda kullanıldığı için silinemez!";
            }

            // Silme işlemi bitince kullanıcıyı anında oda listesine geri gönderiyoruz
            return RedirectToPage("/Odalar/Index");
        }
    }
}