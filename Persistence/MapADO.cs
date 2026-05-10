using Npgsql;
using robot_api.Models;

namespace robot_api.Persistence;

public class MapADO : IMapDataAccess
{
    private static string CONNECTION_STRING => DbConfig.ConnectionString;

    private Map ReadMap(NpgsqlDataReader dr)
    {
        return new Map
        {
            Id = (int)dr["id"],
            Columns = (int)dr["columns"],
            Rows = (int)dr["rows"],
            Name = (string)dr["name"],
            Description = dr["description"] as string,
            CreatedDate = (DateTime)dr["created_date"],
            ModifiedDate = (DateTime)dr["modified_date"],
        };
    }

    public List<Map> GetMaps()
    {
        var maps = new List<Map>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM map", conn);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            maps.Add(ReadMap(dr));
        }
        return maps;
    }

    public List<Map> GetSquareMaps()
    {
        var maps = new List<Map>();
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE is_square = true", conn);
        using var dr = cmd.ExecuteReader();
        while (dr.Read())
        {
            maps.Add(ReadMap(dr));
        }
        return maps;
    }

    public Map? GetMapById(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        using var dr = cmd.ExecuteReader();
        if (dr.Read())
        {
            return ReadMap(dr);
        }
        return null;
    }

    public Map InsertMap(Map map)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"INSERT INTO map (columns, rows, name, description, created_date, modified_date)
              VALUES (@columns, @rows, @name, @description, @created_date, @modified_date)
              RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("created_date", DateTime.Now);
        cmd.Parameters.AddWithValue("modified_date", DateTime.Now);
        var id = (int)cmd.ExecuteScalar()!;
        map.Id = id;
        return map;
    }

    public void UpdateMap(int id, Map map)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"UPDATE map SET columns = @columns, rows = @rows, name = @name,
              description = @description, modified_date = @modified_date
              WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("modified_date", DateTime.Now);
        cmd.ExecuteNonQuery();
    }

    public bool DeleteMap(int id)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand("DELETE FROM map WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("id", id);
        return cmd.ExecuteNonQuery() > 0;
    }
}
