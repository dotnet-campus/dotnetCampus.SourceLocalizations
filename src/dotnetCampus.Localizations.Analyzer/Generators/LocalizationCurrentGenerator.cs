using System.Collections.Immutable;
using System.Text;
using dotnetCampus.Localizations.Assets.Templates;
using dotnetCampus.Localizations.Generators.ModelProviding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

using static dotnetCampus.Localizations.Generators.ModelProviding.IetfLanguageTagExtensions;

namespace dotnetCampus.Localizations.Generators;

/// <summary>
/// 为静态的本地化中心类生成设置当前语言的方法。
/// </summary>
[Generator]
public class LocalizationCurrentGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var localizationFilesProvider = context.AdditionalTextsProvider.SelectLocalizationFileModels();
        var localizationTypeprovider = context.SyntaxProvider.SelectGeneratingModels();
        context.RegisterSourceOutput(localizationTypeprovider.Combine(localizationFilesProvider.Collect()), Execute);
    }

    private void Execute(SourceProductionContext context, (LocalizationGeneratingModel Left, ImmutableArray<LocalizationFileModel> Right) modelTuple)
    {
        var ((typeNamespace, typeName, _, _), models) = modelTuple;

        var code = GenerateSetCurrentMethod(typeNamespace, typeName, models);

        context.AddSource($"{typeName}.g.cs", SourceText.From(code, Encoding.UTF8));
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
        return $"            \"{model.IetfLanguageTag}\" => new global::{typeNamespace}.Localizations.{nameof(LocalizationValues)}_{IetfLanguageTagToIdentifier(model.IetfLanguageTag)}(Default),";
    }
}
