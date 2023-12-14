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
    public class ResidentialBuildingsController : Controller
    {
        private readonly AreasDataBaseContext _context;

        public ResidentialBuildingsController(AreasDataBaseContext context)
        {
            _context = context;
        }

        // GET: ResidentialBuildings
        public async Task<IActionResult> Index(string searchString, string searchColumn, string sortOrder)
        {
            ViewData["HouseNumberSortParam"] = String.IsNullOrEmpty(sortOrder) ? "houseNumber_desc" : "";
            ViewData["YearOfConstructionSortParam"] = sortOrder == "YearOfConstruction" ? "yearOfConstruction_desc" : "YearOfConstruction";
            ViewData["NumbersOfFloorsSortParam"] = sortOrder == "NumbersOfFloors" ? "numbersOfFloors_desc" : "NumbersOfFloors";
            ViewData["StreetNameSortParam"] = sortOrder == "StreetName" ? "streetName_desc" : "StreetName";

            IQueryable<ResidentialBuilding> residentialBuildingsQuery = _context.ResidentialBuilding.Include(r => r.Street);

            switch (sortOrder)
            {
                case "houseNumber_desc":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderByDescending(r => r.HouseNumber);
                    break;
                case "YearOfConstruction":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderBy(r => r.YearOfConstruction);
                    break;
                case "yearOfConstruction_desc":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderByDescending(r => r.YearOfConstruction);
                    break;
                case "NumbersOfFloors":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderBy(r => r.NumbersOfFloors);
                    break;
                case "numbersOfFloors_desc":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderByDescending(r => r.NumbersOfFloors);
                    break;
                case "StreetName":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderBy(r => r.Street.NameStreet);
                    break;
                case "streetName_desc":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderByDescending(r => r.Street.NameStreet);
                    break;
                default:
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderBy(r => r.HouseNumber);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                // Используйте выбранный столбец для поиска
                switch (searchColumn)
                {
                    case "houseNumber":
                        residentialBuildingsQuery = residentialBuildingsQuery.Where(s => s.HouseNumber.ToString().Contains(searchString.ToLower()));
                        break;
                    case "YearOfConstruction":
                        residentialBuildingsQuery = residentialBuildingsQuery.Where(s => s.YearOfConstruction.ToString().Contains(searchString.ToLower()));
                        break;
                    case "NumbersOfFloors":
                        residentialBuildingsQuery = residentialBuildingsQuery.Where(s => s.NumbersOfFloors.ToString().Contains(searchString.ToLower()));
                        break;
                    case "streetName":
                        residentialBuildingsQuery = residentialBuildingsQuery.Where(s => s.Street.NameStreet.ToLower().Contains(searchString.ToLower()));
                        break;
                }
            }

            return View(await residentialBuildingsQuery.ToListAsync());
        }


        // GET: ResidentialBuildings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residentialBuilding = await _context.ResidentialBuilding
                .Include(r => r.Street)
                .FirstOrDefaultAsync(m => m.IdResidentialBuilding == id);
            if (residentialBuilding == null)
            {
                return NotFound();
            }

            return View(residentialBuilding);
        }

        // GET: ResidentialBuildings/Create
        public IActionResult Create()
        {
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet");
            return View();
        }

        // POST: ResidentialBuildings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdResidentialBuilding,HouseNumber,YearOfConstruction,NumbersOfFloors,StreetId")] ResidentialBuilding residentialBuilding)
        {
            if (ModelState.IsValid)
            {
                _context.Add(residentialBuilding);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet", residentialBuilding.StreetId);
            return View(residentialBuilding);
        }

        // GET: ResidentialBuildings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residentialBuilding = await _context.ResidentialBuilding.FindAsync(id);
            if (residentialBuilding == null)
            {
                return NotFound();
            }
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet", residentialBuilding.StreetId);
            return View(residentialBuilding);
        }

        // POST: ResidentialBuildings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdResidentialBuilding,HouseNumber,YearOfConstruction,NumbersOfFloors,StreetId")] ResidentialBuilding residentialBuilding)
        {
            if (id != residentialBuilding.IdResidentialBuilding)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(residentialBuilding);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResidentialBuildingExists(residentialBuilding.IdResidentialBuilding))
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
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet", residentialBuilding.StreetId);
            return View(residentialBuilding);
        }

        // GET: ResidentialBuildings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residentialBuilding = await _context.ResidentialBuilding
                .Include(r => r.Street)
                .FirstOrDefaultAsync(m => m.IdResidentialBuilding == id);
            if (residentialBuilding == null)
            {
                return NotFound();
            }

            return View(residentialBuilding);
        }

        // POST: ResidentialBuildings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var residentialBuilding = await _context.ResidentialBuilding.FindAsync(id);
            if (residentialBuilding != null)
            {
                _context.ResidentialBuilding.Remove(residentialBuilding);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResidentialBuildingExists(int id)
        {
            return _context.ResidentialBuilding.Any(e => e.IdResidentialBuilding == id);
        }
    }
}
