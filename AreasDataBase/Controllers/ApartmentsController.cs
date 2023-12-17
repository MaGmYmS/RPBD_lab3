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
    public class ApartmentsController : Controller
    {
        private readonly AreasDataBaseContext _context;

        public ApartmentsController(AreasDataBaseContext context)
        {
            _context = context;
        }

        // GET: Apartments
        public async Task<IActionResult> Index(string searchString, string searchColumn, string sortOrder)
        {
            ViewData["ApartmentNumberSortParam"] = String.IsNullOrEmpty(sortOrder) ? "apartmentNumber_desc" : "";
            ViewData["NumberOfRoomsSortParam"] = sortOrder == "NumberOfRooms" ? "numberOfRooms_desc" : "NumberOfRooms";
            ViewData["AreaSortParam"] = sortOrder == "Area" ? "area_desc" : "Area";
            ViewData["ResidentialBuildingSortParam"] = sortOrder == "ResidentialBuilding" ? "residentialBuilding_desc" : "ResidentialBuilding";
            ViewData["CitySortParam"] = sortOrder == "City" ? "city_desc" : "City";
            ViewData["DistrictSortParam"] = sortOrder == "District" ? "district_desc" : "District";
            ViewData["StreetSortParam"] = sortOrder == "Street" ? "street_desc" : "Street";

            IQueryable<Apartment> apartmentsQuery = _context.Apartment
                .Include(a => a.ResidentialBuilding)
                .ThenInclude(rb => rb.Street)
                .ThenInclude(s => s.District)
                .ThenInclude(d => d.City);

            switch (sortOrder)
            {
                case "apartmentNumber_desc":
                    apartmentsQuery = apartmentsQuery.OrderByDescending(a => a.ApartmentNumber);
                    break;
                case "NumberOfRooms":
                    apartmentsQuery = apartmentsQuery.OrderBy(a => a.NumberOfRooms);
                    break;
                case "numberOfRooms_desc":
                    apartmentsQuery = apartmentsQuery.OrderByDescending(a => a.NumberOfRooms);
                    break;
                case "Area":
                    apartmentsQuery = apartmentsQuery.OrderBy(a => a.Area);
                    break;
                case "area_desc":
                    apartmentsQuery = apartmentsQuery.OrderByDescending(a => a.Area);
                    break;
                case "ResidentialBuilding":
                    apartmentsQuery = apartmentsQuery.OrderBy(a => a.ResidentialBuilding.HouseNumber);
                    break;
                case "residentialBuilding_desc":
                    apartmentsQuery = apartmentsQuery.OrderByDescending(a => a.ResidentialBuilding.HouseNumber);
                    break;
                case "City":
                    apartmentsQuery = apartmentsQuery.OrderBy(a => a.ResidentialBuilding.Street.District.City.NameCity);
                    break;
                case "city_desc":
                    apartmentsQuery = apartmentsQuery.OrderByDescending(a => a.ResidentialBuilding.Street.District.City.NameCity);
                    break;
                case "District":
                    apartmentsQuery = apartmentsQuery.OrderBy(a => a.ResidentialBuilding.Street.District.NameDistrict);
                    break;
                case "district_desc":
                    apartmentsQuery = apartmentsQuery.OrderByDescending(a => a.ResidentialBuilding.Street.District.NameDistrict);
                    break;
                case "Street":
                    apartmentsQuery = apartmentsQuery.OrderBy(a => a.ResidentialBuilding.Street.NameStreet);
                    break;
                case "street_desc":
                    apartmentsQuery = apartmentsQuery.OrderByDescending(a => a.ResidentialBuilding.Street.NameStreet);
                    break;
                default:
                    apartmentsQuery = apartmentsQuery.OrderBy(a => a.ApartmentNumber);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                // Используйте выбранный столбец для поиска
                switch (searchColumn)
                {
                    case "apartmentNumber":
                        apartmentsQuery = apartmentsQuery.Where(s => s.ApartmentNumber.ToString().ToLower().Contains(searchString.ToLower()));
                        break;
                    case "numberOfRooms":
                        apartmentsQuery = apartmentsQuery.Where(s => s.NumberOfRooms.ToString().Contains(searchString.ToLower()));
                        break;
                    case "area":
                        apartmentsQuery = apartmentsQuery.Where(s => s.Area.ToString().Contains(searchString.ToLower()));
                        break;
                    case "residentialBuilding.houseNumber":
                        apartmentsQuery = apartmentsQuery.Where(s => s.ResidentialBuilding.HouseNumber.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "residentialBuilding.Street.District.City.NameCity":
                        apartmentsQuery = apartmentsQuery.Where(s => s.ResidentialBuilding.Street.District.City.NameCity.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "residentialBuilding.Street.District.NameDistrict":
                        apartmentsQuery = apartmentsQuery.Where(s => s.ResidentialBuilding.Street.District.NameDistrict.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "residentialBuilding.Street.NameStreet":
                        apartmentsQuery = apartmentsQuery.Where(s => s.ResidentialBuilding.Street.NameStreet.ToLower().Contains(searchString.ToLower()));
                        break;
                }
            }

            return View(await apartmentsQuery.ToListAsync());
        }


        // GET: Apartments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment
                .Include(a => a.ResidentialBuilding)
                .FirstOrDefaultAsync(m => m.IdApartment == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        public IActionResult Create()
        {
            ViewData["ResidentialBuildingId"] = new SelectList(_context.ResidentialBuilding, "IdResidentialBuilding", "HouseNumber");
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity");
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict");
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdApartment,ApartmentNumber,NumberOfRooms,Area,ResidentialBuildingId")] Apartment apartment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(apartment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // При ошибке заполнения формы, обновляем также списки для города, района и улицы
            ViewData["ResidentialBuildingId"] = new SelectList(_context.ResidentialBuilding, "IdResidentialBuilding", "HouseNumber", apartment.ResidentialBuildingId);
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity");
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict");
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet");

            return View(apartment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment.FindAsync(id);
            if (apartment == null)
            {
                return NotFound();
            }

            // Загрузка данных для списков
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity");
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict");
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet");
            ViewData["ResidentialBuildingId"] = new SelectList(_context.ResidentialBuilding, "IdResidentialBuilding", "HouseNumber", apartment.ResidentialBuildingId);

            return View(apartment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdApartment,ApartmentNumber,NumberOfRooms,Area,ResidentialBuildingId")] Apartment apartment)
        {
            if (id != apartment.IdApartment)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apartment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApartmentExists(apartment.IdApartment))
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

            // Загрузка данных для списков при ошибке заполнения формы
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity");
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict");
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet");
            ViewData["ResidentialBuildingId"] = new SelectList(_context.ResidentialBuilding, "IdResidentialBuilding", "HouseNumber", apartment.ResidentialBuildingId);

            return View(apartment);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var apartment = await _context.Apartment
                .Include(a => a.ResidentialBuilding)
                .FirstOrDefaultAsync(m => m.IdApartment == id);
            if (apartment == null)
            {
                return NotFound();
            }

            return View(apartment);
        }

        // POST: Apartments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var apartment = await _context.Apartment.FindAsync(id);
            if (apartment != null)
            {
                _context.Apartment.Remove(apartment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApartmentExists(int id)
        {
            return _context.Apartment.Any(e => e.IdApartment == id);
        }
    }
}
