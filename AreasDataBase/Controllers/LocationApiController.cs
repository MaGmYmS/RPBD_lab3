using AreasDataBase.Data;
using Microsoft.AspNetCore.Mvc;

[Route("api/location")]
public class LocationApiController : Controller
{
    private readonly AreasDataBaseContext _context;

    public LocationApiController(AreasDataBaseContext context)
    {
        _context = context;
    }

    [HttpGet("cities")]
    public IActionResult GetCities()
    {
        var cities = _context.City.ToList();
        return Ok(cities);
    }

    [HttpGet("districts/{cityId}")]
    public IActionResult GetDistricts(int cityId)
    {
        var districts = _context.District.Where(d => d.CityId == cityId).ToList();
        return Ok(districts);
    }

    [HttpGet("streets/{districtId}")]
    public IActionResult GetStreets(int districtId)
    {
        var streets = _context.Street.Where(s => s.DistrictId == districtId).ToList();
        return Ok(streets);
    }
}
