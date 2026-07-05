using System.ComponentModel.DataAnnotations;

namespace KahveOtomati.Models
{
    public class MalzemeStok
    {
        [Key]
        public int StokID { get; set; }

        [Required]
        public string MalzemeAdi { get; set; } = string.Empty;

        [Required]
        public int Miktar { get; set; }
    }
}
