using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithOpenApiRoutePattern("/openapi/v1.json");
});

app.UseHttpsRedirection();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
    });
});

// --- In-memory state ---

var commands = new List<RobotCommand>
{
    new(0, "PLACE", "Place the robot at X,Y facing direction D", false),
    new(1, "LEFT", "Turn the robot 90 degrees left", false),
    new(2, "RIGHT", "Turn the robot 90 degrees right", false),
    new(3, "MOVE", "Move the robot one step forward", true),
};

var map = new RobotMap(5);

// --- Endpoints ---

app.MapGet("/", () => "Hello, Robot!");

app.MapGet("/robot-commands", () => Results.Ok(commands));

app.MapGet("/robot-commands/move", () => Results.Ok(commands.Where(c => c.MovesRobot)));

app.MapGet(
    "/robot-commands/{id:int}",
    (int id) =>
    {
        var cmd = commands.FirstOrDefault(c => c.Id == id);
        return cmd is not null ? Results.Ok(cmd) : Results.NotFound();
    }
);

app.MapPost(
    "/robot-commands",
    (RobotCommand cmd) =>
    {
        var newCmd = cmd with { Id = commands.Count };
        commands.Add(newCmd);
        return Results.Created($"/robot-commands/{newCmd.Id}", newCmd);
    }
);

app.MapPut(
    "/robot-commands/{id:int}",
    (int id, RobotCommand updated) =>
    {
        var index = commands.FindIndex(c => c.Id == id);
        if (index == -1)
            return Results.NotFound();
        commands[index] = updated with { Id = id };
        return Results.NoContent();
    }
);

app.MapGet("/robot-map", () => Results.Ok(map));

app.MapGet(
    "/robot-map/{coordinate}",
    (string coordinate) =>
    {
        var parts = coordinate.Split('-');
        if (
            parts.Length != 2
            || !int.TryParse(parts[0], out int x)
            || !int.TryParse(parts[1], out int y)
        )
            return Results.BadRequest("Coordinate must be in the form x-y, e.g. 3-5");

        bool onMap = x >= 0 && x < map.Size && y >= 0 && y < map.Size;
        return Results.Ok(onMap);
    }
);

app.MapPut(
    "/robot-map",
    (RobotMap updated) =>
    {
        if (updated.Size < 2 || updated.Size > 100)
            return Results.BadRequest("Map size must be between 2 and 100.");
        map = updated;
        return Results.NoContent();
    }
);

app.Run();

// --- Data Structures ---

record RobotCommand(int Id, string Name, string Description, bool MovesRobot);

record RobotMap(int Size);
