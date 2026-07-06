using System.ComponentModel.DataAnnotations;

namespace Project_First.Models
{
    public class FeedStock
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public int amount { get; set; }

        
    }
}
