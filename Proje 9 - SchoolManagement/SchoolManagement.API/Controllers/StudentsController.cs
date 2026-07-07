using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Dapper;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly string _connectionString;

        public StudentsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentNullException(nameof(configuration), "DefaultConnection string is missing");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT s.*, c.Name AS ClassroomName 
                            FROM Students s 
                            LEFT JOIN Classrooms c ON s.ClassroomId = c.ID
                            ORDER BY s.ID DESC";
                var students = await db.QueryAsync<Student>(sql);
                return Ok(students);
            }
        }

        [HttpGet("unassigned")]
        public async Task<IActionResult> GetUnassigned()
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = "SELECT * FROM Students WHERE ClassroomId IS NULL ORDER BY ID DESC";
                var students = await db.QueryAsync<Student>(sql);
                return Ok(students);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var sql = @"SELECT s.*, c.Name AS ClassroomName 
                            FROM Students s 
                            LEFT JOIN Classrooms c ON s.ClassroomId = c.ID 
                            WHERE s.ID = @id";
                var student = await db.QueryFirstOrDefaultAsync<Student>(sql, new { id });
                if (student == null)
                    return NotFound();

                return Ok(student);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Student student)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var checkSql = "SELECT COUNT(1) FROM Students WHERE StudentNumber = @StudentNumber";
                var exists = await db.ExecuteScalarAsync<int>(checkSql, new { student.StudentNumber });
                if (exists > 0)
                {
                    return BadRequest("Bu öğrenci numarası zaten kullanımda.");
                }

                var sql = @"INSERT INTO Students (FirstName, LastName, StudentNumber, Email, ClassroomId) 
                            VALUES (@FirstName, @LastName, @StudentNumber, @Email, @ClassroomId);
                            SELECT CAST(SCOPE_IDENTITY() as int);";
                var id = await db.QuerySingleAsync<int>(sql, student);
                student.ID = id;
                return CreatedAtAction(nameof(GetById), new { id }, student);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Student student)
        {
            if (id != student.ID)
                return BadRequest("ID mismatch");

            using (var db = new SqlConnection(_connectionString))
            {
                var checkSql = "SELECT COUNT(1) FROM Students WHERE StudentNumber = @StudentNumber AND ID != @ID";
                var exists = await db.ExecuteScalarAsync<int>(checkSql, new { student.StudentNumber, student.ID });
                if (exists > 0)
                {
                    return BadRequest("Bu öğrenci numarası başka bir öğrenci tarafından kullanılıyor.");
                }

                var sql = @"UPDATE Students 
                            SET FirstName = @FirstName, LastName = @LastName, 
                                StudentNumber = @StudentNumber, Email = @Email, ClassroomId = @ClassroomId 
                            WHERE ID = @ID";
                var rowsAffected = await db.ExecuteAsync(sql, student);
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
                var sql = "DELETE FROM Students WHERE ID = @id";
                var rowsAffected = await db.ExecuteAsync(sql, new { id });
                if (rowsAffected == 0)
                    return NotFound();

                return NoContent();
            }
        }

        [HttpPost("{id}/assign/{classroomId}")]
        public async Task<IActionResult> AssignClassroom(int id, int classroomId)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                int? targetClassroomId = classroomId > 0 ? classroomId : null;

                if (targetClassroomId.HasValue)
                {
                    var checkClassSql = "SELECT COUNT(1) FROM Classrooms WHERE ID = @classroomId";
                    var classExists = await db.ExecuteScalarAsync<int>(checkClassSql, new { classroomId = targetClassroomId.Value });
                    if (classExists == 0)
                    {
                        return NotFound("Sınıf bulunamadı.");
                    }

                    var capacitySql = @"SELECT Capacity FROM Classrooms WHERE ID = @classroomId;
                                        SELECT COUNT(1) FROM Students WHERE ClassroomId = @classroomId;";
                    using (var multi = await db.QueryMultipleAsync(capacitySql, new { classroomId = targetClassroomId.Value }))
                    {
                        var capacity = await multi.ReadFirstAsync<int>();
                        var currentStudents = await multi.ReadFirstAsync<int>();

                        if (currentStudents >= capacity)
                        {
                            return BadRequest("Sınıf kapasitesi dolu.");
                        }
                    }
                }

                var sql = "UPDATE Students SET ClassroomId = @targetClassroomId WHERE ID = @id";
                var rowsAffected = await db.ExecuteAsync(sql, new { id, targetClassroomId });
                if (rowsAffected == 0)
                    return NotFound("Öğrenci bulunamadı.");

                return Ok(new { Message = "Öğrenci sınıfa başarıyla atandı." });
            }
        }
    }
}
