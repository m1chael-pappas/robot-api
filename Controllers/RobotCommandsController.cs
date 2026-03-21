using Microsoft.AspNetCore.Mvc;
using robot_api.Models;

namespace robot_api.Controllers;

[ApiController]
[Route("api/robot-commands")]
public class RobotCommandsController : ControllerBase
{
    private static readonly List<RobotCommand> _commands =
    [
        new(1, "LEFT", true, DateTime.Now, DateTime.Now, "Turn the robot 90 degrees left"),
        new(2, "RIGHT", true, DateTime.Now, DateTime.Now, "Turn the robot 90 degrees right"),
        new(3, "MOVE", true, DateTime.Now, DateTime.Now, "Move the robot one step forward"),
        new(
            4,
            "PLACE",
            false,
            DateTime.Now,
            DateTime.Now,
            "Place the robot at X,Y facing direction D"
        ),
        new(
            5,
            "REPORT",
            false,
            DateTime.Now,
            DateTime.Now,
            "Report the current position of the robot"
        ),
    ];

    [HttpGet()]
    public IEnumerable<RobotCommand> GetAllRobotCommands() => _commands;

    [HttpGet("move")]
    public IEnumerable<RobotCommand> GetMoveCommandsOnly() => _commands.Where(c => c.IsMoveCommand);

    [HttpGet("{id}", Name = "GetRobotCommand")]
    public IActionResult GetRobotCommandById(int id)
    {
        var command = _commands.FirstOrDefault(c => c.Id == id);
        if (command == null)
            return NotFound();

        return Ok(command);
    }

    [HttpPost()]
    public IActionResult AddRobotCommand(RobotCommand newCommand)
    {
        if (newCommand == null)
            return BadRequest();

        if (_commands.Any(c => c.Name == newCommand.Name))
            return Conflict();

        var id = _commands.Max(c => c.Id) + 1;
        var command = new RobotCommand(
            id,
            newCommand.Name,
            newCommand.IsMoveCommand,
            DateTime.Now,
            DateTime.Now,
            newCommand.Description
        );

        _commands.Add(command);

        return CreatedAtRoute("GetRobotCommand", new { id = command.Id }, command);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var command = _commands.FirstOrDefault(c => c.Id == id);
        if (command == null)
            return NotFound();

        if (updatedCommand == null)
            return BadRequest();

        command.Name = updatedCommand.Name;
        command.Description = updatedCommand.Description;
        command.IsMoveCommand = updatedCommand.IsMoveCommand;
        command.ModifiedDate = DateTime.Now;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteRobotCommand(int id)
    {
        var command = _commands.FirstOrDefault(c => c.Id == id);
        if (command == null)
            return NotFound();

        _commands.Remove(command);

        return NoContent();
    }
}
