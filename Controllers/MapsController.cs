using Microsoft.AspNetCore.Mvc;
using robot_api.Models;
using robot_api.Persistence;

namespace robot_api.Controllers;

[ApiController]
[Route("api/maps")]
public class MapsController : ControllerBase
{
    [HttpGet()]
    public IEnumerable<Map> GetAllMaps()
    {
        return MapDataAccess.GetMaps();
    }

    [HttpGet("square")]
    public IEnumerable<Map> GetSquareMapsOnly()
    {
        return MapDataAccess.GetSquareMaps();
    }

    [HttpGet("{id}", Name = "GetMap")]
    public IActionResult GetMapById(int id)
    {
        var map = MapDataAccess.GetMapById(id);
        if (map == null)
            return NotFound();

        return Ok(map);
    }

    [HttpPost()]
    public IActionResult AddMap(Map newMap)
    {
        if (newMap == null)
            return BadRequest();

        var map = MapDataAccess.InsertMap(newMap);

        return CreatedAtRoute("GetMap", new { id = map.Id }, map);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        var map = MapDataAccess.GetMapById(id);
        if (map == null)
            return NotFound();

        if (updatedMap == null)
            return BadRequest();

        MapDataAccess.UpdateMap(id, updatedMap);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteMap(int id)
    {
        var deleted = MapDataAccess.DeleteMap(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    [HttpGet("{id}/{x}-{y}")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        if (x < 0 || y < 0)
            return BadRequest();

        var map = MapDataAccess.GetMapById(id);
        if (map == null)
            return NotFound();

        bool isOnMap = x < map.Columns && y < map.Rows;

        return Ok(isOnMap);
    }
}
