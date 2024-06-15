using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Localizations.Generators;

[Generator]
public class PostInitializationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(Execute);
    }

    private void Execute(IncrementalGeneratorPostInitializationContext context)
    {
        var code = CollectPostInitializationCode();
        context.AddSource("ILocalizedValues.g.cs", SourceText.From(code, Encoding.UTF8));
    }

    private string CollectPostInitializationCode()
    {
        return $"""
namespace {GeneratorInfo.RootNamespace};

partial interface ILocalizedValues;

""";
    }
}
