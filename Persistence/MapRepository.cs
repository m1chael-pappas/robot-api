using Npgsql;
using robot_api.Models;

namespace robot_api.Persistence;

public class MapRepository : IMapDataAccess, IRepository
{
    private IRepository _repo => this;

    public List<Map> GetMaps()
    {
        return _repo.ExecuteReader<Map>("SELECT * FROM public.map");
    }

    public List<Map> GetSquareMaps()
    {
        return _repo.ExecuteReader<Map>("SELECT * FROM public.map WHERE issquare = true");
    }

    public Map? GetMapById(int id)
    {
        var sqlParams = new NpgsqlParameter[] { new("id", id) };
        var result = _repo.ExecuteReader<Map>("SELECT * FROM public.map WHERE id = @id", sqlParams);
        return result.FirstOrDefault();
    }

    public Map InsertMap(Map map)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("columns", map.Columns),
            new("rows", map.Rows),
            new("name", map.Name),
            new("description", map.Description ?? (object)DBNull.Value),
        };
        var result = _repo
            .ExecuteReader<Map>(
                @"INSERT INTO map (columns, rows, ""Name"", description, createddate, modifieddate)
              VALUES (@columns, @rows, @name, @description, current_timestamp, current_timestamp)
              RETURNING *;",
                sqlParams
            )
            .Single();
        return result;
    }

    public void UpdateMap(int id, Map updatedMap)
    {
        var sqlParams = new NpgsqlParameter[]
        {
            new("id", id),
            new("columns", updatedMap.Columns),
            new("rows", updatedMap.Rows),
            new("name", updatedMap.Name),
            new("description", updatedMap.Description ?? (object)DBNull.Value),
        };
        _repo.ExecuteReader<Map>(
            @"UPDATE map SET columns=@columns, rows=@rows, ""Name""=@name,
              description=@description, modifieddate=current_timestamp
              WHERE id=@id RETURNING *;",
            sqlParams
        );
    }

    public bool DeleteMap(int id)
    {
        var sqlParams = new NpgsqlParameter[] { new("id", id) };
        var result = _repo.ExecuteReader<Map>(
            "DELETE FROM map WHERE id=@id RETURNING *;",
            sqlParams
        );
        return result.Count > 0;
    }
}
