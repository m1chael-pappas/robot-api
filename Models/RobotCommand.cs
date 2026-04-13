namespace robot_api.Models;

public class RobotCommand(
    int id,
    string name,
    bool isMoveCommand,
    DateTime createdDate,
    DateTime modifiedDate,
    string? description = null
)
{
    public RobotCommand()
        : this(0, "", false, DateTime.MinValue, DateTime.MinValue) { }

    public int Id { get; set; } = id;
    public string Name { get; set; } = name;
    public string? Description { get; set; } = description;
    public bool IsMoveCommand { get; set; } = isMoveCommand;
    public DateTime CreatedDate { get; set; } = createdDate;
    public DateTime ModifiedDate { get; set; } = modifiedDate;
}
