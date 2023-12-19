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
    public class StreetsController : Controller
    {
        private readonly AreasDataBaseContext _context;
        private readonly IHubContext<UpdateHub> _hubContext;

        public StreetsController(AreasDataBaseContext context, IHubContext<UpdateHub> hub)
        {
            _context = context;
            _hubContext = hub;
        }

        public async Task<IActionResult> Index(string searchString, string searchColumn, string sortOrder)
        {
            ViewData["NameStreetSortParam"] = String.IsNullOrEmpty(sortOrder) ? "nameStreet_desc" : "";
            ViewData["DistrictNameSortParam"] = sortOrder == "DistrictName" ? "districtName_desc" : "DistrictName";
            ViewData["CityNameSortParam"] = sortOrder == "CityName" ? "cityName_desc" : "CityName"; 

            IQueryable<Street> streetsQuery = _context.Street
                .Include(s => s.District)
                    .ThenInclude(d => d.City); 

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
                case "CityName": 
                    streetsQuery = streetsQuery.OrderBy(s => s.District.City.NameCity);
                    break;
                case "cityName_desc": 
                    streetsQuery = streetsQuery.OrderByDescending(s => s.District.City.NameCity);
                    break;
                default:
                    streetsQuery = streetsQuery.OrderBy(s => s.NameStreet);
                    break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                switch (searchColumn)
                {
                    case "nameStreet":
                        streetsQuery = streetsQuery.Where(s => s.NameStreet.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "districtName":
                        streetsQuery = streetsQuery.Where(s => s.District.NameDistrict.ToLower().Contains(searchString.ToLower()));
                        break;
                    case "сityName": 
                        streetsQuery = streetsQuery.Where(s => s.District.City.NameCity.ToLower().Contains(searchString.ToLower()));
                        break;
                }
            }

            return View(await streetsQuery.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict");
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity");
            return View();
        }


        [HttpGet]
        public JsonResult GetDistrictsByCity(int cityId)
        {
            var districts = _context.District
                .Where(d => d.CityId == cityId)
                .Select(d => new { IdDistrict = d.IdDistrict, NameDistrict = d.NameDistrict })
                .ToList();

            return Json(districts);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdStreet,NameStreet,DistrictId")] Street street)
        {
            if (ModelState.IsValid)
            {
                var district = _context.District.FirstOrDefault(d => d.IdDistrict == street.DistrictId);
                if (district != null)
                {
                    street.District = district;
                }

                // Проверяем уникальность названия улицы внутри выбранного города
                if (_context.Street.Any(s => s.NameStreet == street.NameStreet && s.District.CityId == street.District.CityId))
                {
                    ModelState.AddModelError("NameStreet", "Такая улица уже существует в выбранном городе.");
                }
                else
                {
                    _context.Add(street);
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("SendUpdateNotification", street.IdStreet);
                    return RedirectToAction(nameof(Index));
                }
            }

            // Переинициализируйте SelectList здесь, чтобы он был доступен при возврате представления с ошибкой
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict", street.DistrictId);
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity", street.District.CityId);
            return View(street);
        }



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
            ViewBag.CityId = new SelectList(_context.City, "IdCity", "NameCity");

            return View(street);
        }

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
                var district = _context.District.FirstOrDefault(d => d.IdDistrict == street.DistrictId);
                if (district != null)
                {
                    street.District = district;
                }
                // Проверяем уникальность названия улицы внутри выбранного города
                if (_context.Street.Any(s => s.IdStreet != street.IdStreet && s.NameStreet == street.NameStreet && s.District.CityId == street.District.CityId))
                {
                    ModelState.AddModelError("NameStreet", "Такая улица уже существует в выбранном городе.");
                }
                else
                {
                    try
                    {
                        _context.Update(street);
                        await _context.SaveChangesAsync();
                        await _hubContext.Clients.All.SendAsync("SendUpdateNotification", street.IdStreet);
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
            }
            ViewData["DistrictId"] = new SelectList(_context.District, "IdDistrict", "NameDistrict", street.DistrictId);
            ViewData["CityId"] = new SelectList(_context.City, "IdCity", "NameCity", street.District.CityId);
            return View(street);
        }


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
            await _hubContext.Clients.All.SendAsync("SendUpdateNotification", street.IdStreet);
            return RedirectToAction(nameof(Index));
        }

        private bool StreetExists(int id)
        {
            return _context.Street.Any(e => e.IdStreet == id);
        }
    }
}
