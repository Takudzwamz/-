using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Faculty.Data;
using Microsoft.AspNetCore.Mvc;

namespace Faculty.Controllers
{
    public class AutocompleteController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AutocompleteController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AutocompleteData(string pattern, string entityName)
        {// универсальный метод контроллера для фильтрации сущности, заданной строкой entityName и шаблоном поиска pattern
            try
            {
                // определение типа сущности по её имени
                var tType = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(t => t.Name == entityName);
                if (tType != null)
                {
                    // вызов метода Set<T>, где T задан строкой entityName
                    var SetMethod = typeof(ApplicationDbContext).GetMethod("Set");
                    var Set_T_Method = SetMethod.MakeGenericMethod(new[] { tType });
                    var dbSet = Set_T_Method.Invoke(_context, null);

                    List<object> suggestions = new List<object>();
                    foreach (var c in (IQueryable<dynamic>)dbSet)
                    {
                        string name = (string)c.Name;
                        if (name.ToUpper().Contains(pattern.ToUpper()))
                        {
                            suggestions.Add(new { label = c.Name, value = c.Id });
                        }
                    }

                    //suggestions = _context.PaymentTypes.Where(c => c.Name.Contains(pattern)).Select(c => new { label = c.Name, value = c.Id }).ToList<object>();
                    return Json(new { Result = "OK", data = suggestions }); //Json(countries, new Newtonsoft.Json.JsonSerializerSettings());или new System.Text.Json.JsonSerializerOptions()
                }
                else
                {
                    return Json(new { Result = "ERROR", Message = "Не найден тип сущности контекста:" + entityName });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", ex.Message });
            }



        }

    }
}
