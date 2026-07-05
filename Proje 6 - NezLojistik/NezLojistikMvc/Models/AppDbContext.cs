using Microsoft.EntityFrameworkCore;
using NezLojistikMvc.MVC_Data;
using NezLojistikMvc.Controllers;

public class MvcDbContext : DbContext
{
    public MvcDbContext(DbContextOptions<MvcDbContext> options)
        : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
}

  