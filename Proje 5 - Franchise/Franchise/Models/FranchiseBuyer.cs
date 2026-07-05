using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Franchise.Models
{
  


        [Table("Franchise_Buyers")]
        public class FranchiseBuyer
        {
            [Key]
            public int BuyerID { get; set; }

            [Required]
            public string BuyerName { get; set; } = string.Empty; // Örn: Mehmet Can / Can Holding

        public int? PackageID { get; set; }
        public string Notes { get; set; } = string.Empty; // Örn: "Drive-thru konsepti istiyor"

        [ForeignKey("PackageID")]
        public virtual FranchisePackage? FranchisePackage { get; set; } 
    }
    }
