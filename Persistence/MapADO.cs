using Npgsql;
using robot_api.Models;

namespace robot_api.Persistence;

public class MapADO : IMapDataAccess
{
    private const string CONNECTION_STRING =
        "Host=localhost;Username=postgres;Password=;Database=sit331";

    private Map ReadMap(NpgsqlDataReader dr)
    {
        return new Map(
            (int)dr["id"],
            (int)dr["columns"],
            (int)dr["rows"],
            (string)dr["Name"],
            (DateTime)dr["createddate"],
            (DateTime)dr["modifieddate"],
            dr["description"] as string
        );
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
        using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE issquare = true", conn);
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
            @"INSERT INTO map (columns, rows, ""Name"", description, createddate, modifieddate)
              VALUES (@columns, @rows, @name, @description, @createddate, @modifieddate)
              RETURNING id",
            conn
        );
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("createddate", DateTime.Now);
        cmd.Parameters.AddWithValue("modifieddate", DateTime.Now);
        var id = (int)cmd.ExecuteScalar()!;
        map.Id = id;
        return map;
    }

    public void UpdateMap(int id, Map map)
    {
        using var conn = new NpgsqlConnection(CONNECTION_STRING);
        conn.Open();
        using var cmd = new NpgsqlCommand(
            @"UPDATE map SET columns = @columns, rows = @rows, ""Name"" = @name,
              description = @description, modifieddate = @modifieddate
              WHERE id = @id",
            conn
        );
        cmd.Parameters.AddWithValue("id", id);
        cmd.Parameters.AddWithValue("columns", map.Columns);
        cmd.Parameters.AddWithValue("rows", map.Rows);
        cmd.Parameters.AddWithValue("name", map.Name);
        cmd.Parameters.AddWithValue("description", (object?)map.Description ?? DBNull.Value);
        cmd.Parameters.AddWithValue("modifieddate", DateTime.Now);
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
