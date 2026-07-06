using Microsoft.EntityFrameworkCore;

namespace KahveOtomati.Models
{
    public class KahveOtomatiContext : DbContext
    {
        public KahveOtomatiContext(DbContextOptions<KahveOtomatiContext> options) : base(options)
        {
        }

        // Tablolar
        public DbSet<Icecek> Icecekler { get; set; }
        public DbSet<Satis> Satislar { get; set; }
        public DbSet<MalzemeStok> MalzemeStoklari { get; set; }
        public DbSet<ArizaKayit> ArizaKayitlari { get; set; }
    }
}
