using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllAsync();
        Task<CourseDto?> GetByIdAsync(int id);
        Task<CourseDto> CreateAsync(CourseCreateDto model);
        Task<bool> UpdateAsync(int id, CourseUpdateDto model);
        Task<bool> DeleteAsync(int id);
    }

    public class CourseService : ICourseService
    {
        private readonly TeacherContext _db;
        public CourseService(TeacherContext db) { _db = db; }

        public async Task<IEnumerable<CourseDto>> GetAllAsync()
        {
            var list = await _db.Courses.Include(c => c.Subjects).ToListAsync();
            return list.Select(c => new CourseDto { CourseId = c.CourseId, CourseName = c.CourseName, SubjectIds = c.Subjects.Select(s => s.SubjectId).ToList(), SubjectNames = c.Subjects.Select(s => s.Name).ToList() });
        }

        public async Task<CourseDto?> GetByIdAsync(int id)
        {
            var c = await _db.Courses.Include(c => c.Subjects).FirstOrDefaultAsync(c => c.CourseId == id);
            if (c == null) return null;
            return new CourseDto { CourseId = c.CourseId, CourseName = c.CourseName, SubjectIds = c.Subjects.Select(s => s.SubjectId).ToList(), SubjectNames = c.Subjects.Select(s => s.Name).ToList() };
        }

        public async Task<CourseDto> CreateAsync(CourseCreateDto model)
        {
            var course = new Course { CourseName = model.CourseName.Trim() };
            foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>())
            {
                var name = sname?.Trim(); if (string.IsNullOrEmpty(name)) continue;
                var sub = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower());
                if (sub == null) { sub = new Subject { Name = name }; _db.Subjects.Add(sub); await _db.SaveChangesAsync(); }
                course.Subjects.Add(sub);
            }
            _db.Courses.Add(course); await _db.SaveChangesAsync();
            return new CourseDto { CourseId = course.CourseId, CourseName = course.CourseName, SubjectIds = course.Subjects.Select(s => s.SubjectId).ToList(), SubjectNames = course.Subjects.Select(s => s.Name).ToList() };
        }

        public async Task<bool> UpdateAsync(int id, CourseUpdateDto model)
        {
            var existing = await _db.Courses.Include(c => c.Subjects).FirstOrDefaultAsync(c => c.CourseId == id);
            if (existing == null) return false;
            existing.CourseName = model.CourseName;
            existing.Subjects.Clear();
            foreach (var sname in model.SubjectNames ?? Enumerable.Empty<string>()) { var name = sname?.Trim(); if (string.IsNullOrEmpty(name)) continue; var sub = await _db.Subjects.FirstOrDefaultAsync(s => s.Name.ToLower() == name.ToLower()); if (sub == null) { sub = new Subject { Name = name }; _db.Subjects.Add(sub); await _db.SaveChangesAsync(); } existing.Subjects.Add(sub); }
            await _db.SaveChangesAsync(); return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Courses.FindAsync(id); if (existing == null) return false; _db.Courses.Remove(existing); await _db.SaveChangesAsync(); return true;
        }
    }
}
