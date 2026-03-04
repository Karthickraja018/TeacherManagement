using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.Models;
using TeacherManagement.Middleware;
using TeacherManagement.Controllers;
using TeacherManagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Swashbuckle

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Configure EF Core with SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TeacherContext>(options =>
    options.UseSqlServer(connectionString));

// Register application services directly
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IBranchService, BranchService>();

var app = builder.Build();

// Use global exception handler middleware directly
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

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
