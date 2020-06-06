using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faculty.Data;
using Faculty.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;

namespace Faculty.Controllers
{
    public class PerformanceController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PerformanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult ViewAsPDF()
        {
            PerformanceViewModel pvm = new PerformanceViewModel();
            // находим студента для текущего пользователя, если он не студент, то ничего не выводим
            var userId = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
            var student = _context.Students.Where(e => e.AccountId == userId).FirstOrDefault();
            if (student != null)
            {
                pvm.ListLines = _context.Marks.Where(m=>m.StudentId== student.Id).Select(m => new PerformanceListItem() { Mark = m.MarkValue.Name, Subject = m.Statement.Subject.Name }).ToList();
                pvm.StudentName = student.Name;
            }

            return new ViewAsPdf("Index", pvm);
        }
    }
}
