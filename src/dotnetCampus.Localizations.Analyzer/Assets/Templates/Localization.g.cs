#nullable enable

namespace dotnetCampus.Localizations.Assets.Templates;

partial class Localization
{
    private static LocalizedValues _default = new LocalizedValues(null!);
    private static LocalizedValues _current = new LocalizedValues(null!);

    /// <summary>
    /// 获取默认的本地化字符串集。
    /// </summary>
    public static ILocalizedValues Default => _default;

    /// <summary>
    /// 获取当前的本地化字符串集。
    /// </summary>
    public static ILocalizedValues Current => _current;

    /// <summary>
    /// 设置当前的本地化字符串集。
    /// </summary>
    /// <param name="ietfLanguageTag">要设置的 IETF 语言标签。</param>
    public static void SetCurrent(string ietfLanguageTag) => _current = CreateLocalizedValues(ietfLanguageTag);

    /// <summary>
    /// 创建指定 IETF 语言标签的本地化字符串集。
    /// </summary>
    /// <param name="ietfLanguageTag">IETF 语言标签。</param>
    private static LocalizedValues CreateLocalizedValues(string ietfLanguageTag)
    {
        var lowerTag = ietfLanguageTag.ToLowerInvariant();
        if (_default is { } @default && lowerTag == "DEFAULT_IETF_LANGUAGE_TAG")
        {
            return @default;
        }
        return lowerTag switch
        {
            // <FLAG>
            "en" => new LocalizedValues(null!),
            // </FLAG>
            _ => throw new global::System.ArgumentException($"The language tag {ietfLanguageTag} is not supported.", nameof(ietfLanguageTag)),
        };
    }
}
