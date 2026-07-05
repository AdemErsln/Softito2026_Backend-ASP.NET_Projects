using RentacCar.Model;
using System.ComponentModel.DataAnnotations;

namespace RentACar.Models
{
    public class Araclars
    {
        [Key]
        public int CarId { get; set; }

        [Required]
        public string Plaka { get; set; }

        [Required]
        public string Marka { get; set; }

        [Required]
        public string Model { get; set; }

        public int Yil { get; set; }

        public decimal GunlukUcret { get; set; }

        public string Durum { get; set; } = "Müsait";

        // Navigation Property
        public ICollection<Kiralama>? Kiralamas { get; set; }
    }
}