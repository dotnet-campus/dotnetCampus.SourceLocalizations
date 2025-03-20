using System.Collections.Immutable;
using System.Globalization;
using dotnetCampus.Localizations.Assets.Templates;
using dotnetCampus.Localizations.Utils.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dotnetCampus.Localizations.Generators.ModelProviding;

/// <summary>
/// 为 <see cref="LocalizationGeneratingModel"/> 提供扩展方法。
/// </summary>
public static class LocalizationGeneratingModelExtensions
{
    public static IncrementalValuesProvider<LocalizationFileModel> SelectLocalizationFileModels(this IncrementalValuesProvider<AdditionalText> provider) =>
        provider.Where(x =>
                x.Path.EndsWith(".toml", StringComparison.OrdinalIgnoreCase)
                || x.Path.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase)
                || x.Path.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
            .Select((x, ct) =>
            {
                var ietfLanguageTag = GuessIetfLanguageTag(Path.GetFileNameWithoutExtension(x.Path));
                var extension = Path.GetExtension(x.Path) switch
                {
                    ".toml" => "toml",
                    ".yaml" or ".yml" => "yaml",
                    _ => throw new NotSupportedException($"Unsupported localization file format: {x.Path}"),
                };
                var text = x.GetText(ct)!.ToString();
                return ietfLanguageTag is null ? (LocalizationFileModel?)null : new LocalizationFileModel(extension, ietfLanguageTag, text);
            })
            .Where(x => x is not null)
            .Select((x, ct) => (LocalizationFileModel)x!);

    /// <summary>
    /// 从文件名中猜测到底哪一段才是 IETF 语言标签。
    /// </summary>
    /// <param name="fileNameWithoutExtension">没有扩展名的文件名。</param>
    /// <returns>如果能从文件名中猜测出 IETF 语言标签，则返回它，否则返回 null。</returns>
    private static string? GuessIetfLanguageTag(string fileNameWithoutExtension)
    {
        var parts = fileNameWithoutExtension.Split([' ', '.', '_', ',', ';'], StringSplitOptions.RemoveEmptyEntries).Reverse();
        return parts.FirstOrDefault(IsIetfLanguageTag)?.ToLowerInvariant();
    }

    /// <summary>
    /// 判断一个字符串是否是 IETF 语言标签。
    /// </summary>
    /// <param name="text">要判断的字符串。</param>
    /// <returns>如果是 IETF 语言标签，则返回 true，否则返回 false。</returns>
    private static bool IsIetfLanguageTag(string text)
    {
        try
        {
            var cultureInfo = new CultureInfo(text);
            return cultureInfo.Name.Equals(text, StringComparison.OrdinalIgnoreCase);
        }
        catch (CultureNotFoundException)
        {
            return false;
        }
    }

    /// <summary>
    /// 从增量源生成器的语法值提供器中挑选出所有的 <see cref="LocalizationGeneratingModel"/>。
    /// </summary>
    /// <param name="syntaxValueProvider">语法值提供器。</param>
    /// <returns>增量值提供器。</returns>
    public static IncrementalValuesProvider<LocalizationGeneratingModel> SelectGeneratingModels(this SyntaxValueProvider syntaxValueProvider) =>
        syntaxValueProvider.ForAttributeWithMetadataName(typeof(LocalizedConfigurationAttribute).FullName!, (node, ct) =>
        {
            if (node is not ClassDeclarationSyntax cds)
            {
                // 必须是类型。
                return false;
            }

            if (!cds.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                // 必须是分部类。
                return false;
            }

            return true;
        }, (c, ct) =>
        {
            var typeSymbol = c.TargetSymbol;
            var rootNamespace = typeSymbol.ContainingNamespace.ToDisplayString();
            var typeName = typeSymbol.Name;
            var attribute = typeSymbol.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass!.IsAttributeOf<LocalizedConfigurationAttribute>());
            var x = attribute!.ConstructorArguments.ToImmutableArray();
            var namedArguments = attribute!.NamedArguments.ToImmutableDictionary();
            var defaultLanguage = namedArguments.GetValueOrDefault(nameof(Localization.Default)).Value?.ToString()!;
            var currentLanguage = namedArguments.GetValueOrDefault(nameof(Localization.Current)).Value?.ToString()!;

            // 创建模型时，分析器确保了这些值不为空。
            return new LocalizationGeneratingModel(rootNamespace, typeName, defaultLanguage, currentLanguage);
        });
}
