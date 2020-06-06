using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Faculty.Data;
using Faculty.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Controllers
{
    [Authorize]
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public MarksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MarksController
        public async Task<IActionResult> Index()
        {
            var l = await _context.Marks.ToListAsync();
            return View(l);
        }

        // GET: MarksController/Create ============================================
        [Route("Marks/Create/{StatementId}/{StudentId}")]
        public ActionResult Create(int StatementId, int StudentId)
        {
            Mark item = new Mark() { StatementId = StatementId, StudentId = StudentId };
            item.Student = _context.Students.Find(StudentId);
            item.Statement = _context.Statements.Find(StatementId);

            ViewData["EntityName"] = "Оценка";
            return View("Create", item);
        }

        // POST: MarksController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Marks/CreateNew")]
        public async Task<IActionResult> Create(Mark entity)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(entity);
                    await _context.SaveChangesAsync();
                    return Redirect($"/Statement/Edit/{entity.StatementId}");
                }
                catch
                {
                    return View(entity);
                }
            }
            else
            {
                return View(entity);
            }
            //return Redirect("/Statement");
        }

        // GET: MarksController/Edit/5 ============================================
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var entity = await _context.Marks.FindAsync(id);
            if (entity == null)
            {
                return NotFound();
            }
            return View(entity);
        }

        // POST: Marks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mark item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }
            // хотим убрать оценку - пытаемся удалить из базы
            if (item.MarkValueId == 0)
            {
                return Redirect($"/Marks/Delete/{item.Id}");
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
                return Redirect($"/Statement/Edit/{item.StatementId}");
            }
            return View(item);
        }
        private bool TypeExists(int id)
        {
            return _context.Marks.Any(e => e.Id == id);
        }


        // GET: MarksController/Delete/5 ============================================
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var item = await _context.Marks.FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: MarksController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, IFormCollection collection)
        {
            Mark item = null;
            
            try
            {
                item = await _context.Marks.FindAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                int StatementId = item.StatementId;
                _context.Marks.Remove(item);
                await _context.SaveChangesAsync();

                return Redirect($"/Statement/Edit/{StatementId}");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.GetBaseException().Message;
                return View(item);
            }
        }
    }
}
