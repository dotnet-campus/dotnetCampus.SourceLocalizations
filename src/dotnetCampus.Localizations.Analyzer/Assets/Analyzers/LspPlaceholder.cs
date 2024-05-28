global using dotnetCampus.Localizations.Assets.Analyzers;
using dotnetCampus.Localizations.Assets.Templates;

namespace dotnetCampus.Localizations.Assets.Analyzers;

/// <summary>
/// 为生成 <see cref="Localization"/> 的分部类的部分待填充代码提供占位符。
/// </summary>
/// <param name="ietfLanguageTag"></param>
/// <param name="fallback"></param>
public class LspPlaceholder(string ietfLanguageTag, ILocalized_Root? fallback) : ILocalized_Root
{
    /// <inheritdoc />
    public string IetfLanguageTag => ietfLanguageTag;

    /// <inheritdoc />
    public string this[string key] => fallback![key];
}
