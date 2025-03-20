using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Localizations.Generators;

[Generator]
public class PostInitializationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(Execute);
    }

    private void Execute(IncrementalGeneratorPostInitializationContext context)
    {
        var code = CollectPostInitializationCode();
        context.AddSource("ILocalizedValues.g.cs", SourceText.From(code, Encoding.UTF8));
    }

    private string CollectPostInitializationCode()
    {
        return $"""
namespace {GeneratorInfo.RootNamespace};

// Provide the following empty partial types so that the localization mechanism can work normally even when there are no language items.
// 提供以下空的分部类型，以便在没有任何语言项时，本地化机制也可正常工作。
// 提供以下空的分部类型，以便在沒有任何語言項時，本地化機制也可正常工作。
// 以下の空の部分型を提供することで、言語項目がない場合でもローカリゼーションメカニズムが正常に機能します。
// 다음의 빈 부분 유형을 제공하여 언어 항목이 없더라도 로컬리제이션 메커니즘이 정상적으로 작동합니다.

partial interface ILocalizedValues;
sealed partial class LocalizedValues : ILocalizedValues;

""";
    }
}
