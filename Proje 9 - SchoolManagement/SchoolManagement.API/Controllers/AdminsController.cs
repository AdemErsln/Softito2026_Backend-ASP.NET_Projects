using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminsController : ControllerBase
    {
        private readonly string _connectionString;

        public AdminsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration), "DefaultConnection string is missing");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Admins WHERE Username = @Username AND Password = @Password";
                var admin = await db.QueryFirstOrDefaultAsync<Admin>(sql, request);
                if (admin == null)
                    return Unauthorized("Kullanıcı adı veya şifre hatalı.");

                return Ok(new { admin.ID, admin.Username });
            }
        }
    }
}
