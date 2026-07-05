using Microsoft.EntityFrameworkCore;
using RentACar.Models;
using RentacCar.Model;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace RentaCar.Data
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Kullanicilar> kullanicilars { get; set; }
        public DbSet<Araclars> Araclar { get; set; }
        public DbSet<Kiralama> Kiralamas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}




