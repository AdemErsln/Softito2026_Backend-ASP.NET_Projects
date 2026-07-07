using Microsoft.AspNetCore.Mvc;
using SchoolManagement.MVC.Models;
using System.Diagnostics;
using System.Text.Json;

namespace SchoolManagement.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> ClassroomDetails(int id)
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            
            var classResponse = await client.GetAsync($"classrooms/{id}");
            if (!classResponse.IsSuccessStatusCode)
                return NotFound();

            var classData = await classResponse.Content.ReadAsStringAsync();
            var classroom = JsonSerializer.Deserialize<ClassroomViewModel>(classData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var studentsResponse = await client.GetAsync($"classrooms/{id}/students");
            List<StudentViewModel> students = new();
            if (studentsResponse.IsSuccessStatusCode)
            {
                var studentsData = await studentsResponse.Content.ReadAsStringAsync();
                students = JsonSerializer.Deserialize<List<StudentViewModel>>(studentsData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            ViewBag.Classroom = classroom;
            ViewBag.Students = students;

            return View(classroom);
        }
    }
}
