using Franchise.Models;
using Microsoft.EntityFrameworkCore;

namespace Franchise
{
 
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<FranchisePackage> FranchisePackages { get; set; }
        public DbSet<FranchiseManager> FranchiseManagers { get; set; }
        public DbSet<FranchiseBuyer> FranchiseBuyers { get; set; }
    }
}
