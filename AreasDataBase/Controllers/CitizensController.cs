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
using OfficeOpenXml;
using Xceed.Words.NET;
using Xceed.Document.NET;

namespace AreasDataBase.Controllers
{
    public class CitizensController : Controller
    {
        private readonly AreasDataBaseContext _context;
        private readonly IHubContext<UpdateHub> _hubContext;
        private static List<Citizen> _sortedFiltredCitizens;

        public CitizensController(AreasDataBaseContext context, IHubContext<UpdateHub> hub)
        {
            _context = context;
            _hubContext = hub;
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
                            List<Citizen> filteredCitizens = new List<Citizen>();

                            foreach (var citizen in citizens)
                            {
                                DateTime sDate = DateTime.SpecifyKind(citizen.DateOfBirth, DateTimeKind.Utc).Date;

                                if (sDate == searchDate.Date)
                                {
                                    filteredCitizens.Add(citizen);
                                }
                            }

                            var result_tmp = filteredCitizens;
                            _sortedFiltredCitizens = result_tmp;

                            return View(result_tmp);

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

                    //case "apartmentNumber":
                    //    if (int.TryParse(searchString, out int apartmentNum))
                    //    {
                    //        citizens = citizens.Where(s => s.Apartment != null ? s.Apartment.ApartmentNumber == apartmentNum : "неизвестный номер квартиры".Contains(searchString));
                    //    }
                    //    break;
                    case "apartmentNumber":
                        if (int.TryParse(searchString, out int apartmentNum))
                        {
                            citizens = citizens
                                .Where(s => s.Apartment != null ?
                                            s.Apartment.ApartmentNumber == apartmentNum :
                                            "неизвестный номер квартиры".Contains(searchString.ToLower()));
                        }
                        break;

                    case "city":
                        citizens = citizens
                            .Where(s => s.Apartment != null && s.Apartment.ResidentialBuilding != null && s.Apartment.ResidentialBuilding.Street != null &&
                                        s.Apartment.ResidentialBuilding.Street.District != null && s.Apartment.ResidentialBuilding.Street.District.City != null ?
                                         s.Apartment.ResidentialBuilding.Street.District.City.NameCity.ToLower().Contains(searchString.ToLower()) :
                                         "неизвестный город".Contains(searchString.ToLower()));
                        break;

                    case "district":
                        citizens = citizens
                            .Where(s => s.Apartment != null && s.Apartment.ResidentialBuilding != null && s.Apartment.ResidentialBuilding.Street != null &&
                                        (s.Apartment.ResidentialBuilding.Street.District != null ?
                                         s.Apartment.ResidentialBuilding.Street.District.NameDistrict.ToLower().Contains(searchString.ToLower()) :
                                         "неизвестный район".Contains(searchString.ToLower())));
                        break;

                    case "street":
                        citizens = citizens
                            .Where(s => s.Apartment != null && s.Apartment.ResidentialBuilding != null ?
                                        (s.Apartment.ResidentialBuilding.Street != null ?
                                         s.Apartment.ResidentialBuilding.Street.NameStreet.ToLower().Contains(searchString.ToLower()) :
                                         "неизвестная улица".Contains(searchString.ToLower())) :
                                        false);
                        break;

                    case "houseNumber":
                        if (!string.IsNullOrEmpty(searchString))
                        {
                            List<Citizen> filteredCitizens = new List<Citizen>();

                            foreach (var citizen in citizens)
                            {
                                if (citizen.Apartment != null && citizen.Apartment.ResidentialBuilding != null)
                                {
                                    string houseNumber = citizen.Apartment.ResidentialBuilding.HouseNumber;

                                    if (houseNumber != null && houseNumber.ToLower().Contains(searchString.ToLower()))
                                    {
                                        filteredCitizens.Add(citizen);
                                    }
                                    else if (houseNumber == null && "неизвестный номер дома".ToLower().Contains(searchString.ToLower()))
                                    {
                                        filteredCitizens.Add(citizen);
                                    }
                                }
                                else 
                                {
                                    filteredCitizens.Add(citizen);
                                }
                            }

                            citizens = filteredCitizens.AsQueryable();
                        }

                        break;



                }
            }
            _sortedFiltredCitizens = citizens.ToList();

            return View(citizens);
        }

        [HttpGet]
        public JsonResult CheckCitizensCount()
        {
            bool hasCitizens = _sortedFiltredCitizens != null && _sortedFiltredCitizens.Count > 0;
            return Json(new { hasCitizens });
        }


        [HttpGet]
        public IActionResult ExportCitizensToExcel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Создаем новый пакет Excel
            using (var package = new ExcelPackage())
            {
                // Добавляем новый лист
                var worksheet = package.Workbook.Worksheets.Add("Citizens");

                // Добавляем заголовки
                worksheet.Cells[1, 1].Value = "Полное имя";
                worksheet.Cells[1, 2].Value = "Паспортные данные";
                worksheet.Cells[1, 3].Value = "Номер телефона";
                worksheet.Cells[1, 4].Value = "Дата рождения";
                worksheet.Cells[1, 5].Value = "Пол";
                worksheet.Cells[1, 6].Value = "Номер квартиры";
                worksheet.Cells[1, 7].Value = "Город";
                worksheet.Cells[1, 8].Value = "Район";
                worksheet.Cells[1, 9].Value = "Улица";
                worksheet.Cells[1, 10].Value = "Номер дома";

                var sortedFiltredCitizens = _sortedFiltredCitizens;
                // Заполняем ячейки данными
                for (int i = 0; i < sortedFiltredCitizens.Count; i++)
                {
                    var citizen = sortedFiltredCitizens[i];

                    worksheet.Cells[i + 2, 1].Value = citizen.FullName;
                    worksheet.Cells[i + 2, 2].Value = citizen.PassportData;
                    worksheet.Cells[i + 2, 3].Value = citizen.PhoneNumber;
                    worksheet.Cells[i + 2, 4].Value = citizen.DateOfBirth.ToString("dd.MM.yyyy");
                    worksheet.Cells[i + 2, 5].Value = citizen.Gender ? "Мужской" : "Женский";
                    worksheet.Cells[i + 2, 6].Value = citizen.Apartment?.ApartmentNumber.ToString() ?? "Неизвестная квартира";
                    worksheet.Cells[i + 2, 7].Value = citizen.Apartment?.ResidentialBuilding?.Street?.District?.City?.NameCity ?? "Неизвестный город";
                    worksheet.Cells[i + 2, 8].Value = citizen.Apartment?.ResidentialBuilding?.Street?.District?.NameDistrict ?? "Неизвестный район";
                    worksheet.Cells[i + 2, 9].Value = citizen.Apartment?.ResidentialBuilding?.Street?.NameStreet ?? "Неизвестная улица";
                    worksheet.Cells[i + 2, 10].Value = citizen.Apartment?.ResidentialBuilding?.HouseNumber ?? "Неизвестный дом";
                }

                // Устанавливаем автоширину для всех столбцов
                for (int col = 1; col <= 10; col++)
                {
                    worksheet.Column(col).AutoFit();
                }

                // Сохраняем файл на сервере
                var memoryStream = new MemoryStream();
                package.SaveAs(memoryStream);
                memoryStream.Position = 0;

                // Возвращаем файл пользователю
                return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Citizens.xlsx");
            }
        }

        [HttpGet]
        public IActionResult ExportCitizensToWord()
        {
            // Создаем новый документ Word
            using (var doc = DocX.Create("Citizens.docx"))
            {
                // Добавляем заголовок
                doc.InsertParagraph("Список граждан").FontSize(20d).Bold().Alignment = Alignment.center;

                // Добавляем таблицу
                var table = doc.InsertTable(_sortedFiltredCitizens.Count + 1, 10);
                table.Design = TableDesign.TableGrid;

                // Добавляем заголовки таблицы
                table.Rows[0].Cells[0].Paragraphs.First().Append("Полное имя");
                table.Rows[0].Cells[1].Paragraphs.First().Append("Паспортные данные");
                table.Rows[0].Cells[2].Paragraphs.First().Append("Номер телефона");
                table.Rows[0].Cells[3].Paragraphs.First().Append("Дата рождения");
                table.Rows[0].Cells[4].Paragraphs.First().Append("Пол");
                table.Rows[0].Cells[5].Paragraphs.First().Append("Номер квартиры");
                table.Rows[0].Cells[6].Paragraphs.First().Append("Город");
                table.Rows[0].Cells[7].Paragraphs.First().Append("Район");
                table.Rows[0].Cells[8].Paragraphs.First().Append("Улица");
                table.Rows[0].Cells[9].Paragraphs.First().Append("Номер дома");

                for (int i = 0; i < _sortedFiltredCitizens.Count; i++)
                {
                    var citizen = _sortedFiltredCitizens[i];

                    table.Rows[i + 1].Cells[0].Paragraphs.First().Append(citizen.FullName);
                    table.Rows[i + 1].Cells[1].Paragraphs.First().Append(citizen.PassportData.ToString());
                    table.Rows[i + 1].Cells[2].Paragraphs.First().Append(citizen.PhoneNumber.ToString());
                    table.Rows[i + 1].Cells[3].Paragraphs.First().Append(citizen.DateOfBirth.ToString("dd.MM.yyyy"));
                    table.Rows[i + 1].Cells[4].Paragraphs.First().Append(citizen.Gender ? "Мужской" : "Женский");
                    table.Rows[i + 1].Cells[5].Paragraphs.First().Append(citizen.Apartment?.ApartmentNumber.ToString() ?? "Неизвестная квартира");
                    table.Rows[i + 1].Cells[6].Paragraphs.First().Append(citizen.Apartment?.ResidentialBuilding?.Street?.District?.City?.NameCity ?? "Неизвестный город");
                    table.Rows[i + 1].Cells[7].Paragraphs.First().Append(citizen.Apartment?.ResidentialBuilding?.Street?.District?.NameDistrict ?? "Неизвестный район");
                    table.Rows[i + 1].Cells[8].Paragraphs.First().Append(citizen.Apartment?.ResidentialBuilding?.Street?.NameStreet ?? "Неизвестная улица");
                    table.Rows[i + 1].Cells[9].Paragraphs.First().Append(citizen.Apartment?.ResidentialBuilding?.HouseNumber?.ToString() ?? "Неизвестный дом");
                }


                var memoryStream = new MemoryStream();
                doc.SaveAs(memoryStream);
                memoryStream.Position = 0;

               return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "Citizens.docx");
            }
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
                citizen.DateOfBirth = citizen.DateOfBirth.ToUniversalTime();
                _context.Add(citizen);
                await _context.SaveChangesAsync();
                await _hubContext.Clients.All.SendAsync("SendUpdateNotification", citizen.IdCitizen);
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
                    citizen.DateOfBirth = citizen.DateOfBirth.ToUniversalTime();
                    _context.Update(citizen);
                    await _context.SaveChangesAsync();
                    await _hubContext.Clients.All.SendAsync("SendUpdateNotification", citizen.IdCitizen);
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
        public async Task<IActionResult> DeleteConfirmed(int id, string deleteType)
        {
            var citizen = await _context.Citizen.FindAsync(id);
            if (citizen != null)
            { 
                _context.Citizen.Remove(citizen);
            }

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("SendUpdateNotification", citizen?.IdCitizen);
            return RedirectToAction(nameof(Index));
        }



        private bool CitizenExists(int id)
        {
            return _context.Citizen.Any(e => e.IdCitizen == id);
        }
    }
}
