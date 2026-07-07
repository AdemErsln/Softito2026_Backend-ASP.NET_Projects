using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.MVC.Models;
using System.Text;
using System.Text.Json;

namespace SchoolManagement.MVC.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public StudentController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            var response = await client.GetAsync("students");
            List<StudentViewModel> students = new();

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                students = JsonSerializer.Deserialize<List<StudentViewModel>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }

            return View(students);
        }

        private async Task<List<ClassroomViewModel>> GetClassroomsAsync()
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            var response = await client.GetAsync("classrooms");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ClassroomViewModel>>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            return new List<ClassroomViewModel>();
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Classrooms = await GetClassroomsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(StudentViewModel student)
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");

            if (!ModelState.IsValid)
            {
                ViewBag.Classrooms = await GetClassroomsAsync();
                return View(student);
            }

            var content = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("students", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", string.IsNullOrEmpty(errorMsg) ? "Öğrenci eklenirken bir hata oluştu." : errorMsg);
            ViewBag.Classrooms = await GetClassroomsAsync();
            return View(student);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            var response = await client.GetAsync($"students/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var data = await response.Content.ReadAsStringAsync();
            var student = JsonSerializer.Deserialize<StudentViewModel>(data, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            ViewBag.Classrooms = await GetClassroomsAsync();
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, StudentViewModel student)
        {
            if (id != student.ID)
                return BadRequest();

            var client = _httpClientFactory.CreateClient("SchoolApi");

            if (!ModelState.IsValid)
            {
                ViewBag.Classrooms = await GetClassroomsAsync();
                return View(student);
            }

            var content = new StringContent(JsonSerializer.Serialize(student), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"students/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            var errorMsg = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", string.IsNullOrEmpty(errorMsg) ? "Öğrenci güncellenirken bir hata oluştu." : errorMsg);
            ViewBag.Classrooms = await GetClassroomsAsync();
            return View(student);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _httpClientFactory.CreateClient("SchoolApi");
            var response = await client.DeleteAsync($"students/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Öğrenci silinemedi.";
            return RedirectToAction(nameof(Index));
        }
    }
}
