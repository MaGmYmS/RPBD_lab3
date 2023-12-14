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
    public class StreetsController : Controller
    {
        private readonly AreasDataBaseContext _context;

        public StreetsController(AreasDataBaseContext context)
        {
            _context = context;
        }

        // GET: Streets
        public async Task<IActionResult> Index(string searchString, string searchColumn, string sortOrder)
        {
            ViewData["NameStreetSortParam"] = String.IsNullOrEmpty(sortOrder) ? "nameStreet_desc" : "";
            ViewData["DistrictNameSortParam"] = sortOrder == "DistrictName" ? "districtName_desc" : "DistrictName";

            IQueryable<Street> streetsQuery = _context.Street.Include(s => s.District);

            switch (sortOrder)
            {
                case "nameStreet_desc":
                    streetsQuery = streetsQuery.OrderByDescending(s => s.NameStreet);
                    break;
                case "DistrictName":
                    streetsQuery = streetsQuery.OrderBy(s => s.District.NameDistrict);
                    break;
                case "districtName_desc":
                    streetsQuery = streetsQuery.OrderByDescending(s => s.District.NameDistrict);
                    break;
                default:
                    streetsQuery = streetsQuery.OrderBy(s => s.NameStreet);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                // Используйте выбранный столбец для поиска
                switch (searchColumn)
                {
                    case "nameStreet":
                        streetsQuery = streetsQuery.Where(s => s.NameStreet.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "districtName":
                        streetsQuery = streetsQuery.Where(s => s.District.NameDistrict.ToLower().Contains(searchString.ToLower()));
                        break;
                }
            }

            return View(await streetsQuery.ToListAsync());
        }


        // GET: Streets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var street = await _context.Street
                .Include(s => s.District)
                .FirstOrDefaultAsync(m => m.IdStreet == id);
            if (street == null)
            {
                return NotFound();
            }

            return View(street);
        }

        // GET: Streets/Create
        public IActionResult Create()
        {
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict");
            return View();
        }

        // POST: Streets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdStreet,NameStreet,DistrictId")] Street street)
        {
            if (ModelState.IsValid)
            {
                _context.Add(street);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict", street.DistrictId);
            return View(street);
        }

        // GET: Streets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var street = await _context.Street.FindAsync(id);
            if (street == null)
            {
                return NotFound();
            }
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict", street.DistrictId);
            return View(street);
        }

        // POST: Streets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdStreet,NameStreet,DistrictId")] Street street)
        {
            if (id != street.IdStreet)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(street);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StreetExists(street.IdStreet))
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
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict", street.DistrictId);
            return View(street);
        }

        // GET: Streets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var street = await _context.Street
                .Include(s => s.District)
                .FirstOrDefaultAsync(m => m.IdStreet == id);
            if (street == null)
            {
                return NotFound();
            }

            return View(street);
        }

        // POST: Streets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var street = await _context.Street.FindAsync(id);
            if (street != null)
            {
                _context.Street.Remove(street);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StreetExists(int id)
        {
            return _context.Street.Any(e => e.IdStreet == id);
        }
    }
}
