using System;

namespace NezHotel.Models
{
    public class Rezervasyon
    {
        // Veritabanındaki PRIMARY KEY alanı
        public int RezervasyonID { get; set; }

        // Musteriler tablosuna bağlanan yabancı anahtar (Foreign Key)
        public int MusteriID { get; set; }

        // Odalar tablosuna bağlanan yabancı anahtar (Foreign Key)
        public int OdaID { get; set; }

        // Giriş ve Çıkış tarihleri (SQL'deki DATE/DATETIME karşılığı)
        public DateTime GirisTarihi { get; set; }

        public DateTime CikisTarihi { get; set; }


        // --- İSTEĞE BAĞLI / GELİŞMİŞ ÖZELLİK ---
        // Eğer ileride nesne tabanlı mimariyi (Navigation Property) büyütmek istersen 
        // alt satırdaki property'leri de açabilirsin. Ama basit CRUD için üsttekiler yeterlidir.

        // public Musteri Musteri { get; set; }
        // public Odalar Oda { get; set; }
    }
}