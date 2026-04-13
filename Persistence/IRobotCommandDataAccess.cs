using robot_api.Models;

namespace robot_api.Persistence;

public interface IRobotCommandDataAccess
{
    List<RobotCommand> GetRobotCommands();
    List<RobotCommand> GetMoveCommands();
    RobotCommand? GetRobotCommandById(int id);
    RobotCommand InsertRobotCommand(RobotCommand robotCommand);
    void UpdateRobotCommand(int id, RobotCommand robotCommand);
    bool DeleteRobotCommand(int id);
}
