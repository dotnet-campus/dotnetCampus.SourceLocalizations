using System.Collections.Immutable;
using System.Text;
using dotnetCampus.Localizations.Assets.Templates;
using dotnetCampus.Localizations.Generators.ModelProviding;
using dotnetCampus.Localizations.Utils.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;

using static dotnetCampus.Localizations.Generators.ModelProviding.IetfLanguageTagExtensions;

namespace dotnetCampus.Localizations.Generators;

/// <summary>
/// 为静态的本地化中心类生成分部实现。
/// </summary>
/// <example>
/// 本生成器会为下方示例中的类型生成分部实现：
/// <code>
/// [LocalizedConfiguration(Default = "zh-hans", Current = "zh-hans")]
/// public static partial class Lang;
/// </code>
/// </example>
[Generator]
public class LocalizationTypeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var globalOptionsProvider = context.AnalyzerConfigOptionsProvider;
        var localizationFilesProvider = context.AdditionalTextsProvider.SelectLocalizationFileModels();
        var localizationTypeprovider = context.SyntaxProvider.SelectGeneratingModels();
        context.RegisterSourceOutput(localizationTypeprovider.Combine(globalOptionsProvider).Combine(localizationFilesProvider.Collect()), Execute);
    }

    private void Execute(SourceProductionContext context, ((LocalizationGeneratingModel Left, AnalyzerConfigOptionsProvider Right) Left, ImmutableArray<LocalizationFileModel> Right) values)
    {
        var (((typeNamespace, typeName, defaultLanguage, currentLanguage), options), localizationFiles) = values;

        var isIncludedByPackageReference = options.GlobalOptions.GetBoolean("LocalizationIsIncludedByPackageReference");
        var supportsNonIetfLanguageTag = options.GlobalOptions.GetBoolean("LocalizationSupportsNonIetfLanguageTag");

        if (!isIncludedByPackageReference)
        {
            // 只有直接引用本地化库的项目才会生成本地化代码。
            return;
        }

        // 生成 Localization.g.cs
        var localizationFile = GeneratorInfo.GetEmbeddedTemplateFile<Localization>();
        var originalText = ReplaceNamespaceAndTypeName(localizationFile.Content, typeNamespace, typeName);
        var defaultCode = originalText
            .Replace("LocalizedValues _default = new LocalizedValues(null!);",
                $"global::{GeneratorInfo.RootNamespace}.LocalizedValues _default = CreateLocalizedValues(\"{defaultLanguage}\");")
            .Replace("LocalizedValues _current = new LocalizedValues(null!);", defaultLanguage == currentLanguage
                ? $"global::{GeneratorInfo.RootNamespace}.LocalizedValues _current = _default;"
                : $"global::{GeneratorInfo.RootNamespace}.LocalizedValues _current = CreateLocalizedValues(\"{currentLanguage}\");");
        defaultCode = TemplateRegexes.FlagRegex.Replace(defaultCode, GenerateCreateLocalizedValues(defaultLanguage, localizationFiles, supportsNonIetfLanguageTag));
        defaultCode = defaultCode
            .Replace("ILocalizedValues", $"global::{GeneratorInfo.RootNamespace}.ILocalizedValues")
            .Replace(" LocalizedValues", $" global::{GeneratorInfo.RootNamespace}.LocalizedValues");
        context.AddSource($"{typeName}.g.cs", SourceText.From(defaultCode, Encoding.UTF8));
    }

    private string GenerateCreateLocalizedValues(string defaultIetfTag, ImmutableArray<LocalizationFileModel> models, bool supportsNonIetfLanguageTag) => $"""

{string.Join("\n", models.GroupByIetfLanguageTag(supportsNonIetfLanguageTag).Select(x => ConvertModelToPatternMatch(defaultIetfTag, x.IetfLanguageTag)))}
""";

    private string ConvertModelToPatternMatch(string defaultIetfTag, string ietfTag)
    {
        var tagIdentifier = IetfLanguageTagToIdentifier(ietfTag);
        var defaultProvider = ietfTag == defaultIetfTag
            ? "null"
            : "_default.LocalizedStringProvider";
        return $"""
        "{ietfTag}" => new global::{GeneratorInfo.RootNamespace}.LocalizedValues(new global::{GeneratorInfo.RootNamespace}.{nameof(LocalizedStringProvider)}_{tagIdentifier}({defaultProvider})),
""";
    }

    private static string ReplaceNamespaceAndTypeName(string sourceText, string rootNamespace, string? typeName)
    {
        var sourceSpan = sourceText.AsSpan();

        var namespaceKeywordIndex = sourceText.IndexOf("namespace", StringComparison.Ordinal);
        var namespaceStartIndex = namespaceKeywordIndex + "namespace".Length + 1;
        var namespaceEndIndex = sourceText.IndexOf(";", namespaceStartIndex, StringComparison.Ordinal);
        var typeKeywordMatch = TemplateRegexes.TypeRegex.Match(sourceText);

        if (typeName is null || !typeKeywordMatch.Success)
        {
            return string.Concat(
                sourceSpan.Slice(0, namespaceStartIndex).ToString(),
                rootNamespace,
                sourceSpan.Slice(namespaceEndIndex, sourceSpan.Length - namespaceEndIndex).ToString()
            );
        }

        var typeNameIndex = typeKeywordMatch.Groups[1].Index;
        var typeNameLength = typeKeywordMatch.Groups[1].Length;

        return string.Concat(
            sourceSpan.Slice(0, namespaceStartIndex).ToString(),
            rootNamespace,
            sourceSpan.Slice(namespaceEndIndex, typeNameIndex - namespaceEndIndex).ToString(),
            typeName,
            sourceSpan.Slice(typeNameIndex + typeNameLength, sourceSpan.Length - typeNameIndex - typeNameLength).ToString()
        );
    }
}
