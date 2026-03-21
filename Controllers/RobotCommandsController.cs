using Microsoft.AspNetCore.Mvc;
using robot_api.Models;
using robot_api.Persistence;

namespace robot_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    [HttpGet()]
    public IEnumerable<RobotCommand> GetAllRobotCommands()
    {
        return RobotCommandDataAccess.GetRobotCommands();
    }

    [HttpGet("move")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly()
    {
        return RobotCommandDataAccess.GetMoveCommands();
    }

    [HttpGet("{id}", Name = "GetRobotCommand")]
    public IActionResult GetRobotCommandById(int id)
    {
        var command = RobotCommandDataAccess.GetRobotCommandById(id);
        if (command == null)
            return NotFound();

        return Ok(command);
    }

    [HttpPost()]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
            return BadRequest();

        var command = RobotCommandDataAccess.InsertRobotCommand(newCommand);

        return CreatedAtRoute("GetRobotCommand", new { id = command.Id }, command);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var command = RobotCommandDataAccess.GetRobotCommandById(id);
        if (command == null)
            return NotFound();

        if (updatedCommand == null)
            return BadRequest();

        RobotCommandDataAccess.UpdateRobotCommand(id, updatedCommand);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var deleted = RobotCommandDataAccess.DeleteRobotCommand(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
