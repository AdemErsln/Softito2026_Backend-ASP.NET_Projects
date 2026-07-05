using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages.Odalar
{
    [Authorize(AuthenticationSchemes = "CookieAuth")]
    public class IndexModel : PageModel
    {
        [BindProperty]
        public List<Odalar> Listele { get; set; } = new List<Odalar>();

        string connectionString = DatabaseHelper.ConnectionString;

        public void OnGet()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {

                    connection.Open();
                    string sql = "Select * from Odalar";
                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Odalar oda = new Odalar
                                {
                                    OdaID = reader.GetInt32(0).ToString(),
                                    OdaNo = reader.IsDBNull(1) ? "" : reader.GetInt32(1).ToString(), // Eğer OdaNo int ise GetInt32 yapmalısın
                                    OdaTipi = reader.IsDBNull(2) ? "" : reader.GetString(2),

                                    // Hatanın çözümü burası: Decimal olarak oku ve String'e çevir
                                    Fiyat = reader.IsDBNull(3) ? "" : reader.GetDecimal(3).ToString("N2")
                                };

                                Listele.Add(oda);
                            }
                        }
                        }
                   
                }
                ;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
