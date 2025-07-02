using eCommerceLite.Data;
using eCommerceLite.OrderService;
using Microsoft.EntityFrameworkCore; // Ensure this is included for DbContextOptionsBuilder extensions
using Microsoft.EntityFrameworkCore.Sqlite; // Add this using directive for SQLite support
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()      // Allow any domain (for testing/tunnels)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<OrderContext>(static options =>
    options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=eCommDB;Trusted_Connection=True;"));

builder.Services.AddTransient<IOrderService, OrderService>();
// Add services to the container.
builder.Services.AddControllers();
// Add swagger https://aka.ms/aspnetcore/swashbuckle
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
var templatesPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(templatesPath),
    RequestPath = "/templates"
});
app.Run();

