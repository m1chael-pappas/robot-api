using System.Reflection;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
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

var app = builder.Build();

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(setup => setup.InjectStylesheet("/styles/theme-deakin.css"));

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
