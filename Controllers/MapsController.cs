using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using robot_api.Models;
using robot_api.Persistence;

namespace robot_api.Controllers;

/// <summary>
/// Controller for managing maps in the robot simulator.
/// </summary>
[ApiController]
[Route("api/maps")]
public class MapsController(IMapDataAccess mapsRepo) : ControllerBase
{
    private readonly IMapDataAccess _mapsRepo = mapsRepo;

    /// <summary>
    /// Retrieves all maps.
    /// </summary>
    /// <returns>A list of all maps.</returns>
    /// <response code="200">Returns all maps.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet, Authorize(Policy = "UserOnly")]
    public IEnumerable<Map> GetAllMaps()
    {
        return _mapsRepo.GetMaps();
    }

    /// <summary>
    /// Retrieves only square maps (columns == rows).
    /// </summary>
    /// <returns>A list of square maps.</returns>
    /// <response code="200">Returns all square maps.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("square"), Authorize(Policy = "UserOnly")]
    public IEnumerable<Map> GetSquareMapsOnly()
    {
        return _mapsRepo.GetSquareMaps();
    }

    /// <summary>
    /// Retrieves a specific map by its ID.
    /// </summary>
    /// <param name="id">The ID of the map to retrieve.</param>
    /// <returns>The map with the specified ID.</returns>
    /// <response code="200">Returns the map with the specified ID.</response>
    /// <response code="404">If no map with the given ID exists.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}", Name = "GetMap"), Authorize(Policy = "UserOnly")]
    public IActionResult GetMapById(int id)
    {
        var map = _mapsRepo.GetMapById(id);
        if (map == null)
            return NotFound();

        return Ok(map);
    }

    /// <summary>
    /// Creates a new map.
    /// </summary>
    /// <param name="newMap">A new map from the HTTP request.</param>
    /// <returns>A newly created map.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/maps
    ///     {
    ///         "columns": 10,
    ///         "rows": 10,
    ///         "name": "Moon Base Alpha",
    ///         "description": "A 10x10 square map"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created map.</response>
    /// <response code="400">If the map is null.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost, Authorize(Policy = "AdminOnly")]
    public IActionResult AddMap(Map newMap)
    {
        if (newMap == null)
            return BadRequest();

        var map = _mapsRepo.InsertMap(newMap);

        return CreatedAtRoute("GetMap", new { id = map.Id }, map);
    }

    /// <summary>
    /// Updates an existing map.
    /// </summary>
    /// <param name="id">The ID of the map to update.</param>
    /// <param name="updatedMap">The updated map data from the HTTP request.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the map was successfully updated.</response>
    /// <response code="400">If the updated map data is null.</response>
    /// <response code="404">If no map with the given ID exists.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateMap(int id, Map updatedMap)
    {
        var map = _mapsRepo.GetMapById(id);
        if (map == null)
            return NotFound();

        if (updatedMap == null)
            return BadRequest();

        _mapsRepo.UpdateMap(id, updatedMap);

        return NoContent();
    }

    /// <summary>
    /// Deletes a map by its ID.
    /// </summary>
    /// <param name="id">The ID of the map to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the map was successfully deleted.</response>
    /// <response code="404">If no map with the given ID exists.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteMap(int id)
    {
        var deleted = _mapsRepo.DeleteMap(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Checks whether a coordinate is on a given map.
    /// </summary>
    /// <param name="id">The ID of the map.</param>
    /// <param name="x">The x-coordinate (column) to check.</param>
    /// <param name="y">The y-coordinate (row) to check.</param>
    /// <returns>True if the coordinate is on the map, false otherwise.</returns>
    /// <response code="200">Returns true or false.</response>
    /// <response code="400">If x or y is negative.</response>
    /// <response code="404">If no map with the given ID exists.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}/{x}-{y}"), Authorize(Policy = "UserOnly")]
    public IActionResult CheckCoordinate(int id, int x, int y)
    {
        if (x < 0 || y < 0)
            return BadRequest();

        var map = _mapsRepo.GetMapById(id);
        if (map == null)
            return NotFound();

        bool isOnMap = x < map.Columns && y < map.Rows;

        return Ok(isOnMap);
    }
}
