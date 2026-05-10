using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using robot_api.Authentication;
using robot_api.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

DbConfig.ConnectionString =
    builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

// Toggle the active persistence layer for the demo by uncommenting one block.
// 4.1P (ADO.NET):
// builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandADO>();
// builder.Services.AddScoped<IMapDataAccess, MapADO>();
// 4.2C (FastMember + IRepository):
// builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandRepository>();
// builder.Services.AddScoped<IMapDataAccess, MapRepository>();
// 4.3D / 4.4HD (EF Core, snake_case → PascalCase via Handlebars-templated scaffold):
builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandEF>();
builder.Services.AddScoped<IMapDataAccess, MapEF>();
builder.Services.AddDbContext<RobotContext>(options =>
    options.UseNpgsql(DbConfig.ConnectionString)
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Robot Controller API",
            Description =
                "New backend service that provides resources for the Moon robot simulator.",
            Contact = new OpenApiContact
            {
                Name = "Michael Pappas",
                Email = "s225071597@deakin.edu.au",
            },
        }
    );

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder
    .Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
        "BasicAuthentication",
        default
    );

builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"))
    .AddPolicy("UserOnly", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));

var app = builder.Build();

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(setup => setup.InjectStylesheet("/styles/theme-deakin.css"));

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
