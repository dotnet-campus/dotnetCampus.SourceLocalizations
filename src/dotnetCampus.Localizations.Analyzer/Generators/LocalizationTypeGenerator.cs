using System.Collections.Immutable;
using System.Text;
using dotnetCampus.Localizations.Assets.Templates;
using dotnetCampus.Localizations.Generators.ModelProviding;
using dotnetCampus.Localizations.Utils.CodeAnalysis;
using Microsoft.CodeAnalysis;
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
        var localizationFilesProvider = context.AdditionalTextsProvider.SelectLocalizationFileModels();
        var localizationTypeprovider = context.SyntaxProvider.SelectGeneratingModels();
        context.RegisterSourceOutput(localizationTypeprovider.Combine(localizationFilesProvider.Collect()), Execute);
    }

    private void Execute(SourceProductionContext context, (LocalizationGeneratingModel Left, ImmutableArray<LocalizationFileModel> Right) modelTuple)
    {
        var ((typeNamespace, typeName, defaultLanguage, currentLanguage), models) = modelTuple;
        var defaultLanguageIdentifier = IetfLanguageTagToIdentifier(defaultLanguage);
        var currentLanguageIdentifier = IetfLanguageTagToIdentifier(currentLanguage);

        // 生成 Localization.current.g.cs
        var currentCode = GenerateSetCurrentMethod(typeNamespace, typeName, models);
        context.AddSource($"{typeName}.current.g.cs", SourceText.From(currentCode, Encoding.UTF8));

        // 生成 Localization.default.g.cs
        var localizationFile = GeneratorInfo.GetEmbeddedTemplateFile<Localization>();
        var originalText = ReplaceNamespaceAndTypeName(localizationFile.Content, typeNamespace, typeName);
        var defaultCode = originalText
            .Replace("""ILocalizedValues Default { get; } = new LspPlaceholder("default", null)""",
                $"global::{GeneratorInfo.RootNamespace}.ILocalizedValues Default {{ get; }} = new global::{GeneratorInfo.RootNamespace}.{nameof(LocalizationValues)}_{defaultLanguageIdentifier}(null)")
            .Replace("""ILocalizedValues Current { get; private set; } = new LspPlaceholder("current", null)""", defaultLanguage == currentLanguage
                ? $"global::{GeneratorInfo.RootNamespace}.ILocalizedValues Current {{ get; private set; }} = Default"
                : $"global::{GeneratorInfo.RootNamespace}.ILocalizedValues Current {{ get; private set; }} = new global::{GeneratorInfo.RootNamespace}.{nameof(LocalizationValues)}_{currentLanguageIdentifier}(Default)");
        context.AddSource($"{typeName}.default.g.cs", SourceText.From(defaultCode, Encoding.UTF8));
    }

    private string GenerateSetCurrentMethod(string typeNamespace, string typeName, ImmutableArray<LocalizationFileModel> models) => $$"""
#nullable enable

namespace {{typeNamespace}};

partial class {{typeName}}
{
    /// <summary>
    /// 设置当前的本地化字符串集。
    /// </summary>
    /// <param name="ietfLanguageTag">要设置的 IETF 语言标签。</param>
    public static void SetCurrent(string ietfLanguageTag)
    {
        Current = ietfLanguageTag switch
        {
{{string.Join("\n", models.Select(x => ConvertModelToPatternMatch(typeNamespace, x)))}}
            _ => throw new global::System.ArgumentException($"The language tag {ietfLanguageTag} is not supported.", nameof(ietfLanguageTag)),
        };
    }
}

""";

    private string ConvertModelToPatternMatch(string typeNamespace, LocalizationFileModel model)
    {
        // "zh-hans" => new Lang_ZhHans(Default),
        return $"            \"{model.IetfLanguageTag}\" => new global::{GeneratorInfo.RootNamespace}.{nameof(LocalizationValues)}_{IetfLanguageTagToIdentifier(model.IetfLanguageTag)}(Default),";
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
