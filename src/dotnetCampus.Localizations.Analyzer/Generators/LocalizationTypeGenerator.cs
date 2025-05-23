﻿using System.Collections.Immutable;
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
/// [LocalizedConfiguration(Default = "zh-CN", Current = "zh-CN")]
/// public static partial class Lang;
/// </code>
/// </example>
[Generator]
public class LocalizationTypeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var globalOptionsProvider = context.AnalyzerConfigOptionsProvider;
        var localizationFilesProvider = context.SelectLocalizationFileModels();
        var localizationTypeprovider = context.SyntaxProvider.SelectGeneratingModels();
        context.RegisterSourceOutput(localizationTypeprovider.Combine(globalOptionsProvider).Combine(localizationFilesProvider.Collect()), Execute);
    }

    private void Execute(SourceProductionContext context, ((LocalizationGeneratingModel Left, AnalyzerConfigOptionsProvider Right) Left, ImmutableArray<LocalizationFileModel> Right) values)
    {
        var (((typeNamespace, typeName, defaultLanguage, currentLanguage, supportsNotifyChanged), options), localizationFiles) = values;

        var isIncludedByPackageReference = options.GlobalOptions.GetBoolean("LocalizationIsIncludedByPackageReference");
        var supportsNonIetfLanguageTag = options.GlobalOptions.GetBoolean("LocalizationSupportsNonIetfLanguageTag");
        var allLocalizationModels = localizationFiles.GroupByIetfLanguageTag(supportsNonIetfLanguageTag)
            .ToImmutableSortedDictionary(x => x.IetfLanguageTag, x => x.Models);
        var allTags = allLocalizationModels.Keys.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

        if (!isIncludedByPackageReference)
        {
            // 只有直接引用本地化库的项目才会生成本地化代码。
            return;
        }

        // 生成 Localization.g.cs
        var localizationFile = supportsNotifyChanged
            ? GeneratorInfo.GetEmbeddedTemplateFile<NotifiableLocalization>()
            : GeneratorInfo.GetEmbeddedTemplateFile<ImmutableLocalization>();
        var originalText = ReplaceNamespaceAndTypeName(localizationFile.Content, typeNamespace, typeName);
        var localizedValuesTypeName = supportsNotifyChanged ? nameof(NotifiableLocalizedValues) : nameof(ImmutableLocalizedValues);
        var defaultCode = originalText
            .Replace("DEFAULT_IETF_LANGUAGE_TAG", defaultLanguage.ToLowerInvariant())
            .Replace("\"CURRENT_IETF_LANGUAGE_TAG\"", currentLanguage is null ? "global::System.Globalization.CultureInfo.CurrentUICulture.Name" : $"\"{currentLanguage.ToLowerInvariant()}\"")
            .FlagReplace(GenerateCreateLocalizedValues(defaultLanguage, allLocalizationModels))
            .Flag2Replace(GenerateIetfLanguageTagList(allLocalizationModels.Keys))
            .Replace("ILocalizedValues", $"global::{GeneratorInfo.RootNamespace}.ILocalizedValues")
            .Replace("PlaceholderLocalizedValues", $" global::{GeneratorInfo.RootNamespace}.{localizedValuesTypeName}");
        if (supportsNotifyChanged)
        {
            defaultCode = defaultCode.Replace(
                "ILocalizedStringProvider Wrap(ILocalizedStringProvider rawProvider) => rawProvider",
                $"global::{GeneratorInfo.RootNamespace}.MutableLocalizedStringProvider Wrap(ILocalizedStringProvider rawProvider) => new global::{GeneratorInfo.RootNamespace}.MutableLocalizedStringProvider{{ Provider = rawProvider}}");
        }
        context.AddSource($"{typeName}.g.cs", SourceText.From(defaultCode, Encoding.UTF8));
    }

    private string GenerateIetfLanguageTagList(IEnumerable<string> languageTags) => $"""
{string.Join("\n", languageTags.Select(x => $"        \"{x}\","))}
""";

    private string GenerateCreateLocalizedValues(string defaultIetfTag, IReadOnlyDictionary<string, ImmutableArray<LocalizationFileModel>> models) => $"""
{string.Join("\n", models.Select(x => ConvertModelToPatternMatch(defaultIetfTag, x.Key)))}
""";

    private string ConvertModelToPatternMatch(string defaultIetfTag, string ietfTag)
    {
        var tagIdentifier = IetfLanguageTagToIdentifier(ietfTag);
        var defaultProvider = ietfTag == defaultIetfTag
            ? "null"
            : "_default.LocalizedStringProvider";
        return $"""
            "{ietfTag.ToLowerInvariant()}" => new global::{GeneratorInfo.RootNamespace}.{nameof(LocalizedStringProvider)}_{tagIdentifier}({defaultProvider}),
""";
    }

    private static string ReplaceNamespaceAndTypeName(string sourceText, string rootNamespace, string typeName)
    {
        return sourceText
            .Replace("namespace dotnetCampus.Localizations.Assets.Templates;", $"namespace {rootNamespace};")
            .Replace("partial class ImmutableLocalization", $"partial class {typeName}")
            .Replace("partial class NotifiableLocalization", $"partial class {typeName}")
            .Replace("static ImmutableLocalization()", $"static {typeName}()")
            .Replace("static NotifiableLocalization()", $"static {typeName}()");
    }
}
