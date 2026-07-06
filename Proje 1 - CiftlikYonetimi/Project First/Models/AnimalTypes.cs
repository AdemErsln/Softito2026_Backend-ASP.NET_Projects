using System.ComponentModel.DataAnnotations;

namespace Project_First.Models
{
    public class AnimalTypes
    {
        [Key]
        public int Id { get; set; }

        public string animal_type { get; set; } 


    }
}
