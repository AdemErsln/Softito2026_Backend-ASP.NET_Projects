using KahveOtomati.Models;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// EF Core - SQL Server bağlantısı
builder.Services.AddDbContext<KahveOtomatiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<KahveOtomatiContext>();
        context.Database.Migrate();
        
        if (!context.Icecekler.Any())
        {
            context.Icecekler.AddRange(
                new Icecek { IcecekAdi = "Espresso", Fiyat = 45.00m },
                new Icecek { IcecekAdi = "Caffe Latte", Fiyat = 55.00m },
                new Icecek { IcecekAdi = "Americano", Fiyat = 48.00m },
                new Icecek { IcecekAdi = "Cappuccino", Fiyat = 58.00m }
            );
            context.SaveChanges();
        }
        
        if (!context.MalzemeStoklari.Any())
        {
            context.MalzemeStoklari.AddRange(
                new MalzemeStok { MalzemeAdi = "Kahve Çekirdeği (g)", Miktar = 2500 },
                new MalzemeStok { MalzemeAdi = "Süt (ml)", Miktar = 5000 },
                new MalzemeStok { MalzemeAdi = "Su (ml)", Miktar = 10000 },
                new MalzemeStok { MalzemeAdi = "Şeker (g)", Miktar = 1000 }
            );
            context.SaveChanges();
        }
        
        if (!context.Satislar.Any())
        {
            context.Satislar.AddRange(
                new Satis { IcecekAdi = "Espresso", Tarih = DateTime.Now.AddHours(-2), Tutar = 45.00m },
                new Satis { IcecekAdi = "Caffe Latte", Tarih = DateTime.Now.AddHours(-1), Tutar = 55.00m },
                new Satis { IcecekAdi = "Americano", Tarih = DateTime.Now, Tutar = 48.00m }
            );
            context.SaveChanges();
        }
        
        if (!context.ArizaKayitlari.Any())
        {
            context.ArizaKayitlari.AddRange(
                new ArizaKayit { Tarih = DateTime.Now.AddDays(-2), Aciklama = "Su haznesi boş uyarısı giderildi." },
                new ArizaKayit { Tarih = DateTime.Now.AddDays(-1), Aciklama = "Atık haznesi temizlendi." }
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
