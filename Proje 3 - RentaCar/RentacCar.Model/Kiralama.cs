using RentACar.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RentacCar.Model
{
    public class Kiralama
    {

        [Key]
        public int RentalId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public Kullanicilar? Kullanicilar { get; set; }

        [Required]
        public int CarId { get; set; }

        [ForeignKey("CarId")]
        public Araclars? Araclar { get; set; }

        [Required]
        public DateTime AlisTarihi { get; set; }

        [Required]
        public DateTime IadeTarihi { get; set; }

        public decimal ToplamTutar { get; set; }

        public string Durum { get; set; } = "Aktif";
    }
}
