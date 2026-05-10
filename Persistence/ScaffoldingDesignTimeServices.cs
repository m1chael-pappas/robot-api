using System.Text.RegularExpressions;
using EntityFrameworkCore.Scaffolding.Handlebars;
using HandlebarsDotNet;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace robot_api.Persistence;

public class ScaffoldingDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
        services.AddHandlebarsScaffolding(options =>
        {
            options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;
        });

        Handlebars.RegisterHelper(
            "strip-partial",
            (writer, _, parameters) =>
            {
                if (parameters.Length == 0 || parameters[0] is not string text)
                    return;

                text = Regex.Replace(
                    text,
                    @"^[ \t]*OnModelCreatingPartial\(modelBuilder\);[ \t]*\r?\n?",
                    string.Empty,
                    RegexOptions.Multiline
                );
                text = Regex.Replace(
                    text,
                    @"\r?\n[ \t]*partial void OnModelCreatingPartial\(ModelBuilder modelBuilder\);[ \t]*\r?\n?",
                    string.Empty
                );

                writer.WriteSafeString(text);
            }
        );
    }
}
