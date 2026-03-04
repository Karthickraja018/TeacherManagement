using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllAsync();
        Task<SubjectDto?> GetByIdAsync(int id);
        Task<SubjectDto> CreateAsync(SubjectCreateDto model);
        Task<bool> UpdateAsync(int id, SubjectUpdateDto model);
        Task<bool> DeleteAsync(int id);
    }

    public class SubjectService : ISubjectService
    {
        private readonly TeacherContext _db;
        public SubjectService(TeacherContext db) { _db = db; }

        public async Task<IEnumerable<SubjectDto>> GetAllAsync()
        {
            var list = await _db.Subjects.Include(s => s.Courses).ToListAsync();
            return list.Select(s => new SubjectDto { SubjectId = s.SubjectId, Name = s.Name, CourseIds = s.Courses.Select(c => c.CourseId).ToList(), CourseNames = s.Courses.Select(c => c.CourseName).ToList() });
        }

        public async Task<SubjectDto?> GetByIdAsync(int id)
        {
            var s = await _db.Subjects.Include(s => s.Courses).FirstOrDefaultAsync(s => s.SubjectId == id);
            if (s == null) return null;
            return new SubjectDto { SubjectId = s.SubjectId, Name = s.Name, CourseIds = s.Courses.Select(c => c.CourseId).ToList(), CourseNames = s.Courses.Select(c => c.CourseName).ToList() };
        }

        public async Task<SubjectDto> CreateAsync(SubjectCreateDto model)
        {
            var sub = new Subject { Name = model.Name.Trim() };
            foreach (var cname in model.CourseNames ?? Enumerable.Empty<string>())
            {
                var name = cname?.Trim(); if (string.IsNullOrEmpty(name)) continue;
                var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseName.ToLower() == name.ToLower());
                if (course == null) { course = new Course { CourseName = name }; _db.Courses.Add(course); await _db.SaveChangesAsync(); }
                sub.Courses.Add(course);
            }
            _db.Subjects.Add(sub); await _db.SaveChangesAsync();
            return new SubjectDto { SubjectId = sub.SubjectId, Name = sub.Name, CourseIds = sub.Courses.Select(c => c.CourseId).ToList(), CourseNames = sub.Courses.Select(c => c.CourseName).ToList() };
        }

        public async Task<bool> UpdateAsync(int id, SubjectUpdateDto model)
        {
            var existing = await _db.Subjects.Include(s => s.Courses).FirstOrDefaultAsync(s => s.SubjectId == id);
            if (existing == null) return false;
            existing.Name = model.Name;
            existing.Courses.Clear();
            foreach (var cname in model.CourseNames ?? Enumerable.Empty<string>()) { var name = cname?.Trim(); if (string.IsNullOrEmpty(name)) continue; var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseName.ToLower() == name.ToLower()); if (course == null) { course = new Course { CourseName = name }; _db.Courses.Add(course); await _db.SaveChangesAsync(); } existing.Courses.Add(course); }
            await _db.SaveChangesAsync(); return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Subjects.FindAsync(id); if (existing == null) return false; _db.Subjects.Remove(existing); await _db.SaveChangesAsync(); return true;
        }
    }
}
