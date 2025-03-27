using Microsoft.CodeAnalysis;

namespace dotnetCampus.Localizations.Generators.ModelProviding;

/// <summary>
/// 开发者在代码中指定应基于某个类型生成本地化文件时，此模型表示了开发者所指定的所有需要的生成参数。
/// </summary>
/// <param name="Namespace">类型的命名空间。</param>
/// <param name="TypeName">类型的名称。</param>
/// <param name="DefaultLanguage">默认语言的 IETF 语言标签。</param>
/// <param name="CurrentLanguage">当前语言的 IETF 语言标签。</param>
public readonly record struct LocalizationGeneratingModel(
    string Namespace, string TypeName,
    string DefaultLanguage, Location DefaultTagLocation,
    string? CurrentLanguage, Location? CurrentTagLocation);
