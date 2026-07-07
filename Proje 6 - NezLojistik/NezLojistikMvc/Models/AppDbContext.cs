using Microsoft.EntityFrameworkCore;
using NezLojistikMvc.MVC_Data;

public class MvcDbContext : DbContext
{
    public MvcDbContext(DbContextOptions<MvcDbContext> options)
        : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
    public DbSet<Admins> Admins { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed default admin
        modelBuilder.Entity<Admins>().HasData(
            new Admins { Id = 1, FullName = "Admin", Email = "admin@nez.com", Password = "admin123" }
        );

        // Seed default users
        modelBuilder.Entity<Users>().HasData(
            new Users { Id = 1, FullName = "Ahmet Yılmaz", Email = "ahmet@nez.com", Password = "123456", Bolge = "Marmara" },
            new Users { Id = 2, FullName = "Mehmet Kaya", Email = "mehmet@nez.com", Password = "123456", Bolge = "Ege" },
            new Users { Id = 3, FullName = "Ayşe Demir", Email = "ayse@nez.com", Password = "123456", Bolge = "İç Anadolu" }
        );
    }
}