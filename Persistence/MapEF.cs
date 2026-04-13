using Microsoft.EntityFrameworkCore;
using robot_api.Models;

namespace robot_api.Persistence;

public class MapEF : IMapDataAccess
{
    private readonly RobotContext _context;

    public MapEF(RobotContext context)
    {
        _context = context;
    }

    public List<Map> GetMaps()
    {
        return _context.Maps.ToList();
    }

    public List<Map> GetSquareMaps()
    {
        return _context.Maps.Where(x => x.Columns == x.Rows && x.Rows > 0).ToList();
    }

    public Map? GetMapById(int id)
    {
        return _context.Maps.Find(id);
    }

    public Map InsertMap(Map map)
    {
        map.CreatedDate = DateTime.Now;
        map.ModifiedDate = DateTime.Now;
        _context.Maps.Add(map);
        _context.SaveChanges();
        return map;
    }

    public void UpdateMap(int id, Map updatedMap)
    {
        var existing = _context.Maps.Find(id);
        if (existing != null)
        {
            existing.Columns = updatedMap.Columns;
            existing.Rows = updatedMap.Rows;
            existing.Name = updatedMap.Name;
            existing.Description = updatedMap.Description;
            existing.ModifiedDate = DateTime.Now;
            _context.SaveChanges();
        }
    }

    public bool DeleteMap(int id)
    {
        var map = _context.Maps.Find(id);
        if (map == null)
            return false;
        _context.Maps.Remove(map);
        _context.SaveChanges();
        return true;
    }
}
