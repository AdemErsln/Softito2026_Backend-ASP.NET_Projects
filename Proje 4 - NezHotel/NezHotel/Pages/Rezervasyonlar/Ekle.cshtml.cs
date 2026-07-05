using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages.Rezervasyonlar
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class EkleModel : PageModel
    {
        // Form post edildiğinde yakalanacak alanlar
        [BindProperty] public string SecilenMusteriID { get; set; }
        [BindProperty] public string SecilenOdaID { get; set; }
        [BindProperty] public DateTime GirisTarihi { get; set; } = DateTime.Now;
        [BindProperty] public DateTime CikisTarihi { get; set; } = DateTime.Now.AddDays(1);

        // Açılır kutuları (Dropdown) doldurmak için listeler
        public List<DdlItem> MusteriListesi { get; set; } = new List<DdlItem>();
        public List<DdlItem> OdaListesi { get; set; } = new List<DdlItem>();
        public string Mesaj { get; set; }

        string connectionString = DatabaseHelper.ConnectionString;

        // Sayfa ilk yüklenirken dropdown elemanlarını doldurur
        public void OnGet()
        {
            DropdownlariDoldur();
        }

        public IActionResult OnPost()
        {
            if (string.IsNullOrEmpty(SecilenMusteriID) || string.IsNullOrEmpty(SecilenOdaID))
            {
                Mesaj = "Lütfen müşteri ve oda seçimini yapınız.";
                DropdownlariDoldur(); // Sayfa post edildiğinde listelerin boşalmaması için tekrar dolduruyoruz
                return Page();
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "INSERT INTO Rezervasyonlar (MusteriID, OdaID, GirisTarihi, CikisTarihi) VALUES (@MusteriID, @OdaID, @Giris, @Cikis)";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@MusteriID", Convert.ToInt32(SecilenMusteriID));
                        cmd.Parameters.AddWithValue("@OdaID", Convert.ToInt32(SecilenOdaID));
                        cmd.Parameters.AddWithValue("@Giris", GirisTarihi);
                        cmd.Parameters.AddWithValue("@Cikis", CikisTarihi);

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToPage("/Rezervasyonlar/Index");
            }
            catch (Exception ex)
            {
                Mesaj = "Kaydedilirken hata oluştu: " + ex.Message;
                DropdownlariDoldur();
                return Page();
            }
        }

        // ADO.NET ile müşterileri ve odaları çeken yardımcı metot
        private void DropdownlariDoldur()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // 1. Müşterileri Çek
                using (SqlCommand cmd = new SqlCommand("SELECT MusteriID, AdSoyad FROM Musteriler ORDER BY AdSoyad ASC", connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MusteriListesi.Add(new DdlItem { Value = reader.GetInt32(0).ToString(), Text = reader.GetString(1) });
                    }
                }

                // 2. Odaları Çek
                using (SqlCommand cmd = new SqlCommand("SELECT OdaID, OdaNo FROM Odalar ORDER BY OdaNo ASC", connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        OdaListesi.Add(new DdlItem { Value = reader.GetInt32(0).ToString(), Text = "Oda No: " + reader.GetInt32(1).ToString() });
                    }
                }
            }
        }
    }

    // Dropdown eleman yapısı için basit sınıf
    public class DdlItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }
}