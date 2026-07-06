using Microsoft.EntityFrameworkCore;
using Project_First.Models;

namespace Project_First.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Animals> Animals { get; set; }
    public DbSet<AnimalTypes> AnimalType { get; set; }
    public DbSet<FeedStock> FeedStocks { get; set; }
    public DbSet<Product> Products { get; set; }
    
}