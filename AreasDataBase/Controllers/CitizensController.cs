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
            ViewData["CitySortParam"] = sortOrder == "city" ? "city_desc" : "city";
            ViewData["DistrictSortParam"] = sortOrder == "district" ? "district_desc" : "district";
            ViewData["StreetSortParam"] = sortOrder == "street" ? "street_desc" : "street";
            ViewData["HouseNumberSortParam"] = sortOrder == "houseNumber" ? "houseNumber_desc" : "houseNumber";

            IQueryable<Citizen> citizens = _context.Citizen
                 .Include(a => a.Apartment)
                 .ThenInclude(a => a.ResidentialBuilding)
                 .ThenInclude(rb => rb.Street)
                 .ThenInclude(s => s.District)
                 .ThenInclude(d => d.City);

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
                case "city":
                    citizens = citizens.OrderBy(c => c.Apartment.ResidentialBuilding.Street.District.City.NameCity);
                    break;
                case "city_desc":
                    citizens = citizens.OrderByDescending(c => c.Apartment.ResidentialBuilding.Street.District.City.NameCity);
                    break;
                case "district":
                    citizens = citizens.OrderBy(c => c.Apartment.ResidentialBuilding.Street.District.NameDistrict);
                    break;
                case "district_desc":
                    citizens = citizens.OrderByDescending(c => c.Apartment.ResidentialBuilding.Street.District.NameDistrict);
                    break;
                case "street":
                    citizens = citizens.OrderBy(c => c.Apartment.ResidentialBuilding.Street.NameStreet);
                    break;
                case "street_desc":
                    citizens = citizens.OrderByDescending(c => c.Apartment.ResidentialBuilding.Street.NameStreet);
                    break;
                case "houseNumber":
                    citizens = citizens.OrderBy(c => c.Apartment.ResidentialBuilding.HouseNumber);
                    break;
                case "houseNumber_desc":
                    citizens = citizens.OrderByDescending(c => c.Apartment.ResidentialBuilding.HouseNumber);
                    break;
                default:
                    citizens = citizens.OrderBy(c => c.FullName);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchColumn)
                {
                    case "fullName":
                        citizens = citizens.Where(s => s.FullName.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "passportData":
                        if (long.TryParse(searchString, out long passportData))
                        {
                            citizens = citizens.Where(s => s.PassportData == passportData);
                        }
                        break;

                    case "phoneNumber":
                        if (long.TryParse(searchString, out long phoneNumber))
                        {
                            citizens = citizens.Where(s => s.PhoneNumber == phoneNumber);
                        }
                        break;

                    case "dateOfBirth":
                        DateTime searchDate;
                        if (DateTime.TryParse(searchString, out searchDate))
                        {
                            searchDate = DateTime.SpecifyKind(searchDate, DateTimeKind.Utc);
                            citizens = citizens.Where(s => s.DateOfBirth.Date == searchDate.Date);
                        }
                        break;



                    case "gender":
                        if (searchString.Equals("Мужской", StringComparison.OrdinalIgnoreCase))
                        {
                            citizens = citizens.Where(s => s.Gender == true);
                        }
                        else if (searchString.Equals("Женский", StringComparison.OrdinalIgnoreCase))
                        {
                            citizens = citizens.Where(s => s.Gender == false);
                        }
                        break;

                    case "apartmentNumber":
                        if (int.TryParse(searchString, out int apartmentNum))
                        {
                            citizens = citizens.Where(s => s.Apartment.ApartmentNumber == apartmentNum);
                        }
                        break;
                    case "city":
                        citizens = citizens.Where(s => s.Apartment.ResidentialBuilding.Street.District.City.NameCity.ToLower().Contains(searchString.ToLower()));
                        break;

                    case "district":
                        citizens = citizens.Where(s => s.Apartment.ResidentialBuilding.Street.District.NameDistrict.ToLower().Contains(searchString.ToLower())).AsQueryable();
                        break;
                    case "street":
                        citizens = citizens.Where(s => s.Apartment.ResidentialBuilding.Street.NameStreet.ToLower().Contains(searchString.ToLower())).AsQueryable();
                        break;
                    //case "houseNumber":

                    //    if (!string.IsNullOrEmpty(searchString))
                    //    {
                    //        citizens = citizens
                    //            .Where(c => c.Apartment.ResidentialBuilding.HouseNumber.ToString().Contains(searchString));
                    //    }


                    //    //citizens = citizens
                    //    //    .Where(c => EF.Functions.Like(c.Apartment.ResidentialBuilding.HouseNumber.ToString(), $"%{searchString}%"));
                    //    break;



                }

            }
            var result = await citizens.ToListAsync();

            return View(result);
        }


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

        public IActionResult Create()
        {
            ViewData["ApartmentId"] = new SelectList(_context.Apartment, "IdApartment", "ApartmentNumber");
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity");
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict");
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet");
            ViewData["ResidentialBuildingId"] = new SelectList(_context.ResidentialBuilding, "IdResidentialBuilding", "HouseNumber");

            return View();
        }


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
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity");
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict");
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet");
            ViewData["ResidentialBuildingId"] = new SelectList(_context.ResidentialBuilding, "IdResidentialBuilding", "HouseNumber");
            return View(citizen);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var citizen = await _context.Citizen
                .Include(c => c.Apartment)
                .ThenInclude(a => a.ResidentialBuilding)
                .ThenInclude(rb => rb.Street)
                .ThenInclude(s => s.District)
                .ThenInclude(d => d.City)
                .FirstOrDefaultAsync(c => c.IdCitizen == id);

            if (citizen == null)
            {
                return NotFound();
            }

            // Загрузка списков для дропдаунов
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity", citizen?.Apartment?.ResidentialBuilding?.Street?.District?.CityId);
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict", citizen?.Apartment?.ResidentialBuilding?.Street?.DistrictId);
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet", citizen?.Apartment?.ResidentialBuilding?.StreetId);
            ViewData["ResidentialBuildingId"] = new SelectList(_context.ResidentialBuilding, "IdResidentialBuilding", "HouseNumber", citizen?.Apartment?.ResidentialBuildingId);
            ViewData["ApartmentId"] = new SelectList(_context.Apartment, "IdApartment", "ApartmentNumber", citizen.ApartmentId);

            return View(citizen);
        }

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

            // Загрузка списков для дропдаунов в случае ошибки валидации
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity", citizen?.Apartment?.ResidentialBuilding?.Street?.District?.CityId);
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict", citizen?.Apartment?.ResidentialBuilding?.Street?.DistrictId);
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet", citizen?.Apartment?.ResidentialBuilding?.StreetId);
            ViewData["ResidentialBuildingId"] = new SelectList(_context.ResidentialBuilding, "IdResidentialBuilding", "HouseNumber", citizen?.Apartment?.ResidentialBuildingId);
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
