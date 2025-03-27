﻿using System.Collections.Immutable;
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
    public static IncrementalValuesProvider<LocalizationFileModel> SelectLocalizationFileModels(this IncrementalGeneratorInitializationContext context) =>
        context.AdditionalTextsProvider
            .Combine(context.AnalyzerConfigOptionsProvider)
            .Where(pair =>
                // 标记了 DotNetCampusLocalization 的文件才生成。
                (pair.Right.GetOptions(pair.Left).TryGetValue("build_metadata.AdditionalFiles.SourceItemGroup", out var t) && t.Equals("DotNetCampusLocalization", StringComparison.OrdinalIgnoreCase))
                // 目前只支持 toml 和 yaml 格式的文件。
                && (pair.Left.Path.EndsWith(".toml", StringComparison.OrdinalIgnoreCase)
                    || pair.Left.Path.EndsWith(".yaml", StringComparison.OrdinalIgnoreCase)
                    || pair.Left.Path.EndsWith(".yml", StringComparison.OrdinalIgnoreCase))
            )
            .Select((x, ct) => x.Left)
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
            var namedArguments = attribute!.NamedArguments.ToImmutableDictionary();
            var defaultLanguage = namedArguments.GetValueOrDefault(nameof(Localization.Default)).Value?.ToString()!;
            var currentLanguage = namedArguments.GetValueOrDefault(nameof(Localization.Current)).Value?.ToString();
            var defaultTagLocation = c.TargetNode.DescendantNodes()
                .OfType<AttributeSyntax>()
                .FirstOrDefault(x => x.Name.ToString() == nameof(Localization.Default))?.GetLocation()!;
            var currentTagLocation = c.TargetNode.DescendantNodes()
                .OfType<AttributeSyntax>()
                .FirstOrDefault(x => x.Name.ToString() == nameof(Localization.Current))?.GetLocation();

            // 创建模型时，分析器确保了这些值不为空。
            return new LocalizationGeneratingModel(rootNamespace, typeName, defaultLanguage, defaultTagLocation, currentLanguage, currentTagLocation);
        });
}
