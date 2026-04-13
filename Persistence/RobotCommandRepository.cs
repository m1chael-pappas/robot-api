using Npgsql;
using robot_api.Models;

namespace robot_api.Persistence;

public class RobotCommandRepository : IRobotCommandDataAccess, IRepository
{
    private IRepository _repo => this;

    public List<RobotCommand> GetRobotCommands()
    {
        return _repo.ExecuteReader<RobotCommand>("SELECT * FROM public.robotcommand");
    }

    public List<RobotCommand> GetMoveCommands()
    {
        return _repo.ExecuteReader<RobotCommand>(
            "SELECT * FROM public.robotcommand WHERE ismovecommand = true"
        );
    }

    public RobotCommand? GetRobotCommandById(int id)
    {
        var sqlParams = new NpgsqlParameter[] { new("id", id) };
        var result = _repo.ExecuteReader<RobotCommand>(
            "SELECT * FROM public.robotcommand WHERE id = @id",
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
            new("ismovecommand", robotCommand.IsMoveCommand),
        };
        var result = _repo
            .ExecuteReader<RobotCommand>(
                @"INSERT INTO robotcommand (""Name"", description, ismovecommand, createddate, modifieddate)
              VALUES (@name, @description, @ismovecommand, current_timestamp, current_timestamp)
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
            new("ismovecommand", updatedCommand.IsMoveCommand),
        };
        _repo.ExecuteReader<RobotCommand>(
            @"UPDATE robotcommand SET ""Name""=@name, description=@description,
              ismovecommand=@ismovecommand, modifieddate=current_timestamp
              WHERE id=@id RETURNING *;",
            sqlParams
        );
    }

    public bool DeleteRobotCommand(int id)
    {
        var sqlParams = new NpgsqlParameter[] { new("id", id) };
        var result = _repo.ExecuteReader<RobotCommand>(
            "DELETE FROM robotcommand WHERE id=@id RETURNING *;",
            sqlParams
        );
        return result.Count > 0;
    }
}
