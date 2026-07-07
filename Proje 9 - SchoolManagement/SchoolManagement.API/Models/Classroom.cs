namespace SchoolManagement.API.Models
{
    public class Classroom
    {
        public int ID { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string? Location { get; set; }
        public int StudentCount { get; set; }
    }
}
