namespace SchoolManagement.API.Models
{
    public class Student
    {
        public int ID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string StudentNumber { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int? ClassroomId { get; set; }
        public string? ClassroomName { get; set; }
    }
}
