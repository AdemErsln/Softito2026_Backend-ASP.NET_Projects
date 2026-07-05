using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace NezHotel.Pages.Rezervasyonlar
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
                    string sql = "DELETE FROM Rezervasyonlar WHERE RezervasyonID = @RezID";

                    using (SqlCommand cmd = new SqlCommand(sql, connection))
                    {
                        cmd.Parameters.AddWithValue("@RezID", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception) { }

            return RedirectToPage("/Rezervasyonlar/Index");
        }
    }
}