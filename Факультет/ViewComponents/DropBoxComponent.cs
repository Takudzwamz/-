using Faculty.Data;
using Faculty.Utils;
using Faculty.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Faculty.ViewComponents
{
    public class DropBoxViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext db;

        public DropBoxViewComponent(ApplicationDbContext context)
        {
            db = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string PropertyName, int SelectedId)//CathedraId
        {
            var entityName = PropertyName.Replace("Id", "");//Cathedra
            var items = await GetItemsAsync(entityName, SelectedId);
            return View(items);
        }
        private Task<List<SelectListItem>> GetItemsAsync(string entityName, int SelectedId)
        {

            return db.GetContextByName(entityName).Select(c => new SelectListItem() { Selected = c.Id == SelectedId, Text = c.Name, Value = c.Id.ToString() }).ToListAsync();
            // return db.Cathedras.Select(c=>new SelectListItem() { Text=c.Name, Value=c.Id.ToString() }).ToListAsync();
        }
    }
}
