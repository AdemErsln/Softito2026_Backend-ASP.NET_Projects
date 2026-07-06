using System.ComponentModel.DataAnnotations;

namespace KahveOtomati.Models
{
    public class Icecek
    {
        [Key]
        public int IcecekID { get; set; }

        [Required]
        public string IcecekAdi { get; set; } = string.Empty;

        [Required]
        public decimal Fiyat { get; set; }
    }
}
