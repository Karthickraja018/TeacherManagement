using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.Models;
using TeacherManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Swashbuckle

// Configure EF Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TeacherContext>(options =>
    options.UseSqlServer(connectionString));

// Register app services
builder.Services.AddTeacherManagementServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // swagger.json
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeacherManagement API v1");
        c.RoutePrefix = "swagger"; // use /swagger (or set to string.Empty for root)
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
