using Franchise;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// --- VERİ TABANI BUILDER KODU BAŞLANGICI ---
// DbContext'i projeye servis olarak ekliyoruz ve Connection String'i bağlıyoruz
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// --- VERİ TABANI BUILDER KODU BİTİŞİ ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
        
        if (!context.FranchisePackages.Any())
        {
            context.FranchisePackages.AddRange(
                new Franchise.Models.FranchisePackage { PackageName = "Gold Restoran Konsepti", Price = "750.000 TL" },
                new Franchise.Models.FranchisePackage { PackageName = "Kahve Standı Konsepti", Price = "250.000 TL" },
                new Franchise.Models.FranchisePackage { PackageName = "Express Büfe Konsepti", Price = "400.000 TL" }
            );
            context.SaveChanges();
        }
        
        if (!context.FranchiseManagers.Any())
        {
            context.FranchiseManagers.AddRange(
                new Franchise.Models.FranchiseManager { ManagerName = "Ahmet Yılmaz", BranchName = "Kadıköy Şubesi" },
                new Franchise.Models.FranchiseManager { ManagerName = "Zeynep Kaya", BranchName = "Beşiktaş Şubesi" },
                new Franchise.Models.FranchiseManager { ManagerName = "Murat Demir", BranchName = "Alsancak Şubesi" }
            );
            context.SaveChanges();
        }
        
        if (!context.FranchiseBuyers.Any())
        {
            var package = context.FranchisePackages.FirstOrDefault();
            context.FranchiseBuyers.Add(
                new Franchise.Models.FranchiseBuyer
                {
                    BuyerName = "Mehmet Can / Can Holding",
                    PackageID = package?.PackageID,
                    Notes = "Drive-thru konsepti talep ediyor."
                }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding database: {ex.Message}");
    }
}

app.Run();
