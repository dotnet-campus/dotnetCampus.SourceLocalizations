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
    /// 获取支持的语言标签。
    /// </summary>
    /// <remarks>
    /// 由于项目中可以设置 LocalizationSupportsNonIetfLanguageTag 属性，所以不一定是 IETF 语言标签。
    /// </remarks>
    public static System.Collections.Immutable.ImmutableArray<string> SupportedLanguageTags { get; } =
    [
        // <FLAG2>
        "en",
        // </FLAG2>
    ];

    public static ILocalizedValues Create(string languageTag) => CreateLocalizedValues(languageTag);

    /// <summary>
    /// 设置当前的本地化字符串集。
    /// </summary>
    /// <param name="languageTag">要设置的语言标签（推荐 IETF 语言标签）。</param>
    public static void SetCurrent(string languageTag) => _current = CreateLocalizedValues(languageTag);

    /// <summary>
    /// 创建指定语言标签的本地化字符串集。
    /// </summary>
    /// <param name="languageTag">语言标签（推荐 IETF 语言标签）。</param>
    private static LocalizedValues CreateLocalizedValues(string languageTag)
    {
        var lowerTag = languageTag.ToLowerInvariant();
        if (_default is { } @default && lowerTag == "DEFAULT_IETF_LANGUAGE_TAG")
        {
            return @default;
        }
        return lowerTag switch
        {
            // <FLAG>
            "en" => new LocalizedValues(null!),
            // </FLAG>
            _ => throw new global::System.ArgumentException($"The language tag {languageTag} is not supported.", nameof(languageTag)),
        };
    }
}
