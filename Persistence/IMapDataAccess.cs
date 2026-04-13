using robot_api.Models;

namespace robot_api.Persistence;

public interface IMapDataAccess
{
    List<Map> GetMaps();
    List<Map> GetSquareMaps();
    Map? GetMapById(int id);
    Map InsertMap(Map map);
    void UpdateMap(int id, Map map);
    bool DeleteMap(int id);
}
