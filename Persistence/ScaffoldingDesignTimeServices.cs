using EntityFrameworkCore.Scaffolding.Handlebars;
using EntityFrameworkCore.Scaffolding.Handlebars.Internal;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace robot_api.Persistence;

public class ScaffoldingDesignTimeServices : IDesignTimeServices
{
    public void ConfigureDesignTimeServices(IServiceCollection services)
    {
        services.AddHandlebarsScaffolding(options =>
        {
            options.ReverseEngineerOptions = ReverseEngineerOptions.DbContextAndEntities;
        });

        // Replace the package's default DbContext generator with our subclass
        // so the OnModelCreatingPartial call and declaration are dropped at
        // generation time (no regex / no post-processing of the rendered file).
#pragma warning disable EF1001
        services.Replace(
            ServiceDescriptor.Singleton<ICSharpDbContextGenerator, NoPartialHbsDbContextGenerator>()
        );
#pragma warning restore EF1001
    }
}
