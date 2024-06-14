using System.Collections.Immutable;
using dotnetCampus.Localizations.Utils;
using Microsoft.CodeAnalysis;

namespace dotnetCampus.Localizations.Generators;

[Generator]
public class LanguageSubtagRegistryGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.AdditionalTextsProvider.Where(x =>
                x.Path.EndsWith("language-subtag-registry.txt", StringComparison.OrdinalIgnoreCase))
            .Collect();
        context.RegisterImplementationSourceOutput(provider, Execute);
    }

    private void Execute(SourceProductionContext context, ImmutableArray<AdditionalText> texts)
    {
        if (texts.Length is 0)
        {
            throw new InvalidOperationException("No language subtag registry file found.");
        }

        var text = texts.Single().GetText()!.ToString();
        var items = LanguageSubtagRegistryParser.ParseRegistryFile(text).ToImmutableArray();
    }

}
