using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.MVC.Models;
using System.Text;
using System.Text.Json;

namespace SchoolManagement.MVC.Controllers
{
    [Authorize]
    public class ClassroomController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ClassroomController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            var response = await client.GetAsync("classrooms");
            List<ClassroomViewModel> classrooms = new();

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                classrooms = JsonSerializer.Deserialize<List<ClassroomViewModel>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View(classrooms);
        }

        public async Task<IActionResult> Details(int id)
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

            var unassignedResponse = await client.GetAsync("students/unassigned");
            List<StudentViewModel> unassignedStudents = new();
            if (unassignedResponse.IsSuccessStatusCode)
            {
                var unassignedData = await unassignedResponse.Content.ReadAsStringAsync();
                unassignedStudents = JsonSerializer.Deserialize<List<StudentViewModel>>(unassignedData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            ViewBag.Classroom = classroom;
            ViewBag.Students = students;
            ViewBag.UnassignedStudents = unassignedStudents;

            return View(classroom);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ClassroomViewModel classroom)
        {
            if (!ModelState.IsValid)
                return View(classroom);

            var client = _httpClientFactory.CreateClient("SchoolApi");
            var content = new StringContent(JsonSerializer.Serialize(classroom), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("classrooms", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Sınıf eklenirken bir hata oluştu.");
            return View(classroom);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            var response = await client.GetAsync($"classrooms/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var data = await response.Content.ReadAsStringAsync();
            var classroom = JsonSerializer.Deserialize<ClassroomViewModel>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(classroom);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ClassroomViewModel classroom)
        {
            if (id != classroom.ID)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(classroom);

            var client = _httpClientFactory.CreateClient("SchoolApi");
            var content = new StringContent(JsonSerializer.Serialize(classroom), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"classrooms/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Sınıf güncellenirken bir hata oluştu.");
            return View(classroom);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            var response = await client.DeleteAsync($"classrooms/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Sınıf silinemedi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> AddStudentToClass(int classroomId, int studentId)
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            var response = await client.PostAsync($"students/{studentId}/assign/{classroomId}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Öğrenci sınıfa başarıyla eklendi.";
            }
            else
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                TempData["Error"] = string.IsNullOrEmpty(errorMsg) ? "Öğrenci sınıfa eklenemedi (Sınıf kapasitesi dolu olabilir)." : errorMsg;
            }

            return RedirectToAction(nameof(Details), new { id = classroomId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveStudentFromClass(int classroomId, int studentId)
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            var response = await client.PostAsync($"students/{studentId}/assign/0", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Öğrenci sınıftan çıkarıldı.";
            }
            else
            {
                TempData["Error"] = "İşlem gerçekleştirilemedi.";
            }

            return RedirectToAction(nameof(Details), new { id = classroomId });
        }
    }
}
