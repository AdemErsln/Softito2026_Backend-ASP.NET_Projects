using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages.Odalar
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class EkleModel : PageModel
    {
        // Form alanlarını arkaya bağlamak için property'ler tanımlıyoruz
        [BindProperty]
        public string OdaNo { get; set; }

        [BindProperty]
        public string OdaTipi { get; set; }

        [BindProperty]
        public string Fiyat { get; set; }

        // Sayfada göstereceğimiz olası hata/başarı mesajları için
        public string Mesaj { get; set; }

        string connectionString = DatabaseHelper.ConnectionString;

        public void OnGet()
        {
            // Sayfa ilk açıldığında yapılacak bir işlem yok
        }

        public IActionResult OnPost()
        {
            // Basit bir validasyon kontrolü
            if (string.IsNullOrEmpty(OdaNo) || string.IsNullOrEmpty(OdaTipi) || string.IsNullOrEmpty(Fiyat))
            {
                Mesaj = "Lütfen tüm alanları eksiksiz doldurunuz.";
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // SQL Injection'ı önlemek için parametreli sorgu kullanıyoruz
                    string sql = "INSERT INTO odalar (OdaNo, OdaTipi, Fiyat) VALUES (@OdaNo, @OdaTipi, @Fiyat)";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        // Sizin modelinizdeki string yapısına göre ekleme yapıyoruz
                        cmd.Parameters.AddWithValue("@OdaNo", OdaNo);
                        cmd.Parameters.AddWithValue("@OdaTipi", OdaTipi);
                        cmd.Parameters.AddWithValue("@Fiyat", Fiyat);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Ekleme işlemi başarılıysa kullanıcılıyı tekrar listeye yönlendiriyoruz
                return RedirectToPage("/Odalar/Index");
            }
            catch (Exception ex)
            {
                // Bir hata oluşursa sayfada gösteriyoruz
                Mesaj = "Hata oluştu: " + ex.Message;
                return Page();
            }
        }
    }
}