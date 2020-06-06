using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Faculty.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Hosting.Internal;

//Generic controllers in ASP.Net Core
//https://www.ben-morris.com/generic-controllers-in-asp-net-core/
namespace Faculty.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IWebHostEnvironment env)
            : base(options)
        {
            _env = env;
        }
        private readonly IWebHostEnvironment _env;

        public DbSet<Student> Students { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Cathedra> Cathedras { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Statement> Statements { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<MarkValue> MarkValues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) // versh
        {
            base.OnModelCreating(modelBuilder);

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict; // не удалять каскадно
            }
            //modelBuilder.ApplyConfiguration(new RoleConfiguration());

            //https://stackoverflow.com/questions/55970650/ef-core-may-cause-cycles-or-multiple-cascade-paths
            // это дает удаление отметок при удалении ведомости
            //ALTER TABLE[dbo].[Marks]  WITH CHECK ADD CONSTRAINT[FK_Marks_Statements_StatementId] FOREIGN KEY([StatementId])
            //REFERENCES[dbo].[Statements]([Id])
            //ON DELETE CASCADE
            modelBuilder.Entity<Mark>()
           .HasOne(i => i.Statement)
           .WithMany(c => c.Marks)
           .OnDelete(DeleteBehavior.Cascade); //ON DELETE CASCADE

            modelBuilder.Entity<MarkValue>().HasData(new MarkValue { Id = 2, Name = "Неудовлетворительно" });
            modelBuilder.Entity<MarkValue>().HasData(new MarkValue { Id = 3, Name = "Удовлетворительно" });
            modelBuilder.Entity<MarkValue>().HasData(new MarkValue { Id = 4, Name = "Хорошо" });
            modelBuilder.Entity<MarkValue>().HasData(new MarkValue { Id = 5, Name = "Отлично" });

            modelBuilder.Entity<PaymentType>().HasData(new PaymentType { Id = 1, Name = "Очная" });
            modelBuilder.Entity<PaymentType>().HasData(new PaymentType { Id = 2, Name = "Заочная" });
            for (int i = 1; i <= 5; i++)
            {
                modelBuilder.Entity<Course>().HasData(new Course { Id = i, Number = i });
            }

            //string contentRootPath = _env.ContentRootPath; //E:\Visual Studio 2017\Projects\Факультет\Факультет
            //string webRootPath = _env.WebRootPath; //E:\Visual Studio 2017\Projects\Факультет\Факультет\wwwroot
            //Debug.WriteLine(contentRootPath + "\n" + webRootPath);
            //Console.WriteLine(contentRootPath + "\n" + webRootPath);

            var sd = new SeedData(_env);
            var cathedras = sd.GetItems<Cathedra>("Кафедры.txt");
            modelBuilder.Entity<Cathedra>().HasData(cathedras);

            var subjects = sd.GetItems<Subject>("Предметы.txt");
            modelBuilder.Entity<Subject>().HasData(subjects);

            var employees = sd.GetItems<Employee>("Преподаватели.txt");
            var rand = new Random();
            foreach (var e in employees)
            {
                e.CathedraId = rand.Next(1, cathedras.Count() + 1);//The exclusive upper bound
                                                                   // Debug.WriteLine($"Id={e.Id} CathedraId={e.CathedraId} Name={e.Name}");

            }
            modelBuilder.Entity<Employee>().HasData(employees);

            var groups = sd.GetItems<Group>("Группы.txt");
            rand = new Random();
            foreach (var e in groups)
            {
                e.CathedraId = rand.Next(1, cathedras.Count() + 1);//The exclusive upper bound
                e.CourseId = rand.Next(1, 6); // +1 !!! 1-5
                //Console.WriteLine($"Id={e.Id} CathedraId={e.CathedraId} Name={e.Name}");
            }
            modelBuilder.Entity<Group>().HasData(groups);

            var students = sd.GetItems<Student>("Студенты.txt");
            rand = new Random();
            foreach (var e in students)
            {

                e.GroupId = rand.Next(1, groups.Count() + 1);//The exclusive upper bound
                e.PaymentTypeId = rand.Next(1, 3); // +1 !!!
                e.IsActive = true;
            }
            modelBuilder.Entity<Student>().HasData(students);

            var st = new Statement() { Id = 1, EmployeeId = employees[0].Id, GroupId = groups[0].Id, SubjectId = subjects[0].Id, PeriodBegin=DateTime.Now, PeriodEnd= DateTime.Now.AddDays(15) };
            modelBuilder.Entity<Statement>().HasData(st);

        }
    }
    // здесь можно изменить названия ролей и не придется менять доступы в атрибутах Authorize, 
    //так как используем Policy (см. Startup options.AddPolicy)
    public static class BuiltinRoles
    {
        public static readonly string Administrator = "Администратор";
        public static readonly string Employee = "Сотрудник";
        public static readonly string Student = "Студент";
    }


    //drop-Database -context ApplicationDbContext -whatif
    //public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>// versh
    //{
    //    //  Add-Migration SeedRoles
    //    public void Configure(EntityTypeBuilder<IdentityRole> modelBuilder)
    //    {
    //        modelBuilder.HasData(
    //            new IdentityRole("Student") { NormalizedName = "STUDENT" },
    //            new IdentityRole("Employee") { NormalizedName = "EMPLOYEE" },
    //            new IdentityRole("Administrator") { NormalizedName = "ADMINISTRATOR" }
    //        );
    //    }
    //}
}
