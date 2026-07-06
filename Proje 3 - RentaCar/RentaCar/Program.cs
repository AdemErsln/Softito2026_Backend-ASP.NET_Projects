using Microsoft.EntityFrameworkCore;
using RentaCar.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDBContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Register Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});


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

app.UseAuthentication();
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
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        context.Database.Migrate();
        
        if (!context.Araclar.Any())
        {
            context.Araclar.AddRange(
                new RentACar.Models.Araclars { Plaka = "34ABC123", Marka = "BMW", Model = "320i", Yil = 2022, GunlukUcret = 1500, Durum = "Müsait" },
                new RentACar.Models.Araclars { Plaka = "06XYZ987", Marka = "Audi", Model = "A4", Yil = 2021, GunlukUcret = 1400, Durum = "Müsait" },
                new RentACar.Models.Araclars { Plaka = "35QWE456", Marka = "Fiat", Model = "Egea", Yil = 2023, GunlukUcret = 800, Durum = "Müsait" }
            );
            context.SaveChanges();
        }
        
        if (!context.kullanicilars.Any())
        {
            context.kullanicilars.AddRange(
                new RentacCar.Model.Kullanicilar { AdSoyad = "Adem Erslan", yas = 25, Telefon = "05555555555" },
                new RentacCar.Model.Kullanicilar { AdSoyad = "Ahmet Yılmaz", yas = 30, Telefon = "05321234567" }
            );
            context.SaveChanges();
        }
        
        if (!context.Kiralamas.Any())
        {
            var car = context.Araclar.FirstOrDefault();
            var customer = context.kullanicilars.FirstOrDefault();
            if (car != null && customer != null)
            {
                context.Kiralamas.Add(
                    new RentacCar.Model.Kiralama
                    {
                        CarId = car.CarId,
                        CustomerId = customer.Id,
                        AlisTarihi = DateTime.Now.AddDays(-3),
                        IadeTarihi = DateTime.Now.AddDays(2),
                        ToplamTutar = car.GunlukUcret * 5,
                        Durum = "Aktif"
                    }
                );
                context.SaveChanges();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error seeding database: {ex.Message}");
    }
}

app.Run();
