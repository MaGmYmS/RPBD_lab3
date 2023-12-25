using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AreasDataBase.Data;
using AreasDataBase.Models;
using Microsoft.AspNetCore.SignalR;

namespace AreasDataBase.Controllers
{
    public class ResidentialBuildingsController : Controller
    {
        private readonly AreasDataBaseContext _context;
        private readonly IHubContext<UpdateHub> _hubContext;

        public ResidentialBuildingsController(AreasDataBaseContext context, IHubContext<UpdateHub> hub)
        {
            _context = context;
            _hubContext = hub;
        }

        public async Task<IActionResult> Index(string searchString, string searchColumn, string sortOrder)
        {
            ViewData["HouseNumberSortParam"] = String.IsNullOrEmpty(sortOrder) ? "houseNumber_desc" : "";
            ViewData["YearOfConstructionSortParam"] = sortOrder == "YearOfConstruction" ? "yearOfConstruction_desc" : "YearOfConstruction";
            ViewData["NumbersOfFloorsSortParam"] = sortOrder == "NumbersOfFloors" ? "numbersOfFloors_desc" : "NumbersOfFloors";
            ViewData["StreetNameSortParam"] = sortOrder == "StreetName" ? "streetName_desc" : "StreetName";
            ViewData["DistrictNameSortParam"] = sortOrder == "DistrictName" ? "districtName_desc" : "DistrictName";
            ViewData["CityNameSortParam"] = sortOrder == "CityName" ? "cityName_desc" : "CityName";

            IQueryable<ResidentialBuilding> residentialBuildingsQuery = _context.ResidentialBuilding
                .Include(r => r.Street)
                    .ThenInclude(s => s.District)
                        .ThenInclude(d => d.City);

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
                case "DistrictName":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderBy(r => r.Street.District.NameDistrict);
                    break;
                case "districtName_desc":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderByDescending(r => r.Street.District.NameDistrict);
                    break;
                case "CityName":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderBy(r => r.Street.District.City.NameCity);
                    break;
                case "cityName_desc":
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderByDescending(r => r.Street.District.City.NameCity);
                    break;
                default:
                    residentialBuildingsQuery = residentialBuildingsQuery.OrderBy(r => r.HouseNumber);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchColumn)
                {
                    case "houseNumber":
                        residentialBuildingsQuery = residentialBuildingsQuery
                            .Where(r => r.HouseNumber != null && EF.Functions.Like(r.HouseNumber, $"%{searchString}%"));
                        break;

                    case "YearOfConstruction":
                        if (int.TryParse(searchString, out int year))
                        {
                            residentialBuildingsQuery = residentialBuildingsQuery
                                .Where(r => r.YearOfConstruction == year);
                        }
                        break;

                    case "NumbersOfFloors":
                        if (int.TryParse(searchString, out int floors))
                        {
                            residentialBuildingsQuery = residentialBuildingsQuery
                                .Where(r => r.NumbersOfFloors == floors);
                        }
                        break;

                    case "streetName":
                        residentialBuildingsQuery = residentialBuildingsQuery
                            .Where(s => s.Street != null ? s.Street.NameStreet.ToLower().Contains(searchString.ToLower()) : "неизвестная улица".ToLower().Contains(searchString.ToLower()));
                        break;

                    case "districtName":
                        residentialBuildingsQuery = residentialBuildingsQuery
                            .Where(s => s.Street != null && s.Street.District != null ?
                                        s.Street.District.NameDistrict.ToLower().Contains(searchString.ToLower()) :
                                        "неизвестный район".ToLower().Contains(searchString.ToLower()));
                        break;

                    case "cityName":
                        residentialBuildingsQuery = residentialBuildingsQuery
                            .Where(s => s.Street != null && s.Street.District != null && s.Street.District.City != null ?
                                        s.Street.District.City.NameCity.ToLower().Contains(searchString.ToLower()) :
                                        "неизвестный город".ToLower().Contains(searchString.ToLower()));
                        break;


                }
            }

            return View(await residentialBuildingsQuery.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity");
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict");
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdResidentialBuilding,HouseNumber,YearOfConstruction,NumbersOfFloors,StreetId")] ResidentialBuilding residentialBuilding)
        {
            if (ModelState.IsValid)
            {
                var street = _context.Street.FirstOrDefault(s => s.IdStreet == residentialBuilding.StreetId);
                if (street != null)
                {
                    residentialBuilding.Street = street;
                    var district = _context.District.FirstOrDefault(s => s.IdDistrict == residentialBuilding.Street.DistrictId);
                    residentialBuilding.Street.District = district;
                }

                // Проверяем уникальность дома и названия улицы
                if (_context.ResidentialBuilding.Any(rb => rb.HouseNumber == residentialBuilding.HouseNumber && rb.StreetId == residentialBuilding.StreetId))
                {
                    ModelState.AddModelError("HouseNumber", "Такой дом уже существует на выбранной улице.");
                }
                else
                {
                    _context.Add(residentialBuilding);
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("SendUpdateNotification", residentialBuilding.IdResidentialBuilding);
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity", residentialBuilding.Street.District.CityId);
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict", residentialBuilding.Street.DistrictId);
            ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet", residentialBuilding.StreetId);
            return View(residentialBuilding);
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var residentialBuilding = _context.ResidentialBuilding
                .Include(rb => rb.Street)
                    .ThenInclude(s => s.District)
                        .ThenInclude(d => d.City)
                .FirstOrDefault(rb => rb.IdResidentialBuilding == id);

            if (residentialBuilding == null)
            {
                return NotFound();
            }

            SetViewBagsForDropdowns(residentialBuilding);

            return View(residentialBuilding);
        }

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
                var street = _context.Street.FirstOrDefault(s => s.IdStreet == residentialBuilding.StreetId);
                if (street != null)
                {
                    residentialBuilding.Street = street;
                    var district = _context.District.FirstOrDefault(s => s.IdDistrict == residentialBuilding.Street.DistrictId);
                    residentialBuilding.Street.District = district;
                }

                if (_context.ResidentialBuilding.Any(rb => rb.HouseNumber == residentialBuilding.HouseNumber && rb.StreetId == residentialBuilding.StreetId && rb.IdResidentialBuilding != id))
                {
                    ModelState.AddModelError("HouseNumber", "Такой дом уже существует на выбранной улице.");
                }
                else
                {
                    try
                    {
                        _context.Update(residentialBuilding);
                        await _context.SaveChangesAsync();
                        await _hubContext.Clients.All.SendAsync("SendUpdateNotification", residentialBuilding.IdResidentialBuilding);

                        return RedirectToAction(nameof(Index));
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
                }
            }

            SetViewBagsForDropdowns(residentialBuilding);
            return View(residentialBuilding);
        }

        private void SetViewBagsForDropdowns(ResidentialBuilding residentialBuilding)
        {
            if (residentialBuilding.Street != null && residentialBuilding.Street.District != null && residentialBuilding.Street.District.CityId.HasValue)
            {
                ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict", residentialBuilding.Street.DistrictId);
                ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity", residentialBuilding.Street.District.CityId);
                ViewData["StreetId"] = new SelectList(_context.Street, "IdStreet", "NameStreet", residentialBuilding.StreetId);
            }
            else
            {
                var districts = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "", Text = "", Selected = true }
                    };
                districts.AddRange(_context.District.Select(d => new SelectListItem { Value = d.IdDistrict.ToString(), Text = d.NameDistrict }));
                ViewData["DistrictId"] = districts;

                var cities = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "", Text = "", Selected = true }
                    };
                cities.AddRange(_context.City.Select(c => new SelectListItem { Value = c.IdCity.ToString(), Text = c.NameCity }));
                ViewData["CityId"] = cities;

                var streets = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "", Text = "", Selected = true }
                    };
                streets.AddRange(_context.Street.Select(s => new SelectListItem { Value = s.IdStreet.ToString(), Text = s.NameStreet }));
                ViewData["StreetId"] = streets;
            }
        }




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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string deleteType)
        {
            bool cascadeDelete = deleteType == "cascade";

            var residentialBuilding = await _context.ResidentialBuilding.FindAsync(id);
            if (residentialBuilding != null)
            {
                if (!cascadeDelete)
                {
                    // Убираем связь здания с квартирами, если таковые есть
                    var relatedApartments = _context.Apartment.Where(a => a.ResidentialBuildingId == id);
                    foreach (var apartment in relatedApartments)
                    {
                        apartment.ResidentialBuildingId = null;
                    }
                }

                _context.ResidentialBuilding.Remove(residentialBuilding);
            }

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("SendUpdateNotification", residentialBuilding?.IdResidentialBuilding);
            return RedirectToAction(nameof(Index));
        }


        private bool ResidentialBuildingExists(int id)
        {
            return _context.ResidentialBuilding.Any(e => e.IdResidentialBuilding == id);
        }
    }
}
