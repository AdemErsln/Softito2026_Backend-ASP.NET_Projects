using System.ComponentModel.DataAnnotations;

namespace RentacCar.Model
{
    public class Kullanicilar
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        [StringLength(20)]
        public string AdSoyad { get; set; }


        public int yas { get; set; }
        public string Telefon { get; set; }

        public ICollection<Kiralama>? Kiralama
        {
            get; set;

        }
}
}
