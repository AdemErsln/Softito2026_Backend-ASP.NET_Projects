using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.MVC.Models;
using System.Text.Json;

namespace SchoolManagement.MVC.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Dashboard()
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            
            List<ClassroomViewModel> classrooms = new();
            List<StudentViewModel> students = new();

            try
            {
                var classroomsResponse = await client.GetAsync("classrooms");
                if (classroomsResponse.IsSuccessStatusCode)
                {
                    var data = await classroomsResponse.Content.ReadAsStringAsync();
                    classrooms = JsonSerializer.Deserialize<List<ClassroomViewModel>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }

                var studentsResponse = await client.GetAsync("students");
                if (studentsResponse.IsSuccessStatusCode)
                {
                    var data = await studentsResponse.Content.ReadAsStringAsync();
                    students = JsonSerializer.Deserialize<List<StudentViewModel>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"API bağlantı hatası: {ex.Message}";
            }

            ViewBag.ClassroomsCount = classrooms.Count;
            ViewBag.StudentsCount = students.Count;
            ViewBag.UnassignedStudentsCount = students.Count(s => s.ClassroomId == null);
            ViewBag.RecentStudents = students.Take(5).ToList();
            ViewBag.Classrooms = classrooms;

            return View();
        }
    }
}
