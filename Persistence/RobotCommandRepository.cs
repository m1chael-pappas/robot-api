using Npgsql;
using robot_api.Models;

namespace robot_api.Persistence;

public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
{
    private IRepository _repo => this;

    public List<RobotCommand> GetRobotCommands()
    {
        return _repo.ExecuteReader<RobotCommand>("SELECT * FROM public.robot_command");
    }

    public List<RobotCommand> GetMoveCommands()
    {
        return _repo.ExecuteReader<RobotCommand>(
            "SELECT * FROM public.robot_command WHERE is_move_command = true"
        );
    }

    public RobotCommand? GetRobotCommandById(int id)
    {
        var sqlParams = new NpgsqlParameter[] { new("id", id) };
        var result = _repo.ExecuteReader<RobotCommand>(
            "SELECT * FROM public.robot_command WHERE id = @id",
            sqlParams
        );
        return result.FirstOrDefault();
    }

    public RobotCommand InsertRobotCommand(RobotCommand robotCommand)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("name", robotCommand.Name),
            new("description", robotCommand.Description ?? (object)DBNull.Value),
            new("is_move_command", robotCommand.IsMoveCommand),
        };
        var result = _repo
            .ExecuteReader<RobotCommand>(
                @"INSERT INTO robot_command (name, description, is_move_command, created_date, modified_date)
              VALUES (@name, @description, @is_move_command, current_timestamp, current_timestamp)
              RETURNING *;",
                sqlParams
            )
            .Single();
        return result;
    }

    public void UpdateRobotCommand(int id, RobotCommand updatedCommand)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id),
            new("name", updatedCommand.Name),
            new("description", updatedCommand.Description ?? (object)DBNull.Value),
            new("is_move_command", updatedCommand.IsMoveCommand),
        };
        _repo.ExecuteReader<RobotCommand>(
            @"UPDATE robot_command SET name=@name, description=@description,
              is_move_command=@is_move_command, modified_date=current_timestamp
              WHERE id=@id RETURNING *;",
            sqlParams
        );
    }

    public bool DeleteRobotCommand(int id)
    {
        var sqlParams = new NpgsqlParameter[] { new("id", id) };
        var result = _repo.ExecuteReader<RobotCommand>(
            "DELETE FROM robot_command WHERE id=@id RETURNING *;",
            sqlParams
        );
        return result.Count > 0;
    }
}
