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
    public class CitiesController : Controller
    {
        private readonly AreasDataBaseContext _context;

        public CitiesController(AreasDataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string searchColumn, string sortOrder)
        {
            ViewData["NameCitySortParam"] = String.IsNullOrEmpty(sortOrder) ? "nameCity_desc" : "";
            ViewData["PostalCodeSortParam"] = sortOrder == "PostalCode" ? "postalCode_desc" : "PostalCode";
            ViewData["AreaNameSortParam"] = sortOrder == "AreaName" ? "areaName_desc" : "AreaName";

            IQueryable<City> citiesQuery = _context.City.Include(a => a.Area);

            switch (sortOrder)
            {
                case "nameCity_desc":
                    citiesQuery = citiesQuery.OrderByDescending(c => c.NameCity);
                    break;
                case "PostalCode":
                    citiesQuery = citiesQuery.OrderBy(c => c.PostalCode);
                    break;
                case "postalCode_desc":
                    citiesQuery = citiesQuery.OrderByDescending(c => c.PostalCode);
                    break;
                case "AreaName":
                    citiesQuery = citiesQuery.OrderBy(c => c.Area.NameArea);
                    break;
                case "areaName_desc":
                    citiesQuery = citiesQuery.OrderByDescending(c => c.Area.NameArea);
                    break;
                default:
                    citiesQuery = citiesQuery.OrderBy(c => c.NameCity);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchColumn)
                {
                    case "nameCity":
                        citiesQuery = citiesQuery.Where(s => s.NameCity.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "postalCode":
                        if (int.TryParse(searchString, out int code))
                        {
                            citiesQuery = citiesQuery.Where(s => s.PostalCode == code);
                        }
                        break;
                    case "areaName":
                        citiesQuery = citiesQuery.Where(s => s.Area.NameArea.ToLower().Contains(searchString.ToLower()));
                        break;
                }
            }

            return View(await citiesQuery.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Area, "IdArea", "NameArea");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCity,NameCity,PostalCode,AreaId")] City city)
        {
            if (ModelState.IsValid)
            {
                // Проверяем уникальность названия города
                if (_context.City.Any(c => c.NameCity.ToLower() == city.NameCity.ToLower()))
                {
                    ModelState.AddModelError("NameCity", "Такой город уже существует.");
                }
                else
                {
                    _context.Add(city);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["AreaId"] = new SelectList(_context.Area, "IdArea", "NameArea", city.AreaId);
            return View(city);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.City.FindAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            ViewData["AreaId"] = new SelectList(_context.Area, "IdArea", "NameArea", city.AreaId);
            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCity,NameCity,PostalCode,AreaId")] City city)
        {
            if (id != city.IdCity)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Проверяем уникальность названия города
                if (_context.City.Any(c => c.IdCity != city.IdCity && c.NameCity.ToLower() == city.NameCity.ToLower()))
                {
                    ModelState.AddModelError("NameCity", "Такой город уже существует.");
                }
                else
                {
                    try
                    {
                        _context.Update(city);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CityExists(city.IdCity))
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
            ViewData["AreaId"] = new SelectList(_context.Area, "IdArea", "NameArea", city.AreaId);
            return View(city);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.City
                .Include(c => c.Area)
                .FirstOrDefaultAsync(m => m.IdCity == id);
            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.City.FindAsync(id);
            if (city != null)
            {
                _context.City.Remove(city);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CityExists(int id)
        {
            return _context.City.Any(e => e.IdCity == id);
        }
    }
}
