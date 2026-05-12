using EntityFrameworkCore.Scaffolding.Handlebars;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.Extensions.Options;

namespace robot_api.Persistence;

// Subclass of the package's HbsCSharpDbContextGenerator that removes the
// OnModelCreatingPartial call and partial-method declaration from the
// `on-model-creating` template-data entry before it reaches the Handlebars
// template. This way the rendered DbContext is non-partial without any
// post-render regex stripping.
public class NoPartialHbsDbContextGenerator : HbsCSharpDbContextGenerator
{
    private const string PartialCall = "OnModelCreatingPartial(modelBuilder);";
    private const string PartialDecl =
        "partial void OnModelCreatingPartial(ModelBuilder modelBuilder);";

    public NoPartialHbsDbContextGenerator(
        IProviderConfigurationCodeGenerator providerConfigurationCodeGenerator,
        IAnnotationCodeGenerator annotationCodeGenerator,
        IDbContextTemplateService dbContextTemplateService,
        IEntityTypeTransformationService entityTypeTransformationService,
        ICSharpHelper cSharpHelper,
        IOptions<HandlebarsScaffoldingOptions> options
    )
        : base(
            providerConfigurationCodeGenerator,
            annotationCodeGenerator,
            dbContextTemplateService,
            entityTypeTransformationService,
            cSharpHelper,
            options
        ) { }

    protected override void GenerateOnModelCreating(IModel model)
    {
        base.GenerateOnModelCreating(model);

        if (!TemplateData.TryGetValue("on-model-creating", out var raw) || raw is not string text)
            return;

        TemplateData["on-model-creating"] = StripPartialMethod(text);
    }

    private static string StripPartialMethod(string text)
    {
        var output = new List<string>();
        foreach (var line in text.Replace("\r\n", "\n").Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed == PartialCall)
                continue;
            if (trimmed == PartialDecl)
            {
                if (output.Count > 0 && string.IsNullOrWhiteSpace(output[^1]))
                    output.RemoveAt(output.Count - 1);
                continue;
            }
            output.Add(line);
        }
        return string.Join('\n', output);
    }
}
