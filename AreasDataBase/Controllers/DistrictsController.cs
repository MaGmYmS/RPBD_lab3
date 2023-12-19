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
    public class DistrictsController : Controller
    {
        private readonly AreasDataBaseContext _context;

        public DistrictsController(AreasDataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string searchColumn, string sortOrder)
        {
            ViewData["NameDistrictSortParam"] = String.IsNullOrEmpty(sortOrder) ? "nameDistrict_desc" : "";
            ViewData["CityNameSortParam"] = sortOrder == "CityName" ? "cityName_desc" : "CityName";

            IQueryable<District> districtsQuery = _context.District.Include(d => d.City);

            switch (sortOrder)
            {
                case "nameDistrict_desc":
                    districtsQuery = districtsQuery.OrderByDescending(d => d.NameDistrict);
                    break;
                case "CityName":
                    districtsQuery = districtsQuery.OrderBy(d => d.City.NameCity);
                    break;
                case "cityName_desc":
                    districtsQuery = districtsQuery.OrderByDescending(d => d.City.NameCity);
                    break;
                default:
                    districtsQuery = districtsQuery.OrderBy(d => d.NameDistrict);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                // Используйте выбранный столбец для поиска
                switch (searchColumn)
                {
                    case "nameDistrict":
                        districtsQuery = districtsQuery.Where(s => s.NameDistrict.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "cityName":
                        districtsQuery = districtsQuery.Where(s => s.City.NameCity.ToLower().Contains(searchString.ToLower()));
                        break;
                }
            }

            return View(await districtsQuery.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDistrict,NameDistrict,CityId")] District district)
        {
            if (ModelState.IsValid)
            {
                // Проверяем уникальность сочетания района и города
                if (_context.District.Any(d => d.NameDistrict == district.NameDistrict && d.CityId == district.CityId))
                {
                    ModelState.AddModelError("NameDistrict", "Такой район уже существует в выбранном городе.");
                }
                else
                {
                    _context.Add(district);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity", district.CityId);
            return View(district);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var district = await _context.District.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity", district.CityId);
            return View(district);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDistrict,NameDistrict,CityId")] District district)
        {
            if (id != district.IdDistrict)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Проверяем уникальность сочетания района и города
                if (_context.District.Any(d => d.IdDistrict != district.IdDistrict && d.NameDistrict == district.NameDistrict && d.CityId == district.CityId))
                {
                    ModelState.AddModelError("NameDistrict", "Такой район уже существует в выбранном городе.");
                }
                else
                {
                    try
                    {
                        _context.Update(district);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!DistrictExists(district.IdDistrict))
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
            }
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity", district.CityId);
            return View(district);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var district = await _context.District
                .Include(d => d.City)
                .FirstOrDefaultAsync(m => m.IdDistrict == id);
            if (district == null)
            {
                return NotFound();
            }

            return View(district);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var district = await _context.District.FindAsync(id);
            if (district != null)
            {
                _context.District.Remove(district);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DistrictExists(int id)
        {
            return _context.District.Any(e => e.IdDistrict == id);
        }
    }
}
