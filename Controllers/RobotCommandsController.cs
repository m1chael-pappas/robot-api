using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using robot_api.Models;
using robot_api.Persistence;

namespace robot_api.Controllers;

/// <summary>
/// Controller for managing robot commands.
/// </summary>
[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController(IRobotCommandDataAccess robotCommandsRepo) : ControllerBase
{
    private readonly IRobotCommandDataAccess _robotCommandsRepo = robotCommandsRepo;

    /// <summary>
    /// Retrieves all robot commands.
    /// </summary>
    /// <returns>A list of all robot commands.</returns>
    /// <response code="200">Returns all robot commands.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet, Authorize(Policy = "UserOnly")]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return _robotCommandsRepo.GetRobotCommands();
    }

    /// <summary>
    /// Retrieves only move commands.
    /// </summary>
    /// <returns>A list of move commands.</returns>
    /// <response code="200">Returns all move commands.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpGet("move"), Authorize(Policy = "UserOnly")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly()
    {
        return _robotCommandsRepo.GetMoveCommands();
    }

    /// <summary>
    /// Retrieves a specific robot command by its ID.
    /// </summary>
    /// <param name="id">The ID of the robot command to retrieve.</param>
    /// <returns>The robot command with the specified ID.</returns>
    /// <response code="200">Returns the robot command with the specified ID.</response>
    /// <response code="404">If no robot command with the given ID exists.</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id}", Name = "GetRobotCommand"), Authorize(Policy = "UserOnly")]
    public IActionResult GetRobotCommandById(int id)
    {
        var command = _robotCommandsRepo.GetRobotCommandById(id);
        if (command == null)
            return NotFound();

        return Ok(command);
    }

    /// <summary>
    /// Creates a robot command.
    /// </summary>
    /// <param name="newCommand">A new robot command from the HTTP request.</param>
    /// <returns>A newly created robot command.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/robot-commands
    ///     {
    ///         "name": "DANCE",
    ///         "isMoveCommand": true,
    ///         "description": "Salsa on the Moon"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Returns the newly created robot command.</response>
    /// <response code="400">If the robot command is null.</response>
    /// <response code="409">If a robot command with the same name already exists.</response>
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost, Authorize(Policy = "AdminOnly")]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
            return BadRequest();

        var command = _robotCommandsRepo.InsertRobotCommand(newCommand);

        return CreatedAtRoute("GetRobotCommand", new { id = command.Id }, command);
    }

    /// <summary>
    /// Updates an existing robot command.
    /// </summary>
    /// <param name="id">The ID of the robot command to update.</param>
    /// <param name="updatedCommand">The updated robot command data from the HTTP request.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the robot command was successfully updated.</response>
    /// <response code="400">If the updated robot command data is null.</response>
    /// <response code="404">If no robot command with the given ID exists.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpPut("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var command = _robotCommandsRepo.GetRobotCommandById(id);
        if (command == null)
            return NotFound();

        if (updatedCommand == null)
            return BadRequest();

        _robotCommandsRepo.UpdateRobotCommand(id, updatedCommand);

        return NoContent();
    }

    /// <summary>
    /// Deletes a robot command by its ID.
    /// </summary>
    /// <param name="id">The ID of the robot command to delete.</param>
    /// <returns>No content.</returns>
    /// <response code="204">If the robot command was successfully deleted.</response>
    /// <response code="404">If no robot command with the given ID exists.</response>
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{id}"), Authorize(Policy = "AdminOnly")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var deleted = _robotCommandsRepo.DeleteRobotCommand(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
