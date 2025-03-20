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
public class StringsGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var localizationFilesProvider = context.AdditionalTextsProvider.SelectLocalizationFileModels().Collect();
        var localizationTypeprovider = context.SyntaxProvider.SelectGeneratingModels().Collect();
        context.RegisterSourceOutput(localizationFilesProvider.Combine(localizationTypeprovider), Execute);
    }

    private void Execute(SourceProductionContext context, (ImmutableArray<LocalizationFileModel> Left, ImmutableArray<LocalizationGeneratingModel> Right) models)
    {
        var localizationFiles = models.Left;
        var options = models.Right.FirstOrDefault();

        foreach (var (ietfLanguageTag, group) in localizationFiles.GroupByIetfLanguageTag())
        {
            var transformer = new LocalizationCodeTransformer(group);

            var code = transformer.ToProviderCodeText(options.Namespace, ietfLanguageTag);
            context.AddSource($"{nameof(LocalizedStringProvider)}.{ietfLanguageTag}.g.cs", SourceText.From(code, Encoding.UTF8));

            if (ietfLanguageTag == options.DefaultLanguage)
            {
                var interfaceCode = transformer.ToInterfaceCodeText();
                context.AddSource($"{nameof(ILocalizedValues)}.g.cs", SourceText.From(interfaceCode, Encoding.UTF8));

                var implementationCode = transformer.ToImplementationCodeText(options.TypeName);
                context.AddSource($"{nameof(LocalizedValues)}.g.cs", SourceText.From(implementationCode, Encoding.UTF8));
            }
        }
    }
}
