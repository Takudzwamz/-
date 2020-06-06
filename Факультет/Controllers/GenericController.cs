using Faculty.Data;
using Faculty.Models;
using Faculty.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;

namespace Faculty.Controllers
{
    [Route("[controller]")]
    [GenericControllerName]
    [Authorize]
    public class GenericController<T> : Controller where T : class, IEntityBase, new()
    {
        private readonly ApplicationDbContext _context;
        private readonly IModelMetadataProvider _provider;
        public GenericController(ApplicationDbContext context, IModelMetadataProvider provider)
        {
            _context = context;
            _provider = provider;
        }

        //   /Views/Cathedra/Index.cshtml
        //   /Views/Shared/Index.cshtml
        //   /Pages/Shared/Index.cshtml
        //[Route("[controller]")]
        public IActionResult Index()
        {
            return View(typeof(T));
        }
        // СПИСОК  !!!!!!!!!!! Для ведомостей показать только для данного преподавателя
        [Route("GetPartial/{filter?}")]
        public async Task<IActionResult> GetPartial(string filter)
        {
            TempData["filter"] = filter;
            List<T> l;
            var t = typeof(T);
            if (t == typeof(Statement))
            {
                if (User.IsInRole(BuiltinRoles.Administrator))
                {
                    l = await _context.Set<T>().ToListAsync();
                }
                else
                {
                    var userId = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
                    var employee = _context.Employees.Where(e => e.AccountId == userId).First();
                    var ll = await _context.Statements.Include("Group.Course").Where(s => s.EmployeeId == employee.Id).Select(x => x).ToListAsync();
                    //return View(ll);
                    return PartialView("IndexPartial", ll);
                }
            }

            else if (t == typeof(Student))
            {
                List<Student> res;
                if (filter == null)
                {
                    res = _context.Students.ToList();
                }
                else
                {
                    res = _context.Students.Where(s => s.Name.Contains(filter)).ToList();
                }
                return PartialView("IndexPartial", res);


            }
            else if (t == typeof(Mark))
            {
                l = await _context.Set<T>().ToListAsync();
            }
            else
            {
                l = await _context.Set<T>().OrderBy(x => x.Name).Select(x => x).ToListAsync(); //Microsoft.EntityFrameworkCore
            }

            //return View(l);
            return PartialView("IndexPartial", l);
        }

        // НОВЫЙ
        [Route("Create")]
        public IActionResult Create()
        {
            var t = typeof(T);
            if (t == typeof(Statement))
            {
                var userId = _context.Users.First(u => u.UserName == User.Identity.Name).Id;
                var employee = _context.Employees.Where(e => e.AccountId == userId).First();
                var st = new Statement()
                {
                    EmployeeId = employee.Id,
                    Employee = employee
                };

                ViewData["EntityName"] = st.GetType().Name;
                return View(st);
            }
            else
            {
            T item = new T();
            ViewData["EntityName"] = typeof(T).Name;
            return View(item);
            }


        }
        [Route("MarkCreate/{StatementId}/{StudentId}")]
        public IActionResult MarkCreate(int StatementId, int StudentId)
        {

            Mark item = new Mark() { StatementId = StatementId, StudentId = StudentId };
            item.Student = _context.Students.Find(StudentId);
            item.Statement = _context.Statements.Find(StatementId);

            ViewData["EntityName"] = "Оценка";
            return View("CreateMark", item);
        }

        //[HttpPost]
        //[Route("MarkCreatePost")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> MarkCreatePost(Mark entity)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(entity);
        //        await _context.SaveChangesAsync();
        //        return Redirect($"/Statement/Edit/{entity.StatementId}");
        //    }
        //    return Redirect("/Statement");
        //}



        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Route("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(T entity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();


                if (entity is Mark mark) // вернуться на редактирование ведомости после редактирования оценки
                {
                    return Redirect($"/Statement/Edit/{mark.StatementId}");
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }

            }
            return View(entity);
        }

        // РЕДАКТИРОВАТЬ
        [Route("Edit/{id?}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var tcontext = _context.Set<T>();
            var entity = await tcontext.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            if (entity is Statement sst) // для ведомости вид StatementEdit и модель StatementViewModel
            {
                var stEdit = new StatementViewModel() { Statement = sst };
                var setMmarks = _context.Set<Mark>().Where(m => m.StatementId == sst.Id).ToList(); // имеющиеся отметки 
                // срисок отметок для всех студентов группы
                var allMarks = _context.Set<Student>().Where(s => s.GroupId == sst.GroupId).Select(s => new Mark()
                {
                    Id = 0,
                    StatementId = sst.Id,
                    StudentId = s.Id
                }).ToList();
                var unMarks = setMmarks.Union(allMarks, new MarkComparer()).ToList();
                foreach (var m in unMarks)
                {
                    if (m.Id == 0)
                    {
                        m.Student = _context.Set<Student>().Find(m.StudentId);
                    }
                }
                stEdit.Marks = unMarks.OrderBy(m => m.Student.Name).ToList();
                return View("StatementEdit", stEdit); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            }
            else
            if (entity is Mark mrk)
            {
                return View("EditMark", entity);
            }
            else
            {
                return View(entity);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("StatementEdit/{id?}")]
        public async Task<IActionResult> StatementEdit(int id, StatementViewModel se)
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] T item) // тут толко указанные поля заполнит, остальные пустые (нулевые)
        {
            var item = se.Statement;
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id?}")]
        public async Task<IActionResult> Edit(int id, T item)
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] T item) // тут толко указанные поля заполнит, остальные пустые (нулевые)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TypeExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                    return RedirectToAction(nameof(Index));
            }
            return View(item);
        }
        private bool TypeExists(int id)
        {
            return _context.Set<T>().Any(e => e.Id == id);
        }

        // УДАЛИТЬ
        // GET: PaymentTypes/Delete/5
        [Route("Delete/{id?}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Set<T>().FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: PaymentTypes/Delete/5
        [Route("Delete/{id?}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            T item = null;
            try
            {
                item = await _context.Set<T>().FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                _context.Set<T>().Remove(item);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.GetBaseException().Message;
                return View(item);
            }
        }
    }

    // Типы, обрабатываемые этим контроллером
    public static class IncludedEntities
    {
        public static IReadOnlyList<TypeInfo> Types = new List<TypeInfo>
            {
                typeof(PaymentType).GetTypeInfo(),
                typeof(Employee).GetTypeInfo(),
                typeof(Cathedra).GetTypeInfo(),
                typeof(Subject).GetTypeInfo(),
                typeof(Group).GetTypeInfo(),
                typeof(Student).GetTypeInfo(),
                typeof(Statement).GetTypeInfo(),
                //typeof(Mark).GetTypeInfo()
            };
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GenericControllerNameAttribute : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() == typeof(GenericController<>))
            {
                var entityType = controller.ControllerType.GenericTypeArguments[0];
                controller.ControllerName = entityType.Name;
            }
        }
    }

    public class GenericControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            // Get the list of entities that we want to support for the generic controller
            foreach (var entityType in IncludedEntities.Types)
            {
                var typeName = entityType.Name + "Controller";

                // Check to see if there is a "real" controller for this class
                if (!feature.Controllers.Any(t => t.Name == typeName))
                {
                    // Create a generic controller for this type
                    var controllerType = typeof(GenericController<>).MakeGenericType(entityType.AsType()).GetTypeInfo();
                    feature.Controllers.Add(controllerType);
                }
            }
        }
    }

    public class MarkComparer : IEqualityComparer<Mark>
    {
        public bool Equals([AllowNull] Mark x, [AllowNull] Mark y)
        {
            return x.StatementId == y.StatementId && x.StudentId == y.StudentId;
        }

        public int GetHashCode([DisallowNull] Mark obj)
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + obj.StudentId.GetHashCode();
                hash = hash * 23 + obj.StatementId.GetHashCode();
                return hash;
            }
        }
    }
}
