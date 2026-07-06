using Microsoft.EntityFrameworkCore;
using Project_First.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")));
// Add services to the container.
builder.Services.AddControllersWithViews();

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
        
        if (!context.AnimalType.Any())
        {
            context.AnimalType.AddRange(
                new Project_First.Models.AnimalTypes { animal_type = "Büyükbaş" },
                new Project_First.Models.AnimalTypes { animal_type = "Küçükbaş" },
                new Project_First.Models.AnimalTypes { animal_type = "Kanatlı" }
            );
            context.SaveChanges();
        }
        
        if (!context.Animals.Any())
        {
            var buyukbas = context.AnimalType.FirstOrDefault(t => t.animal_type == "Büyükbaş");
            var kucukbas = context.AnimalType.FirstOrDefault(t => t.animal_type == "Küçükbaş");
            context.Animals.AddRange(
                new Project_First.Models.Animals { Name = "Sarıkız", AnimalTypeId = buyukbas?.Id ?? 1 },
                new Project_First.Models.Animals { Name = "Karabaş", AnimalTypeId = kucukbas?.Id ?? 2 },
                new Project_First.Models.Animals { Name = "Benekli", AnimalTypeId = buyukbas?.Id ?? 1 }
            );
            context.SaveChanges();
        }
        
        if (!context.FeedStocks.Any())
        {
            context.FeedStocks.AddRange(
                new Project_First.Models.FeedStock { name = "Saman Yemi", amount = 500 },
                new Project_First.Models.FeedStock { name = "Yonca Yemi", amount = 300 },
                new Project_First.Models.FeedStock { name = "Mısır Silajı", amount = 1000 }
            );
            context.SaveChanges();
        }
        
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Project_First.Models.Product { id = "SUT01", Name = "Süt", Quantity = 120, Unit = "Litre", ProductionDate = DateTime.Now },
                new Project_First.Models.Product { id = "PEY01", Name = "Peynir", Quantity = 45, Unit = "KG", ProductionDate = DateTime.Now },
                new Project_First.Models.Product { id = "YUM01", Name = "Yumurta", Quantity = 500, Unit = "Adet", ProductionDate = DateTime.Now }
            );
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding data: {ex.Message}");
    }
}

app.Run();
