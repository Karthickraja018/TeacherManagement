using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly TeacherContext _db;
        private readonly IMapper _mapper;

        public SubjectService(TeacherContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SubjectDto>> GetAllAsync()
        {
            var list = await _db.Subjects.Include(s => s.Courses).ToListAsync();
            return _mapper.Map<IEnumerable<SubjectDto>>(list);
        }

        public async Task<SubjectDto?> GetByIdAsync(int id)
        {
            var s = await _db.Subjects.Include(s => s.Courses).FirstOrDefaultAsync(s => s.SubjectId == id);
            if (s == null) return null;
            return _mapper.Map<SubjectDto>(s);
        }

        public async Task<SubjectDto> CreateAsync(SubjectCreateDto model)
        {
            var sub = _mapper.Map<Subject>(model);
            
            foreach (var cname in model.CourseNames ?? Enumerable.Empty<string>())
            {
                var name = cname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseName.ToLower() == name.ToLower());
                if (course == null)
                {
                    course = new Course { CourseName = name };
                    _db.Courses.Add(course);
                    await _db.SaveChangesAsync();
                }
                sub.Courses.Add(course);
            }
            
            _db.Subjects.Add(sub);
            await _db.SaveChangesAsync();
            
            var created = await _db.Subjects.Include(s => s.Courses).FirstOrDefaultAsync(s => s.SubjectId == sub.SubjectId);
            return _mapper.Map<SubjectDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, SubjectUpdateDto model)
        {
            var existing = await _db.Subjects.Include(s => s.Courses).FirstOrDefaultAsync(s => s.SubjectId == id);
            if (existing == null) return false;
            
            _mapper.Map(model, existing);
            existing.Courses.Clear();
            
            foreach (var cname in model.CourseNames ?? Enumerable.Empty<string>())
            {
                var name = cname?.Trim();
                if (string.IsNullOrEmpty(name)) continue;
                var course = await _db.Courses.FirstOrDefaultAsync(c => c.CourseName.ToLower() == name.ToLower());
                if (course == null)
                {
                    course = new Course { CourseName = name };
                    _db.Courses.Add(course);
                    await _db.SaveChangesAsync();
                }
                existing.Courses.Add(course);
            }
            
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Subjects.FindAsync(id);
            if (existing == null) return false;
            _db.Subjects.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
