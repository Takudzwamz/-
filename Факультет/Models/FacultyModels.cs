using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Faculty.Models
{
    // это не есть сущность базы
    public class Persion : EntityBase // возможный пользователь сайта
    {
        //Notice that we are following the convention  EntityId, where in our case the Entity is represented by the property called Account, which is of the IdentityUser type.
        // This is enough to tell Entity Framework Core that we want a relationship between the Student table and the AspNetUsers table. 
        [Metadata(ShowInList = false)]
        [ScaffoldColumn(false)]
        public string AccountId { get; set; } //foreign Key property
        [Metadata(ShowInList = false)]
        [ScaffoldColumn(false)]
        public virtual IdentityUser Account { get; set; }

        [NotMapped]
        [UIHint("Label")]
        [Display(Name = "Учетная запись", Order = 100)]
        public string AccountName { get { return Account?.ToString(); } }
    }


    [Display(Name = "Студенты", Description = "Студент")]
    public class Student : Persion
    {
        [Display(Name = "ФИО студента", Order = 10)]
        public override string Name { get; set; }

        [Display(Name = "Вид обучения")]
        [UIHint("DropBox")]
        [Metadata(ShowInList = false)]
        public int PaymentTypeId { get; set; } //foreign Key property to set not NULL
        [Display(Name = "Вид обучения", Order = 20)]
        public virtual PaymentType PaymentType { get; set; } // reference navigation property

        [NotMapped]
        [ScaffoldColumn(false)]
        [Display(Name = "Курс", Order = 30)]
        public Course Course { get { return Group?.Course; } }

        [Display(Name = "Группа")]
        [UIHint("DropBox")]
        [Metadata(ShowInList = false)]
        public int GroupId { get; set; }
        [Display(Name = "Группа", Order = 40)]
        public virtual Group Group { get; set; }

        [UIHint("Toggle")]
        [Display(Name = "Действующий", Order = 50)]
        public bool IsActive { get; set; }

        //public virtual ICollection<Mark> Marks { get; set; }
    }
    [Display(Name = "Преподаватели", Description = "Преподаватель")]
    public class Employee : Persion
    {
        [Display(Name = "ФИО преподавателя", Order = 10)]
        public override string Name { get; set; } // override to set own Display attribute

        [Display(Name = "Кафедра")]
        [UIHint("DropBox")]
        [Metadata(ShowInList = false)]
        public int CathedraId { get; set; }
        [Display(Name = "Кафедра", Order = 20)]
        public virtual Cathedra Cathedra { get; set; } //foreign Key property
        //public virtual ICollection<Statement> Statements { get; set; }
    }


    [Display(Name = "Виды обучения", Description = "Вид обучения")]
    public class PaymentType : EntityBase
    {
        public PaymentType()
        {
            // Students = new HashSet<Student>();
        }
        // [Display(Order = 10)]
        //[Metadata(ShowInList = false)]
        //public virtual ICollection<Student> Students { get; set; } //one-to-many relationship, collection navigation property
    }
    [Display(Name = "Кафедры", Description = "Кафедра")]
    public class Cathedra : EntityBase
    {
        //public virtual ICollection<Group> Groups { get; set; }
        //public virtual ICollection<Employee> Employees { get; set; }
    }
    [Display(Name = "Дисциплины", Description = "Дисциплина")]
    public class Subject : EntityBase
    {
        //public virtual ICollection<Statement> Statements { get; set; }
    }
    [Display(Name = "Курсы", Description = "Курс")]
    public class Course : EntityBase
    {
        //private string name;
        [NotMapped]
        public override string Name
        {
            get { return _Number.ToString(); }
            set { int.TryParse(value, out int res); _Number = res; }
        }
        int _Number;
        [Display(Name = "Курс")]
        public int Number
        {
            get { return _Number; }
            set { _Number = value; Name = value.ToString(); }
        }
        //public virtual ICollection<Group> Groups { get; set; }
        //public override string ToString()
        //{
        //    return Number.ToString();
        //}
    }
    [Display(Name = "Группы", Description = "Группа")]
    public class Group : EntityBase
    {
        [Display(Name = "Курс")]
        [UIHint("DropBox")]
        [Metadata(ShowInList = false)] //!!!!
        public int CourseId { get; set; }
        [Display(Name = "Курс", Order = 10)]
        public virtual Course Course { get; set; }

        [Display(Name = "Кафедра")]
        [UIHint("DropBox")]
        [Metadata(ShowInList = false)]
        public int CathedraId { get; set; }
        [Display(Name = "Кафедра", Order = 20)]
        public virtual Cathedra Cathedra { get; set; }
        //[Metadata(ShowInList = false)]
        //public  ICollection<Statement> Statements { get; set; }
        //[Metadata(ShowInList = false)]
        //public  ICollection<Student> Students { get; set; }
    }
    [Display(Name = "Ведомости", Description = "Ведомость")]
    public class Statement : EntityBase
    {
        [NotMapped]
        [Metadata(ShowInList = false)]
        [HiddenInput(DisplayValue = false)]
        public override string Name { get; set; } = "_";

        [Metadata(ShowInList = false)]
        [UIHint("DropBox")]
        [Display(Name = "Группа")]
        public int GroupId { get; set; }
        [Display(Name = "Группа", Order = 10)]
        public virtual Group Group { get; set; }
        [NotMapped]
        [ScaffoldColumn(false)]
        [Display(Name = "Курс", Order =15)]
        public string Course { get { return Group?.Course.Name; } }

        [Metadata(ShowInList = false)]
        [UIHint("DropBox")]
        [Display(Name = "Предмет")]
        public int SubjectId { get; set; }
        [Display(Name = "Предмет", Order = 20)]
        public virtual Subject Subject { get; set; }

        [Metadata(ShowInList = false)]
        [UIHint("DropBox")]
        [Display(Name = "Преподаватель")]
        public int EmployeeId { get; set; }
        [Display(Name = "Преподаватель", Order = 30)]
        public virtual Employee Employee { get; set; }

        [Display(Name = "Начало периода", Order = 40)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime PeriodBegin { get; set; }
        [Display(Name = "Конец периода", Order = 50)]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}")]
        public DateTime PeriodEnd { get; set; }
        [Metadata(ShowInList = false)]
        public virtual ICollection<Mark> Marks { get; set; }
    }
    public class StatementViewModel
    {
        public StatementViewModel()
        {
        }
        public Statement Statement { get; set; }
        public List<Mark> Marks { get; set; }
    }
    [Display(Name = "Оценки", Description = "Оценка")]
    public class Mark : EntityBase
    {
        [NotMapped]
        [HiddenInput(DisplayValue = false)]
        [Metadata(ShowInList = false)]
        public override string Name { get; set; } = "_";

        [Metadata(ShowInList = false)]
        public int StatementId { get; set; }
        [Display(Name = "Ведомость")]
        [Metadata(ShowInList = false)]
        [ScaffoldColumn(false)]
        public virtual Statement Statement { get; set; }

        [Metadata(ShowInList = false)]
        // [ScaffoldColumn(false)]
        public int StudentId { get; set; }
        [UIHint("Label")]
        [Display(Name = "Студент", Order = 10)]
        public virtual Student Student { get; set; }

        [Metadata(ShowInList = false)]
        [UIHint("DropBox")]
        [Display(Name = "Оценка")]
        public int MarkValueId { get; set; }
        [Display(Name = "Оценка", Order = 20)]
        public virtual MarkValue MarkValue { get; set; }
    }

    public class MarkValue : EntityBase
    {
    }

    //public enum MarkEnum
    //{
    //    Неудовлетворительно = 2,
    //    Удовлетворительно = 3,
    //    Хорошо = 4,
    //    Отлично = 5
    //}
}
