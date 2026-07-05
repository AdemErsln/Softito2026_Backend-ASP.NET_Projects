using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Franchise.Models
{
    
        [Table("Franchise_Packages")]
        public class FranchisePackage
        {
            [Key]
            public int PackageID { get; set; }

            [Required]
            public string PackageName { get; set; } = string.Empty; // Örn: Gold Restoran Konsepti, Kahve Standı

            public string Price { get; set; } = string.Empty; // Örn: 750.000 TL
        
    }
}
