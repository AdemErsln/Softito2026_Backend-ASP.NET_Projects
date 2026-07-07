namespace SchoolManagement.MVC.Models
{
    public class ClassroomViewModel
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? Location { get; set; }
        public int StudentCount { get; set; }
    }
}
