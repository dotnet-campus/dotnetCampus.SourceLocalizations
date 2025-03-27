#nullable enable
using global::dotnetCampus.Localizations;

namespace dotnetCampus.Localizations.Assets.Templates;

partial class Localization
{
    private static PlaceholderLocalizedValues _default = new PlaceholderLocalizedValues(null!);
    private static PlaceholderLocalizedValues _current = new PlaceholderLocalizedValues(null!);

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

    /// <summary>
    /// 设置当前的本地化字符串集。
    /// </summary>
    /// <param name="languageTag">要设置的语言标签（推荐 IETF 语言标签）。</param>
    public static void SetCurrent(string languageTag)
    {
        // <FLAG>

        // 无通知型。
        _current = (ImmutableLocalizedValues)Create(languageTag);

        // 有通知型。
        NotificationLocalizedValues notificationCurrent = _current;
        notificationCurrent.SetProvider(CreateLocalizedStringProvider(languageTag));

        // </FLAG>
    }

    /// <summary>
    /// 创建指定语言标签的本地化字符串集。（如果刚好是默认或当前的语言标签，则直接返回默认或当前的本地化字符串集的实例。）
    /// </summary>
    /// <param name="languageTag">语言标签（推荐 IETF 语言标签）。</param>
    /// <returns></returns>
    public static ILocalizedValues Create(string languageTag) => CreateLocalizedValues(languageTag);

    /// <summary>
    /// 创建指定语言标签的本地化字符串集。（如果刚好是默认或当前的语言标签，则直接返回默认或当前的本地化字符串集的实例。）
    /// </summary>
    /// <param name="languageTag">语言标签（推荐 IETF 语言标签）。</param>
    /// <returns></returns>
    public static PlaceholderLocalizedValues CreateLocalizedValues(string languageTag)
    {
        if (_default is { } @default && languageTag.Equals("DEFAULT_IETF_LANGUAGE_TAG", StringComparison.OrdinalIgnoreCase))
        {
            return @default;
        }
        if (_current is { } current && languageTag.Equals(current.LocalizedStringProvider.IetfLanguageTag, StringComparison.OrdinalIgnoreCase))
        {
            return @current;
        }
        return new PlaceholderLocalizedValues(CreateLocalizedStringProvider(languageTag));
    }

    /// <summary>
    /// 创建指定语言标签的本地化字符串集。
    /// </summary>
    /// <param name="languageTag">语言标签（推荐 IETF 语言标签）。</param>
    private static ILocalizedStringProvider CreateLocalizedStringProvider(string languageTag)
    {
        var lowerTag = languageTag.ToLowerInvariant();
        if (_default is { } @default && lowerTag == "DEFAULT_IETF_LANGUAGE_TAG")
        {
            return @default.LocalizedStringProvider;
        }
        if (_current is { } current && languageTag.Equals(current.LocalizedStringProvider.IetfLanguageTag, StringComparison.OrdinalIgnoreCase))
        {
            return current.LocalizedStringProvider;
        }
        return lowerTag switch
        {
            // <FLAG>
            "en" => null!,
            // </FLAG>
            _ => throw new global::System.ArgumentException($"The language tag {languageTag} is not supported.", nameof(languageTag)),
        };
    }
}
