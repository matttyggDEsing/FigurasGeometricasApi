using FigurasGeometricasApi.Data;
using FigurasGeometricasApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=figuras.db";
builder.Services.AddDbContext<FigurasContext>(options =>
    options.UseSqlite(conn)
);

// Application services
builder.Services.AddScoped<IFiguraService, FiguraService>();

var app = builder.Build();

// Ensure DB created (best for dev; for production use migrations)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FigurasContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
