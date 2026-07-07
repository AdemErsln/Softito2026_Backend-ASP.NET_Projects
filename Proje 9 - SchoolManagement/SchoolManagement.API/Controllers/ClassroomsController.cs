using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassroomsController : ControllerBase
    {
        private readonly string _connectionString;

        public ClassroomsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration), "DefaultConnection string is missing");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT c.ID, c.Name, c.Capacity, c.Location, 
                            (SELECT COUNT(1) FROM Students s WHERE s.ClassroomId = c.ID) AS StudentCount 
                            FROM Classrooms c";
                var classrooms = await db.QueryAsync<Classroom>(sql);
                return Ok(classrooms);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Classrooms WHERE ID = @id";
                var classroom = await db.QueryFirstOrDefaultAsync<Classroom>(sql, new { id });
                if (classroom == null)
                    return NotFound();

                return Ok(classroom);
            }
        }

        [HttpGet("{id}/students")]
        public async Task<IActionResult> GetClassroomStudents(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT s.*, c.Name AS ClassroomName 
                            FROM Students s 
                            LEFT JOIN Classrooms c ON s.ClassroomId = c.ID 
                            WHERE s.ClassroomId = @id";
                var students = await db.QueryAsync<Student>(sql, new { id });
                return Ok(students);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Classroom classroom)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"INSERT INTO Classrooms (Name, Capacity, Location) 
                            VALUES (@Name, @Capacity, @Location);
                            SELECT CAST(SCOPE_IDENTITY() as int);";
                var id = await db.QuerySingleAsync<int>(sql, classroom);
                classroom.ID = id;
                return CreatedAtAction(nameof(GetById), new { id }, classroom);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Classroom classroom)
        {
            if (id != classroom.ID)
                return BadRequest("ID mismatch");

            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"UPDATE Classrooms 
                            SET Name = @Name, Capacity = @Capacity, Location = @Location 
                            WHERE ID = @ID";
                var rowsAffected = await db.ExecuteAsync(sql, classroom);
                if (rowsAffected == 0)
                    return NotFound();

                return NoContent();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "DELETE FROM Classrooms WHERE ID = @id";
                var rowsAffected = await db.ExecuteAsync(sql, new { id });
                if (rowsAffected == 0)
                    return NotFound();

                return NoContent();
            }
        }
    }
}
