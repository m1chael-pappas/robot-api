using Microsoft.EntityFrameworkCore;
using robot_api.Models;

namespace robot_api.Persistence;

public class RobotCommandEF : IRobotCommandDataAccess
{
    private readonly RobotContext _context;

    public RobotCommandEF(RobotContext context)
    {
        _context = context;
    }

    public List<RobotCommand> GetRobotCommands()
    {
        return _context.RobotCommands.ToList();
    }

    public List<RobotCommand> GetMoveCommands()
    {
        return _context.RobotCommands.Where(x => x.IsMoveCommand).ToList();
    }

    public RobotCommand? GetRobotCommandById(int id)
    {
        return _context.RobotCommands.Find(id);
    }

    public RobotCommand InsertRobotCommand(RobotCommand robotCommand)
    {
        robotCommand.CreatedDate = DateTime.Now;
        robotCommand.ModifiedDate = DateTime.Now;
        _context.RobotCommands.Add(robotCommand);
        _context.SaveChanges();
        return robotCommand;
    }

    public void UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var existing = _context.RobotCommands.Find(id);
        if (existing != null)
        {
            existing.Name = updatedCommand.Name;
            existing.Description = updatedCommand.Description;
            existing.IsMoveCommand = updatedCommand.IsMoveCommand;
            existing.ModifiedDate = DateTime.Now;
            _context.SaveChanges();
        }
    }

    public bool DeleteRobotCommand(int id)
    {
        var command = _context.RobotCommands.Find(id);
        if (command == null)
            return false;
        _context.RobotCommands.Remove(command);
        _context.SaveChanges();
        return true;
    }
}
