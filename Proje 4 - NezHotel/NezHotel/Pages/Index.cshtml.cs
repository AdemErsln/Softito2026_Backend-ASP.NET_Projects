using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class IndexModel : PageModel
    {
        // Ön yüzde göstereceğimiz sayaç değişkenleri
        public int ToplamOdaSayisi { get; set; } = 0;
        public int ToplamMusteriSayisi { get; set; } = 0;
        public int ToplamRezervasyonSayisi { get; set; } = 0;

        string connectionString = DatabaseHelper.ConnectionString;

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // 1. Toplam Oda Sayısını Çek
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Odalar", connection))
                    {
                        ToplamOdaSayisi = (int)cmd.ExecuteScalar();
                    }

                    // 2. Toplam Müşteri Sayısını Çek
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Musteriler", connection))
                    {
                        ToplamMusteriSayisi = (int)cmd.ExecuteScalar();
                    }

                    // 3. Toplam Rezervasyon Sayısını Çek
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Rezervasyonlar", connection))
                    {
                        ToplamRezervasyonSayisi = (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception)
            {
                // Veritabanı boşsa veya bağlantı hatası varsa sayaçlar 0 kalır
            }
        }
    }
}