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
        public async Task<IActionResult> Index()
        {
            var areasDataBaseContext = _context.ResidentialBuilding.Include(r => r.Street);
            return View(await areasDataBaseContext.ToListAsync());
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
