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
public class LocalizationFilesGenerator : IIncrementalGenerator
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

        foreach (var file in localizationFiles)
        {
            var transformer = new LocalizationCodeTransformer(file.Content, file.FileFormat switch
            {
                "toml" => new TomlLocalizationFileReader(),
                "yaml" => new YamlLocalizationFileReader(),
                _ => throw new NotSupportedException($"Unsupported localization file format: {file.FileFormat}"),
            });

            var code = transformer.ToImplementationCodeText(options.Namespace, file.IetfLanguageTag);
            context.AddSource($"{nameof(LocalizationValues)}.{file.IetfLanguageTag}.g.cs", SourceText.From(code, Encoding.UTF8));

            if (file.IetfLanguageTag == options.DefaultLanguage)
            {
                var keyCode = transformer.ToInterfaceCodeText(options.Namespace);
                context.AddSource($"{nameof(ILocalized_Root)}.g.cs", SourceText.From(keyCode, Encoding.UTF8));
            }
        }
    }
}
