using System.ComponentModel.DataAnnotations;

namespace KahveOtomati.Models
{
    public class Satis
    {
        [Key]
        public int SatisID { get; set; }

        [Required]
        public string IcecekAdi { get; set; } = string.Empty;

        [Required]
        public DateTime Tarih { get; set; }

        [Required]
        public decimal Tutar { get; set; }
    }
}
