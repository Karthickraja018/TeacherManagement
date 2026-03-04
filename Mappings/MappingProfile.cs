using AutoMapper;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Student mappings
            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.CourseIds, opt => opt.MapFrom(src => src.Courses.Select(c => c.CourseId).ToList()));
            
            CreateMap<StudentCreateDto, Student>()
                .ForMember(dest => dest.StudentId, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore());

            CreateMap<StudentUpdateDto, Student>()
                .ForMember(dest => dest.StudentId, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore());

            // Teacher mappings
            CreateMap<Teacher, TeacherDto>()
                .ForMember(dest => dest.SubjectIds, opt => opt.MapFrom(src => src.Subjects.Select(s => s.SubjectId).ToList()));

            CreateMap<TeacherCreateDto, Teacher>()
                .ForMember(dest => dest.TeacherId, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                .ForMember(dest => dest.Subjects, opt => opt.Ignore());

            CreateMap<TeacherUpdateDto, Teacher>()
                .ForMember(dest => dest.TeacherId, opt => opt.Ignore())
                .ForMember(dest => dest.Branch, opt => opt.Ignore())
                .ForMember(dest => dest.BranchId, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .ForMember(dest => dest.AddressId, opt => opt.Ignore())
                .ForMember(dest => dest.Subjects, opt => opt.Ignore());

            // Course mappings
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.SubjectIds, opt => opt.MapFrom(src => src.Subjects.Select(s => s.SubjectId).ToList()))
                .ForMember(dest => dest.SubjectNames, opt => opt.MapFrom(src => src.Subjects.Select(s => s.Name).ToList()));

            CreateMap<CourseCreateDto, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore())
                .ForMember(dest => dest.Subjects, opt => opt.Ignore());

            CreateMap<CourseUpdateDto, Course>()
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.Students, opt => opt.Ignore())
                .ForMember(dest => dest.Subjects, opt => opt.Ignore());

            // Subject mappings
            CreateMap<Subject, SubjectDto>()
                .ForMember(dest => dest.CourseIds, opt => opt.MapFrom(src => src.Courses.Select(c => c.CourseId).ToList()))
                .ForMember(dest => dest.CourseNames, opt => opt.MapFrom(src => src.Courses.Select(c => c.CourseName).ToList()));

            CreateMap<SubjectCreateDto, Subject>()
                .ForMember(dest => dest.SubjectId, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore())
                .ForMember(dest => dest.Teachers, opt => opt.Ignore());

            CreateMap<SubjectUpdateDto, Subject>()
                .ForMember(dest => dest.SubjectId, opt => opt.Ignore())
                .ForMember(dest => dest.Courses, opt => opt.Ignore())
                .ForMember(dest => dest.Teachers, opt => opt.Ignore());

            // Address mappings
            CreateMap<Address, AddressDto>();
            CreateMap<AddressCreateDto, Address>()
                .ForMember(dest => dest.AddressId, opt => opt.Ignore());

            // Branch mappings
            CreateMap<Branch, BranchDto>();
            CreateMap<BranchDto, Branch>()
                .ForMember(dest => dest.Students, opt => opt.Ignore())
                .ForMember(dest => dest.Teachers, opt => opt.Ignore());
        }
    }
}
