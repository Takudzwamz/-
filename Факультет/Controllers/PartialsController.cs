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
    public class PartialController : Controller
    {
        private readonly ApplicationDbContext _context;
        public PartialController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: MarksController/Create ============================================
        [Route("Partial/{filter}")]
        public ActionResult Partial(string filter)
        {
           var res= _context.Students.Where(s=>s.Name.Contains(filter)).ToList();
            return PartialView("IndexPartial", res);
        }


    }
}
