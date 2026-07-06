using System.ComponentModel.DataAnnotations;

namespace KahveOtomati.Models
{
    public class ArizaKayit
    {
        [Key]
        public int ArizaID { get; set; }

        [Required]
        public DateTime Tarih { get; set; }

        [Required]
        public string Aciklama { get; set; } = string.Empty;
    }
}
