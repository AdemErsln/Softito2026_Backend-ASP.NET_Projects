using Kutuphane.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
// ---- EKLEMENİZ GEREKEN KISIM BAŞLANGICI ----
// SQL Server bağlantı dizesini (Connection String) ekliyoruz ve AppDbContext'i kaydediyoruz
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
// ---- EKLEMENİZ GEREKEN KISIM BİTİŞİ ----

// Register Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
});

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
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        
        if (!context.Authors.Any())
        {
            context.Authors.AddRange(
                new Author { Name = "Yaşar Kemal", Country = "Türkiye" },
                new Author { Name = "Fyodor Dostoyevski", Country = "Rusya" },
                new Author { Name = "George Orwell", Country = "İngiltere" }
            );
            context.SaveChanges();
        }
        
        if (!context.Books.Any())
        {
            var yasar = context.Authors.FirstOrDefault(a => a.Name == "Yaşar Kemal");
            var dostoyevski = context.Authors.FirstOrDefault(a => a.Name == "Fyodor Dostoyevski");
            var orwell = context.Authors.FirstOrDefault(a => a.Name == "George Orwell");
            
            context.Books.AddRange(
                new Book { Title = "İnce Memed", PublishYear = 1955, AuthorId = yasar?.AuthorId ?? 1 },
                new Book { Title = "Suç ve Ceza", PublishYear = 1866, AuthorId = dostoyevski?.AuthorId ?? 2 },
                new Book { Title = "1984", PublishYear = 1949, AuthorId = orwell?.AuthorId ?? 3 }
            );
            context.SaveChanges();
        }
        
        if (!context.Borrowers.Any())
        {
            var book = context.Books.FirstOrDefault();
            if (book != null)
            {
                context.Borrowers.AddRange(
                    new Borrower { FullName = "Ahmet Yılmaz", BorrowDate = DateTime.Now.AddDays(-5), BookId = book.BookId },
                    new Borrower { FullName = "Ayşe Kaya", BorrowDate = DateTime.Now.AddDays(-2), BookId = book.BookId }
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
