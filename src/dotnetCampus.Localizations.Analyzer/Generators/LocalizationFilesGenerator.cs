using System.Collections.Immutable;
using System.Text;
using dotnetCampus.Localizations.Assets.Templates;
using dotnetCampus.Localizations.Generators.CodeTransforming;
using dotnetCampus.Localizations.Generators.ModelProviding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Localizations.Generators;

/// <summary>
/// 为所有通过 LocalizationFile 指定的文件生成对应的 C# 代码。
/// </summary>
[Generator]
public class LocalizationFilesGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var localizationFilesProvider = context.AdditionalTextsProvider.SelectLocalizationFileModels();
        var localizationTypeprovider = context.SyntaxProvider.SelectGeneratingModels();
        context.RegisterSourceOutput(localizationFilesProvider.Combine(localizationTypeprovider.Collect()), Execute);
    }

    private void Execute(SourceProductionContext context, (LocalizationFileModel Left, ImmutableArray<LocalizationGeneratingModel> Right) modelTuple)
    {
        var ((_, ietfLanguageTag, textContent), localizationGeneratingModels) = modelTuple;
        var options = localizationGeneratingModels.FirstOrDefault();

        var transformer = new LocalizationCodeTransformer(textContent, new YamlLocalizationFileReader());
        var code = transformer.ToImplementationCodeText(options.Namespace, ietfLanguageTag);
        context.AddSource($"{nameof(LocalizationValues)}.{ietfLanguageTag}.g.cs", SourceText.From(code, Encoding.UTF8));

        if (ietfLanguageTag == options.DefaultLanguage)
        {
            var keyCode = transformer.ToInterfaceCodeText(options.Namespace);
            context.AddSource($"{nameof(ILocalized_Root)}.g.cs", SourceText.From(keyCode, Encoding.UTF8));
        }
    }
}
