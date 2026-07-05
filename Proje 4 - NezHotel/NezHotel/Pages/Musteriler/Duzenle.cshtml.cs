using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages.Musteriler
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class DuzenleModel : PageModel
    {
        [BindProperty] public string MusteriID { get; set; }
        [BindProperty] public string AdSoyad { get; set; }
        [BindProperty] public string Telefon { get; set; }
        [BindProperty] public string Email { get; set; }
        public string Mesaj { get; set; }

        string connectionString = DatabaseHelper.ConnectionString;

        public void OnGet(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Musteriler WHERE MusteriID = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                MusteriID = reader.GetInt32(0).ToString();
                                AdSoyad = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                Telefon = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                Email = reader.IsDBNull(3) ? "" : reader.GetString(3);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { Mesaj = "Veri alınamadı: " + ex.Message; }
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(AdSoyad))
            {
                Mesaj = "Ad Soyad alanı boş geçilemez.";
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Musteriler SET AdSoyad=@AdSoyad, Telefon=@Telefon, Email=@Email WHERE MusteriID=@MusteriID";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@AdSoyad", AdSoyad);
                        cmd.Parameters.AddWithValue("@Telefon", Telefon ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Email", Email ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@MusteriID", Convert.ToInt32(MusteriID));

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToPage("/Musteriler/Index");
            }
            catch (Exception ex) { Mesaj = "Güncellenirken hata oluştu: " + ex.Message; return Page(); }
        }
    }
}