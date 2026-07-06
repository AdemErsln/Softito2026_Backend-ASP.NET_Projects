using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApiDbContext>(options => options.
UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<ApiDbContext>();
        context.Database.Migrate();
        
        if (!context.Depots.Any())
        {
            context.Depots.AddRange(
                new NezLojstikApi.Models.Depot { DepotName = "Merkez Depo", Address = "Atatürk Cad. No:1", City = "İstanbul" },
                new NezLojstikApi.Models.Depot { DepotName = "Ege Bölge Depo", Address = "Liman Sok. No:45", City = "İzmir" }
            );
            context.SaveChanges();
        }
        
        if (!context.Vehicles.Any())
        {
            context.Vehicles.AddRange(
                new NezLojstikApi.Models.Vehicle { Plate = "34TC1010", Brand = "Mercedes Actros", Capacity = 24000 },
                new NezLojstikApi.Models.Vehicle { Plate = "35KS999", Brand = "Volvo FH", Capacity = 26000 }
            );
            context.SaveChanges();
        }
        
        if (!context.Drivers.Any())
        {
            context.Drivers.AddRange(
                new NezLojstikApi.Models.Driver { FullName = "Hasan Kaya", PhoneNumber = "05321112233", LicenseNumber = "A-B-C-D-E" },
                new NezLojstikApi.Models.Driver { FullName = "Kemal Yıldız", PhoneNumber = "05442223344", LicenseNumber = "A-B-C" }
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
