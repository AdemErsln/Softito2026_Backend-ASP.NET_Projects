using System.ComponentModel.DataAnnotations;

namespace Project_First.Models
{
    public class Product
    {
        [Key]
        public string id { get; set; }
        public string Name { get; set; } 

        public decimal Quantity { get; set; }

        public string Unit { get; set; }

        public DateTime ProductionDate { get; set; }
    }
}
