using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages.Odalar
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class DuzenleModel : PageModel
    {
        // Form alanlarını çift yönlü bağlamak ve içini doldurmak için property'ler
        [BindProperty]
        public string OdaID { get; set; }

        [BindProperty]
        public string OdaNo { get; set; }

        [BindProperty]
        public string OdaTipi { get; set; }

        [BindProperty]
        public string Fiyat { get; set; }

        public string Mesaj { get; set; }

        string connectionString = DatabaseHelper.ConnectionString;

        // Sayfa URL'den gelen ID ile açıldığında mevcut bilgileri yükler
        public void OnGet(int id)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Odalar WHERE OdaID = @OdaID";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@OdaID", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Veritabanındaki mevcut değerleri form elementlerine aktarıyoruz
                                OdaID = reader.GetInt32(0).ToString();
                                OdaNo = reader.IsDBNull(1) ? "" : reader.GetInt32(1).ToString();
                                OdaTipi = reader.IsDBNull(2) ? "" : reader.GetString(2);
                                Fiyat = reader.IsDBNull(3) ? "" : reader.GetDecimal(3).ToString("F2"); // input içinde rahat düzenlensin diye düz decimal formatı (Örn: 2500.00)
                            }
                            else
                            {
                                Mesaj = "Aranan oda bulunamadı.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Mesaj = "Veri yüklenirken hata oluştu: " + ex.Message;
            }
        }

        // Kullanıcı "Kaydet" butonuna bastığında değişiklikleri veritabanına işler
        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(OdaNo) || string.IsNullOrEmpty(OdaTipi) || string.IsNullOrEmpty(Fiyat))
            {
                Mesaj = "Lütfen tüm alanları doldurunuz.";
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE odalar SET OdaNo = @OdaNo, OdaTipi = @OdaTipi, Fiyat = @Fiyat WHERE OdaID = @OdaID";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@OdaNo", OdaNo);
                        cmd.Parameters.AddWithValue("@OdaTipi", OdaTipi);
                        cmd.Parameters.AddWithValue("@Fiyat", Fiyat);
                        cmd.Parameters.AddWithValue("@OdaID", OdaID);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Güncelleme başarılıysa listeye geri gönder
                return RedirectToPage("/Odalar/Index");
            }
            catch (Exception ex)
            {
                Mesaj = "Güncelleme sırasında hata oluştu: " + ex.Message;
                return Page();
            }
        }
    }
}