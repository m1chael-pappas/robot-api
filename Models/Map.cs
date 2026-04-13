namespace robot_api.Models;

public class Map(
    int id,
    int columns,
    int rows,
    string name,
    DateTime createdDate,
    DateTime modifiedDate,
    string? description = null
)
{
    public Map() : this(0, 0, 0, "", DateTime.MinValue, DateTime.MinValue) { }

    public int Id { get; set; } = id;
    public int Columns { get; set; } = columns;
    public int Rows { get; set; } = rows;
    public string Name { get; set; } = name;
    public string? Description { get; set; } = description;
    public DateTime CreatedDate { get; set; } = createdDate;
    public DateTime ModifiedDate { get; set; } = modifiedDate;
}
