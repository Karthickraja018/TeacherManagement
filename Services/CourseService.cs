using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;
using TeacherManagement.Services.Interfaces;


namespace TeacherManagement.Services
{
   
    public class CourseService : ICourseService
    {
        private readonly TeacherContext _db;
        private readonly IMapper _mapper;

        public CourseService(TeacherContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            var list = await _db.Courses.Include(c => c.Subjects).ToListAsync();
            return _mapper.Map<IEnumerable<CourseDto>>(list);
        }

        public async Task<CourseDto?> GetByIdAsync(int id)
        {
            var c = await _db.Courses.Include(c => c.Subjects).FirstOrDefaultAsync(c => c.CourseId == id);
            if (c == null) return null;
            return _mapper.Map<CourseDto>(c);
        }

        public async Task<CourseDto> CreateAsync(CourseCreateDto model)
        {
            var course = _mapper.Map<Course>(model);
            
            foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>())
            {
                var name = sname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var sub = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
                if (sub == null)
                {
                    sub = new Subject { Name = name };
                    _db.Subjects.Add(sub);
                    await _db.SaveChangesAsync();
                }
                course.Subjects.Add(sub);
            }
            
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
            
            var created = await _db.Courses.Include(c => c.Subjects).FirstOrDefaultAsync(c => c.CourseId == course.CourseId);
            return _mapper.Map<CourseDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, CourseUpdateDto model)
        {
            var existing = await _db.Courses.Include(c => c.Subjects).FirstOrDefaultAsync(c => c.CourseId == id);
            if (existing == null) return false;
            
            _mapper.Map(model, existing);
            existing.Subjects.Clear();
            
            foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>())
            {
                var name = sname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var sub = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
                if (sub == null)
                {
                    sub = new Subject { Name = name };
                    _db.Subjects.Add(sub);
                    await _db.SaveChangesAsync();
                }
                existing.Subjects.Add(sub);
            }
            
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Courses.FindAsync(id);
            if (existing == null) return false;
            _db.Courses.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
