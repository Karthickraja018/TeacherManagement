using Microsoft.Extensions.DependencyInjection;
using TeacherManagement.Controllers;

namespace TeacherManagement.Services
{
    public static class TeacherManagementExtensions
    {
        public static IServiceCollection AddTeacherManagementServices(this IServiceCollection services)
        {
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ISubjectService, SubjectService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<IBranchService, BranchService>();
            return services;
        }
    }
}