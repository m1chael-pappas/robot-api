using Microsoft.AspNetCore.Mvc;
using robot_api.Models;

namespace robot_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    private static readonly List<Map> _maps =
    [
        new(1, 5, 5, "DEAKIN", DateTime.Now, DateTime.Now, "Default 5x5 square map"),
        new(2, 10, 10, "MOON", DateTime.Now, DateTime.Now, "Large 10x10 square map"),
        new(3, 5, 10, "BURWOOD", DateTime.Now, DateTime.Now, "Rectangular 5x10 map"),
    ];

    [HttpGet()]
    public IEnumerable<Map> GetAllMaps() => _maps;

    [HttpGet("square")]
    public IEnumerable<Map> GetSquareMapsOnly() => _maps.Where(m => m.Columns == m.Rows);

    [HttpGet("{id}", Name = "GetMap")]
    public IActionResult GetMapById(int id)
    {
        var map = _maps.FirstOrDefault(m => m.Id == id);
        if (map == null)
            return NotFound();

        return Ok(map);
    }

    [HttpPost()]
    public IActionResult AddMap(Map newMap)
    {
        if (newMap == null)
            return BadRequest();

        if (_maps.Any(m => m.Name == newMap.Name))
            return Conflict();

        var id = _maps.Max(m => m.Id) + 1;
        var map = new Map(
            id,
            newMap.Columns,
            newMap.Rows,
            newMap.Name,
            DateTime.Now,
            DateTime.Now,
            newMap.Description
        );

        _maps.Add(map);

        return CreatedAtRoute("GetMap", new { id = map.Id }, map);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        var map = _maps.FirstOrDefault(m => m.Id == id);
        if (map == null)
            return NotFound();

        if (updatedMap == null)
            return BadRequest();

        map.Columns = updatedMap.Columns;
        map.Rows = updatedMap.Rows;
        map.Name = updatedMap.Name;
        map.Description = updatedMap.Description;
        map.ModifiedDate = DateTime.Now;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMap(int id)
    {
        var map = _maps.FirstOrDefault(m => m.Id == id);
        if (map == null)
            return NotFound();

        _maps.Remove(map);

        return NoContent();
    }

    [HttpGet("{id}/{x}-{y}")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        if (x < 0 || y < 0)
            return BadRequest();

        var map = _maps.FirstOrDefault(m => m.Id == id);
        if (map == null)
            return NotFound();

        bool isOnMap = x < map.Columns && y < map.Rows;

        return Ok(isOnMap);
    }
}
