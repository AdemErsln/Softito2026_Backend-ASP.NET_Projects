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

app.MapGet("/", async (HttpContext context, KahveOtomati.Models.KahveOtomatiContext db) =>
{
    context.Response.ContentType = "text/html; charset=UTF-8";
    
    int drinkCount = await db.Icecekler.CountAsync();
    int stockCount = await db.MalzemeStoklari.CountAsync();
    int salesCount = await db.Satislar.CountAsync();
    decimal totalSales = await db.Satislar.SumAsync(s => s.Tutar);
    int activeErrors = await db.ArizaKayitlari.CountAsync();

    string html = $@"
<!DOCTYPE html>
<html lang=""tr"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Kahve Otomatı API Portal</title>
    <link href=""https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@300;400;500;600;700&display=swap"" rel=""stylesheet"">
    <style>
        :root {{
            --bg-color: #0b0f19;
            --card-bg: rgba(20, 26, 46, 0.45);
            --border-color: rgba(255, 255, 255, 0.08);
            --text-color: #f3f4f6;
            --text-muted: #9ca3af;
            --primary: #d97706;
            --primary-glow: rgba(217, 119, 6, 0.15);
            --accent: #f59e0b;
        }}
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
            font-family: 'Plus Jakarta Sans', sans-serif;
        }}
        body {{
            background: radial-gradient(circle at 50% 0%, #1e1b18 0%, var(--bg-color) 70%);
            color: var(--text-color);
            min-height: 100vh;
            display: flex;
            justify-content: center;
            align-items: center;
            padding: 20px;
            overflow-x: hidden;
        }}
        .container {{
            width: 100%;
            max-width: 800px;
            background: var(--card-bg);
            backdrop-filter: blur(20px);
            -webkit-backdrop-filter: blur(20px);
            border: 1px solid var(--border-color);
            border-radius: 24px;
            padding: 40px;
            box-shadow: 0 20px 40px rgba(0, 0, 0, 0.5), inset 0 1px 0 rgba(255, 255, 255, 0.1);
            text-align: center;
            position: relative;
        }}
        .container::before {{
            content: '';
            position: absolute;
            top: -2px;
            left: -2px;
            right: -2px;
            bottom: -2px;
            background: linear-gradient(135deg, var(--primary), transparent 60%);
            border-radius: 24px;
            z-index: -1;
            opacity: 0.5;
        }}
        .icon {{
            font-size: 64px;
            margin-bottom: 20px;
            display: inline-block;
            filter: drop-shadow(0 0 15px var(--primary));
        }}
        h1 {{
            font-size: 2.5rem;
            font-weight: 700;
            margin-bottom: 10px;
            background: linear-gradient(135deg, #ffffff 0%, #d97706 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
        }}
        .subtitle {{
            color: var(--text-muted);
            font-size: 1.1rem;
            margin-bottom: 30px;
        }}
        .stats-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(130px, 1fr));
            gap: 15px;
            margin-bottom: 35px;
        }}
        .stat-card {{
            background: rgba(255, 255, 255, 0.03);
            border: 1px solid rgba(255, 255, 255, 0.05);
            border-radius: 16px;
            padding: 15px;
            transition: all 0.3s ease;
        }}
        .stat-card:hover {{
            background: rgba(255, 255, 255, 0.06);
            border-color: var(--primary);
            transform: translateY(-2px);
        }}
        .stat-val {{
            font-size: 1.5rem;
            font-weight: 700;
            color: var(--accent);
            margin-bottom: 5px;
        }}
        .stat-lbl {{
            font-size: 0.8rem;
            color: var(--text-muted);
            text-transform: uppercase;
            letter-spacing: 0.05em;
        }}
        .btn {{
            display: inline-flex;
            align-items: center;
            justify-content: center;
            background: linear-gradient(135deg, var(--primary), var(--accent));
            color: #000;
            font-weight: 600;
            padding: 14px 28px;
            border-radius: 12px;
            text-decoration: none;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(217, 119, 6, 0.4);
            border: none;
            cursor: pointer;
            font-size: 1rem;
        }}
        .btn:hover {{
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(217, 119, 6, 0.6);
        }}
        .badge {{
            display: inline-block;
            padding: 6px 12px;
            background: rgba(16, 185, 129, 0.1);
            color: #10b981;
            border-radius: 50px;
            font-size: 0.85rem;
            font-weight: 500;
            margin-bottom: 20px;
            border: 1px solid rgba(16, 185, 129, 0.2);
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <span class=""icon"">☕</span>
        <div style=""margin-bottom: 5px;"">
            <span class=""badge"">● API Çevrimiçi</span>
        </div>
        <h1>Kahve Otomatı Yönetim Sistemi</h1>
        <p class=""subtitle"">EF Core ve SQL Server tabanlı, akıllı otomat takip ve stok yönetim API Servisi</p>
        
        <div class=""stats-grid"">
            <div class=""stat-card"">
                <div class=""stat-val"">{drinkCount}</div>
                <div class=""stat-lbl"">Aktif İçecek</div>
            </div>
            <div class=""stat-card"">
                <div class=""stat-val"">{stockCount}</div>
                <div class=""stat-lbl"">Malzeme Kalemi</div>
            </div>
            <div class=""stat-card"">
                <div class=""stat-val"">{salesCount}</div>
                <div class=""stat-lbl"">Toplam Satış</div>
            </div>
            <div class=""stat-card"">
                <div class=""stat-val"">{totalSales:N2} TL</div>
                <div class=""stat-lbl"">Ciro</div>
            </div>
            <div class=""stat-card"">
                <div class=""stat-val"">{activeErrors}</div>
                <div class=""stat-lbl"">Arıza Kaydı</div>
            </div>
        </div>
        
        <a href=""/scalar/v1"" class=""btn"">Scalar API Dokümantasyonunu Aç</a>
    </div>
</body>
</html>
";
    await context.Response.WriteAsync(html);
});

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
