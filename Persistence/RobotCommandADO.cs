using Npgsql;
using robot_api.Models;

namespace robot_api.Persistence;

public class RobotCommandADO : IRobotCommandDataAccess
{
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=;Database=sit331";

    private RobotCommand ReadRobotCommand(NpgsqlDataReader dr)
    {
        return new RobotCommand(
            (int)dr["id"],
            (string)dr["Name"],
            (bool)dr["ismovecommand"],
            (DateTime)dr["createddate"],
            (DateTime)dr["modifieddate"],
            dr["description"] as string
        );
    }

    public List<RobotCommand> GetRobotCommands()
    {
        var robotCommands = new List<RobotCommand>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand", conn);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            robotCommands.Add(ReadRobotCommand(dr));
        }
        return robotCommands;
    }

    public List<RobotCommand> GetMoveCommands()
    {
        var robotCommands = new List<RobotCommand>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            "SELECT * FROM robotcommand WHERE ismovecommand = true",
            conn
        );
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            robotCommands.Add(ReadRobotCommand(dr));
        }
        return robotCommands;
    }

    public RobotCommand? GetRobotCommandById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM robotcommand WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            return ReadRobotCommand(dr);
        }
        return null;
    }

    public RobotCommand InsertRobotCommand(RobotCommand robotCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"INSERT INTO robotcommand (""Name"", description, ismovecommand, createddate, modifieddate)
              VALUES (@name, @description, @ismovecommand, @createddate, @modifieddate)
              RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("name", robotCommand.Name);
        cmd.Parameters.AddWithValue(
            "description",
            (object?)robotCommand.Description ?? DBNull.Value
        );
        cmd.Parameters.AddWithValue("ismovecommand", robotCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("createddate", DateTime.Now);
        cmd.Parameters.AddWithValue("modifieddate", DateTime.Now);
        var id = (int)cmd.ExecuteScalar()!;
        robotCommand.Id = id;
        return robotCommand;
    }

    public void UpdateRobotCommand(int id, RobotCommand robotCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"UPDATE robotcommand SET ""Name"" = @name, description = @description,
              ismovecommand = @ismovecommand, modifieddate = @modifieddate
              WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("name", robotCommand.Name);
        cmd.Parameters.AddWithValue(
            "description",
            (object?)robotCommand.Description ?? DBNull.Value
        );
        cmd.Parameters.AddWithValue("ismovecommand", robotCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("modifieddate", DateTime.Now);
        cmd.ExecuteNonQuery();
    }

    public bool DeleteRobotCommand(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM robotcommand WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return cmd.ExecuteNonQuery() > 0;
    }
}
