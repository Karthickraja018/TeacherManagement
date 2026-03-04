using Microsoft.EntityFrameworkCore;
using TeacherManagement.Data;
using TeacherManagement.DTOs;
using TeacherManagement.Models;

namespace TeacherManagement.Data
{
    public class TeacherContext : DbContext
    {
        public TeacherContext(DbContextOptions<TeacherContext> options) : base(options)
        {
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }

        // Keyless DTO mapping for stored proc
        public DbSet<StudentDetailsDto> StudentDetails { get; set; }
        public DbSet<TeacherDetailsDto> TeacherDetails { get; set; } // <-- Added TeacherDetailsDto DbSet

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureBranch(modelBuilder);
            ConfigureAddress(modelBuilder);
            ConfigureStudent(modelBuilder);
            ConfigureTeacher(modelBuilder);
            ConfigureCourse(modelBuilder);
            ConfigureSubject(modelBuilder);

            // Configure many-to-many join tables explicitly
            modelBuilder.Entity("StudentCourse", b =>
            {
                b.ToTable("StudentCourses");
                b.HasKey("StudentId", "CourseId");
                b.HasOne(typeof(Student), null).WithMany().HasForeignKey("StudentId").OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Course), null).WithMany().HasForeignKey("CourseId").OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("TeacherSubject", b =>
            {
                b.ToTable("TeacherSubjects");
                b.HasKey("TeacherId", "SubjectId");
                b.HasOne(typeof(Teacher), null).WithMany().HasForeignKey("TeacherId").OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Subject), null).WithMany().HasForeignKey("SubjectId").OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("CourseSubject", b =>
            {
                b.ToTable("CourseSubjects");
                b.HasKey("CourseId", "SubjectId");
                b.HasOne(typeof(Course), null).WithMany().HasForeignKey("CourseId").OnDelete(DeleteBehavior.Cascade);
                b.HasOne(typeof(Subject), null).WithMany().HasForeignKey("SubjectId").OnDelete(DeleteBehavior.Cascade);
            });

            // Keyless mapping for stored-proc DTO
            modelBuilder.Entity<StudentDetailsDto>().HasNoKey();
            modelBuilder.Entity<TeacherDetailsDto>().HasNoKey(); // <-- Configured TeacherDetailsDto as keyless
        }

        private void ConfigureBranch(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Branch>(entity =>
            {
                entity.ToTable("Branches");
                entity.HasKey(b => b.BranchId);
                entity.Property(b => b.Name).IsRequired().HasMaxLength(200);

                entity.HasMany(b => b.Students).WithOne(s => s.Branch).HasForeignKey(s => s.BranchId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(b => b.Teachers).WithOne(t => t.Branch).HasForeignKey(t => t.BranchId).OnDelete(DeleteBehavior.Restrict);
            });
        }

        private void ConfigureAddress(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Addresses");
                entity.HasKey(a => a.AddressId);
                entity.Property(a => a.Line1).IsRequired().HasMaxLength(200);
                entity.Property(a => a.Line2).HasMaxLength(200);
                entity.Property(a => a.City).HasMaxLength(100);
                entity.Property(a => a.State).HasMaxLength(100);
                entity.Property(a => a.Zip).HasMaxLength(20);
            });
        }

        private void ConfigureStudent(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");
                entity.HasKey(s => s.StudentId);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
                entity.Property(s => s.BranchId).IsRequired();
                entity.Property(s => s.AddressId).IsRequired(false);

                // Many-to-one to Branch is configured in Branch entity but make explicit here as well
                entity.HasOne(s => s.Branch).WithMany(b => b.Students).HasForeignKey(s => s.BranchId).OnDelete(DeleteBehavior.Restrict);

                // Optional address
                entity.HasOne(s => s.Address).WithMany().HasForeignKey(s => s.AddressId).OnDelete(DeleteBehavior.SetNull);

                // Many-to-many with Course via join table
                entity.HasMany(s => s.Courses).WithMany(c => c.Students)
                    .UsingEntity<Dictionary<string, object>>("StudentCourse",
                        j => j.HasOne<Course>().WithMany().HasForeignKey("CourseId").OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<Student>().WithMany().HasForeignKey("StudentId").OnDelete(DeleteBehavior.Cascade),
                        j => j.ToTable("StudentCourses").HasKey("StudentId", "CourseId")
                    );
            });
        }

        private void ConfigureTeacher(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teachers");
                entity.HasKey(t => t.TeacherId);
                entity.Property(t => t.TeacherName).IsRequired().HasMaxLength(200);
                entity.Property(t => t.BranchId).IsRequired();
                entity.Property(t => t.AddressId).IsRequired(false);

                entity.HasOne(t => t.Branch).WithMany(b => b.Teachers).HasForeignKey(t => t.BranchId).OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.Address).WithMany().HasForeignKey(t => t.AddressId).OnDelete(DeleteBehavior.SetNull);

                // FIX: Specify generic types for UsingEntity to avoid CS0266 and CS1662
                entity.HasMany(t => t.Subjects).WithMany(s => s.Teachers)
                    .UsingEntity<Dictionary<string, object>>(
                        "TeacherSubject",
                        j => j.HasOne<Subject>().WithMany().HasForeignKey("SubjectId").OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<Teacher>().WithMany().HasForeignKey("TeacherId").OnDelete(DeleteBehavior.Cascade),
                        j => j.ToTable("TeacherSubjects").HasKey("TeacherId", "SubjectId")
                    );
            });
        }

        private void ConfigureCourse(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Courses");
                entity.HasKey(c => c.CourseId);
                entity.Property(c => c.CourseName).IsRequired().HasMaxLength(200);

                entity.HasMany(c => c.Subjects).WithMany(s => s.Courses)
                    .UsingEntity<Dictionary<string, object>>("CourseSubject",
                        j => j.HasOne<Subject>().WithMany().HasForeignKey("SubjectId").OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<Course>().WithMany().HasForeignKey("CourseId").OnDelete(DeleteBehavior.Cascade),
                        j => j.ToTable("CourseSubjects").HasKey("CourseId", "SubjectId")
                    );
            });
        }

        private void ConfigureSubject(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Subject>(entity =>
            {
                entity.ToTable("Subjects");
                entity.HasKey(s => s.SubjectId);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(200);
            });
        }
    }
}
