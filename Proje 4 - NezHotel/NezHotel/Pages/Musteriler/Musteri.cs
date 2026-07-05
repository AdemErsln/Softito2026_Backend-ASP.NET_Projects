using System;

namespace NezHotel.Models
{
    public class Musteri
    {
        // Veritabanındaki PRIMARY KEY (MusteriID INT) alanı
        public int MusteriID { get; set; }

        // Müşterinin Adı Soyadı (NVARCHAR(100)) - Boş geçilemez
        public string AdSoyad { get; set; }

        // Müşterinin Telefon Numarası (NVARCHAR(20)) - Boş geçilebilir
        public string Telefon { get; set; }

        // Müşterinin E-Posta Adresi (NVARCHAR(100)) - Boş geçilebilir
        public string Email { get; set; }
    }
}