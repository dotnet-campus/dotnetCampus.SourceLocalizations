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
public class LocalizationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var localizationTypeprovider = context.SyntaxProvider.SelectGeneratingModels();
        context.RegisterSourceOutput(localizationTypeprovider, Execute);
    }

    private void Execute(SourceProductionContext context, LocalizationGeneratingModel model)
    {
        var (typeNamespace, typeName, defaultLanguage, currentLanguage) = model;
        var defaultLanguageIdentifier = IetfLanguageTagToIdentifier(defaultLanguage);
        var currentLanguageIdentifier = IetfLanguageTagToIdentifier(currentLanguage);

        var localizationFile = GeneratorInfo.GetEmbeddedTemplateFile<Localization>();
        var originalText = ReplaceNamespaceAndTypeName(localizationFile.Content, typeNamespace, typeName);
        var code = originalText
            .Replace("""ILocalized_Root Default { get; } = new LspPlaceholder("default", null)""",
                $"global::{typeNamespace}.Localizations.ILocalized_Root Default {{ get; }} = new global::{typeNamespace}.Localizations.{nameof(LocalizationValues)}_{defaultLanguageIdentifier}(null)")
            .Replace("""ILocalized_Root Current { get; private set; } = new LspPlaceholder("current", null)""", defaultLanguage == currentLanguage
                ? $"global::{typeNamespace}.Localizations.ILocalized_Root Current {{ get; private set; }} = Default"
                : $"global::{typeNamespace}.Localizations.ILocalized_Root Current {{ get; private set; }} = new global::{typeNamespace}.Localizations.{nameof(LocalizationValues)}_{currentLanguageIdentifier}(Default)");

        context.AddSource($"{typeName}.g.cs", SourceText.From(code, Encoding.UTF8));
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
