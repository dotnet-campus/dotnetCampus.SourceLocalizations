using System.Collections.Immutable;
using System.Text;
using dotnetCampus.Localizations.Assets.Templates;
using dotnetCampus.Localizations.Generators.CodeTransforming;
using dotnetCampus.Localizations.Generators.ModelProviding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
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
        var globalOptionsProvider = context.AnalyzerConfigOptionsProvider;
        var localizationFilesProvider = context.AdditionalTextsProvider.SelectLocalizationFileModels().Collect();
        var localizationTypeProvider = context.SyntaxProvider.SelectGeneratingModels().Collect();
        context.RegisterSourceOutput(globalOptionsProvider.Combine(localizationFilesProvider).Combine(localizationTypeProvider), Execute);
    }

    private void Execute(SourceProductionContext context, ((AnalyzerConfigOptionsProvider Left, ImmutableArray<LocalizationFileModel> Right) Left, ImmutableArray<LocalizationGeneratingModel> Right) values)
    {
        var ((options, localizationFiles), localizationTypes) = values;
        var localizationType = localizationTypes.FirstOrDefault();

        if (localizationType == default)
        {
            return;
        }

        var supportsNonIetfLanguageTag = options.GlobalOptions.TryGetValue("build_property.LocalizationSupportsNonIetfLanguageTag", out var v)
                                         && (v?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false);

        foreach (var (ietfLanguageTag, group) in localizationFiles.GroupByIetfLanguageTag(supportsNonIetfLanguageTag))
        {
            var transformer = new LocalizationCodeTransformer(group);

            var code = transformer.ToProviderCodeText(localizationType.Namespace, ietfLanguageTag);
            context.AddSource($"{nameof(LocalizedStringProvider)}.{ietfLanguageTag}.g.cs", SourceText.From(code, Encoding.UTF8));

            if (ietfLanguageTag == localizationType.DefaultLanguage)
            {
                var interfaceCode = transformer.ToInterfaceCodeText();
                context.AddSource($"{nameof(ILocalizedValues)}.g.cs", SourceText.From(interfaceCode, Encoding.UTF8));

                var implementationCode = transformer.ToImplementationCodeText(localizationType.TypeName);
                context.AddSource($"{nameof(LocalizedValues)}.g.cs", SourceText.From(implementationCode, Encoding.UTF8));
            }
        }
    }
}
