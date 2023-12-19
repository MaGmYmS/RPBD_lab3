using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AreasDataBase.Data;
using AreasDataBase.Models;

namespace AreasDataBase.Controllers
{
    public class AreasController : Controller
    {
        private readonly AreasDataBaseContext _context;

        public AreasController(AreasDataBaseContext context)
        {
            _context = context;
        }

        // GET: Areas
        public async Task<IActionResult> Index(string searchString, string searchColumn, string sortOrder)
        {
            ViewData["SubjectCodeSortParam"] = String.IsNullOrEmpty(sortOrder) ? "subjectCode_desc" : "";
            ViewData["NameAreaSortParam"] = sortOrder == "NameArea" ? "nameArea_desc" : "NameArea";

            IQueryable<Area> areasQuery = _context.Area;

            switch (sortOrder)
            {
                case "subjectCode_desc":
                    areasQuery = areasQuery.OrderByDescending(a => a.SubjectCode);
                    break;
                case "NameArea":
                    areasQuery = areasQuery.OrderBy(a => a.NameArea);
                    break;
                case "nameArea_desc":
                    areasQuery = areasQuery.OrderByDescending(a => a.NameArea);
                    break;
                default:
                    areasQuery = areasQuery.OrderBy(a => a.SubjectCode);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchColumn)
                {
                    case "subjectCode":
                        if (int.TryParse(searchString, out int code))
                        {
                            areasQuery = areasQuery.Where(a => a.SubjectCode == code);
                        }
                        break;
                    case "nameArea":
                        areasQuery = areasQuery.Where(a => a.NameArea.ToLower().Contains(searchString.ToLower()));
                        break;
                }
            }


            return View(await areasQuery.ToListAsync());
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdArea,SubjectCode,NameArea")] Area area)
        {
            if (ModelState.IsValid)
            {
                // Проверка на уникальность SubjectCode
                var existingArea = await _context.Area.FirstOrDefaultAsync(a => a.SubjectCode == area.SubjectCode);
                if (existingArea != null)
                {
                    ModelState.AddModelError("SubjectCode", "Код региона уже существует.");
                    return View(area);
                }

                _context.Add(area);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(area);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Area.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdArea,SubjectCode,NameArea")] Area area)
        {
            if (id != area.IdArea)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Проверка на уникальность SubjectCode, исключая текущую запись
                var existingAreaWithSameCode = await _context.Area.FirstOrDefaultAsync(a => a.SubjectCode == area.SubjectCode && a.IdArea != id);
                if (existingAreaWithSameCode != null)
                {
                    ModelState.AddModelError("SubjectCode", "Код региона уже существует.");
                    return View(area);
                }

                try
                {
                    _context.Update(area);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(area.IdArea))
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
            return View(area);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var area = await _context.Area
                .FirstOrDefaultAsync(m => m.IdArea == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var area = await _context.Area.FindAsync(id);
            if (area != null)
            {
                _context.Area.Remove(area);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AreaExists(int id)
        {
            return _context.Area.Any(e => e.IdArea == id);
        }
    }
}
