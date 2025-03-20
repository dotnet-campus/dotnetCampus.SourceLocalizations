using System.Collections.Immutable;
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
                var ietfLanguageTag = IetfLanguageTagExtensions.GuessIetfLanguageTagFromFileName(Path.GetFileNameWithoutExtension(x.Path));
                var extension = Path.GetExtension(x.Path) switch
                {
                    ".toml" => "toml",
                    ".yaml" or ".yml" => "yaml",
                    _ => throw new NotSupportedException($"Unsupported localization file format: {x.Path}"),
                };
                var text = x.GetText(ct)!.ToString();
                return new LocalizationFileModel(extension, ietfLanguageTag, text);
            });

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
