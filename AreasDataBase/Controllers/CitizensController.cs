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
    public class CitizensController : Controller
    {
        private readonly AreasDataBaseContext _context;

        public CitizensController(AreasDataBaseContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string searchColumn, string sortOrder)
        {
            ViewData["FullNameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "fullName_desc" : "";
            ViewData["PassportSortParam"] = sortOrder == "passport" ? "passport_desc" : "passport";
            ViewData["PhoneNumberSortParam"] = sortOrder == "phoneNumber" ? "phoneNumber_desc" : "phoneNumber";
            ViewData["DateOfBirthSortParam"] = sortOrder == "dateOfBirth" ? "dateOfBirth_desc" : "dateOfBirth";
            ViewData["GenderSortParam"] = sortOrder == "gender" ? "gender_desc" : "gender";
            ViewData["ApartmentSortParam"] = sortOrder == "apartment" ? "apartment_desc" : "apartment";

            IQueryable<Citizen> citizens = _context.Citizen.Include(a => a.Apartment);

            switch (sortOrder)
            {
                case "fullName_desc":
                    citizens = citizens.OrderByDescending(c => c.FullName);
                    break;
                case "passport":
                    citizens = citizens.OrderBy(c => c.PassportData);
                    break;
                case "passport_desc":
                    citizens = citizens.OrderByDescending(c => c.PassportData);
                    break;
                case "phoneNumber":
                    citizens = citizens.OrderBy(c => c.PhoneNumber);
                    break;
                case "phoneNumber_desc":
                    citizens = citizens.OrderByDescending(c => c.PhoneNumber);
                    break;
                case "dateOfBirth":
                    citizens = citizens.OrderBy(c => c.DateOfBirth);
                    break;
                case "dateOfBirth_desc":
                    citizens = citizens.OrderByDescending(c => c.DateOfBirth);
                    break;
                case "gender":
                    citizens = citizens.OrderBy(c => c.Gender);
                    break;
                case "gender_desc":
                    citizens = citizens.OrderByDescending(c => c.Gender);
                    break;
                case "apartment":
                    citizens = citizens.OrderBy(c => c.Apartment.ApartmentNumber);
                    break;
                case "apartment_desc":
                    citizens = citizens.OrderByDescending(c => c.Apartment.ApartmentNumber);
                    break;
                default:
                    citizens = citizens.OrderBy(c => c.FullName);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                // Используйте выбранный столбец для поиска
                switch (searchColumn)
                {
                    case "fullName":
                        citizens = citizens.Where(s => s.FullName.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "passportData":
                        citizens = citizens.Where(s => s.PassportData.ToString().ToLower().Contains(searchString.ToLower()));
                        break;
                    case "phoneNumber":
                        citizens = citizens.Where(s => s.PhoneNumber.ToString().ToLower().Contains(searchString.ToLower()));
                        break;
                    case "dateOfBirth":
                        citizens = citizens.Where(s => s.DateOfBirth.ToString().Contains(searchString.ToLower()));
                        break;
                    case "gender":
                        citizens = citizens.Where(s => s.Gender.ToString().ToLower().Contains(searchString.ToLower()));
                        break;
                    case "apartmentNumber":
                        citizens = citizens.Where(s => s.Apartment.ApartmentNumber.ToString().Contains(searchString.ToLower()));
                        break;
                        // Добавьте другие case для остальных столбцов, если необходимо
                }
            }

            return View(await citizens.ToListAsync());
        }



        // GET: Citizens/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizen = await _context.Citizen
                .Include(c => c.Apartment)
                .FirstOrDefaultAsync(m => m.IdCitizen == id);
            if (citizen == null)
            {
                return NotFound();
            }

            return View(citizen);
        }

        // GET: Citizens/Create
        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartment, "IdApartment", "ApartmentNumber");
            return View();
        }

        // POST: Citizens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdCitizen,FullName,PassportData,PhoneNumber,DateOfBirth,Gender,ApartmentId")] Citizen citizen)
        {
            if (ModelState.IsValid)
            {
                _context.Add(citizen);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartment, "IdApartment", "ApartmentNumber", citizen.ApartmentId);
            return View(citizen);
        }

        // GET: Citizens/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizen = await _context.Citizen.FindAsync(id);
            if (citizen == null)
            {
                return NotFound();
            }
            ViewData["ApartmentId"] = new SelectList(_context.Apartment, "IdApartment", "ApartmentNumber", citizen.ApartmentId);
            return View(citizen);
        }

        // POST: Citizens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdCitizen,FullName,PassportData,PhoneNumber,DateOfBirth,Gender,ApartmentId")] Citizen citizen)
        {
            if (id != citizen.IdCitizen)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(citizen);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitizenExists(citizen.IdCitizen))
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
            ViewData["ApartmentId"] = new SelectList(_context.Apartment, "IdApartment", "ApartmentNumber", citizen.ApartmentId);
            return View(citizen);
        }

        // GET: Citizens/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizen = await _context.Citizen
                .Include(c => c.Apartment)
                .FirstOrDefaultAsync(m => m.IdCitizen == id);
            if (citizen == null)
            {
                return NotFound();
            }

            return View(citizen);
        }

        // POST: Citizens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var citizen = await _context.Citizen.FindAsync(id);
            if (citizen != null)
            {
                _context.Citizen.Remove(citizen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitizenExists(int id)
        {
            return _context.Citizen.Any(e => e.IdCitizen == id);
        }
    }
}
