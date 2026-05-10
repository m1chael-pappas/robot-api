using Npgsql;
using robot_api.Models;

namespace robot_api.Persistence;

public class RobotCommandADO : IRobotCommandDataAccess
{
    private static string CONNECTION_STRING => DbConfig.ConnectionString;

    private RobotCommand ReadRobotCommand(NpgsqlDataReader dr)
    {
        return new RobotCommand
        {
            Id = (int)dr["id"],
            Name = (string)dr["name"],
            Description = dr["description"] as string,
            IsMoveCommand = (bool)dr["is_move_command"],
            CreatedDate = (DateTime)dr["created_date"],
            ModifiedDate = (DateTime)dr["modified_date"],
        };
    }

    public List<RobotCommand> GetRobotCommands()
    {
        var robotCommands = new List<RobotCommand>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM robot_command", conn);
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
            "SELECT * FROM robot_command WHERE is_move_command = true",
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
        using var cmd = new NpgsqlCommand("SELECT * FROM robot_command WHERE id = @id", conn);
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
            @"INSERT INTO robot_command (name, description, is_move_command, created_date, modified_date)
              VALUES (@name, @description, @is_move_command, @created_date, @modified_date)
              RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("name", robotCommand.Name);
        cmd.Parameters.AddWithValue(
            "description",
            (object?)robotCommand.Description ?? DBNull.Value
        );
        cmd.Parameters.AddWithValue("is_move_command", robotCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("created_date", DateTime.Now);
        cmd.Parameters.AddWithValue("modified_date", DateTime.Now);
        var id = (int)cmd.ExecuteScalar()!;
        robotCommand.Id = id;
        return robotCommand;
    }

    public void UpdateRobotCommand(int id, RobotCommand robotCommand)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"UPDATE robot_command SET name = @name, description = @description,
              is_move_command = @is_move_command, modified_date = @modified_date
              WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("name", robotCommand.Name);
        cmd.Parameters.AddWithValue(
            "description",
            (object?)robotCommand.Description ?? DBNull.Value
        );
        cmd.Parameters.AddWithValue("is_move_command", robotCommand.IsMoveCommand);
        cmd.Parameters.AddWithValue("modified_date", DateTime.Now);
        cmd.ExecuteNonQuery();
    }

    public bool DeleteRobotCommand(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM robot_command WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return cmd.ExecuteNonQuery() > 0;
    }
}
