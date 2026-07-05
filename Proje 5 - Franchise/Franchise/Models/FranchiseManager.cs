using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Franchise.Models
{
   
        [Table("Franchise_Managers")]
        public class FranchiseManager
        {
            [Key]
            public int ManagerID { get; set; }

            [Required]
            public string ManagerName { get; set; } = string.Empty; // Örn: Ahmet Yılmaz

            [Required]
            public string BranchName { get; set; } = string.Empty; // Örn: Kadıköy Şubesi
        
    }
}
