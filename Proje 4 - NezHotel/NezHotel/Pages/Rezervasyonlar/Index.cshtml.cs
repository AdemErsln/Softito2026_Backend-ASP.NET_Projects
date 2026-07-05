using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace NezHotel.Pages.Rezervasyonlar
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class IndexModel : PageModel
    {
        // Ön yüzde döngüye sokacağımız dinamik rezervasyon listesi sınıfı
        public List<RezervasyonGörünüm> RezervasyonlarListele { get; set; } = new List<RezervasyonGörünüm>();

        string connectionString = DatabaseHelper.ConnectionString;

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Üç tabloyu birbirine bağlayan SQL JOIN Sorgusu
                    string sql = @"SELECT r.RezervasyonID, m.AdSoyad, o.OdaNo, r.GirisTarihi, r.CikisTarihi 
                                   FROM Rezervasyonlar r
                                   INNER JOIN Musteriler m ON r.MusteriID = m.MusteriID
                                   INNER JOIN Odalar o ON r.OdaID = o.OdaID
                                   ORDER BY r.RezervasyonID DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var rez = new RezervasyonGörünüm
                                {
                                    RezervasyonID = reader.GetInt32(0).ToString(),
                                    MusteriAdSoyad = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                    OdaNo = reader.IsDBNull(2) ? "" : reader.GetInt32(2).ToString(),
                                    // Tarihleri Türkiye formatında şık görünmesi için kısa tarih formatına çeviriyoruz
                                    GirisTarihi = reader.IsDBNull(3) ? "" : reader.GetDateTime(3).ToString("dd.MM.yyyy"),
                                    CikisTarihi = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("dd.MM.yyyy")
                                };
                                RezervasyonlarListele.Add(rez);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    // Listeleme sayfasına özel hafif bir veri transfer nesnesi (DTO)
    public class RezervasyonGörünüm
    {
        public string RezervasyonID { get; set; }
        public string MusteriAdSoyad { get; set; }
        public string OdaNo { get; set; }
        public string GirisTarihi { get; set; }
        public string CikisTarihi { get; set; }
    }
}