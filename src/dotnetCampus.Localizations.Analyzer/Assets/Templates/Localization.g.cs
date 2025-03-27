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
        // <FLAG3>

        // 无通知型。
        _current = (ImmutableLocalizedValues)Create(languageTag);

        // 有通知型。
        NotificationLocalizedValues notificationCurrent = _current;
        notificationCurrent.SetProvider(GetOrCreateLocalizedStringProvider(languageTag));

        // </FLAG3>
    }

    /// <summary>
    /// 创建指定语言标签的本地化字符串集。（如果刚好是默认或当前的语言标签，则直接返回默认或当前的本地化字符串集的实例。）
    /// </summary>
    /// <param name="languageTag">语言标签（推荐 IETF 语言标签）。</param>
    /// <returns></returns>
    public static ILocalizedValues Create(string languageTag) => GetOrCreateLocalizedValues(languageTag);

    /// <summary>
    /// 创建指定语言标签的本地化字符串集。（如果刚好是默认或当前的语言标签，则直接返回默认或当前的本地化字符串集的实例。）
    /// </summary>
    /// <param name="languageTag">语言标签（推荐 IETF 语言标签）。</param>
    /// <returns></returns>
    private static PlaceholderLocalizedValues GetOrCreateLocalizedValues(string languageTag)
    {
        if (_default is { } @default && languageTag.Equals("DEFAULT_IETF_LANGUAGE_TAG", StringComparison.OrdinalIgnoreCase))
        {
            return @default;
        }
        if (_current is { } current && languageTag.Equals(current.LocalizedStringProvider.IetfLanguageTag, StringComparison.OrdinalIgnoreCase))
        {
            return @current;
        }
        return new PlaceholderLocalizedValues(Wrap(GetOrCreateLocalizedStringProvider(languageTag)));
    }

    /// <summary>
    /// 包装一个 <see cref="ILocalizedStringProvider"/> 对象，使其根据开发者设置的是否允许语言项变更通知返回不同的类型。
    /// </summary>
    /// <param name="rawProvider">原始的 <see cref="ILocalizedStringProvider"/> 对象。</param>
    /// <returns>包装后的 <see cref="ILocalizedStringProvider"/> 对象。</returns>
    private static ILocalizedStringProvider Wrap(ILocalizedStringProvider rawProvider) => rawProvider;

    /// <summary>
    /// 创建指定语言标签的本地化字符串集。
    /// </summary>
    /// <param name="languageTag">语言标签（推荐 IETF 语言标签）。</param>
    private static ILocalizedStringProvider GetOrCreateLocalizedStringProvider(string languageTag)
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
        var provider = CreateLocalizedStringProvider(languageTag);
        while (provider is null)
        {
            var parentTag = new global::System.Globalization.CultureInfo(languageTag).Parent.Name;
            if (string.IsNullOrWhiteSpace(parentTag))
            {
                break;
            }
            provider = CreateLocalizedStringProvider(parentTag);
        }
        if (provider is not null)
        {
            return provider;
        }
        throw new global::System.ArgumentException($"The language tag {languageTag} is not supported.", nameof(languageTag));
    }

    /// <summary>
    /// 创建指定语言标签的本地化字符串集。
    /// </summary>
    /// <param name="languageTag">语言标签（推荐 IETF 语言标签）。</param>
    private static ILocalizedStringProvider? CreateLocalizedStringProvider(string languageTag)
    {
        return languageTag.ToLowerInvariant() switch
        {
            // <FLAG>
            "en" => null,
            // </FLAG>
            _ => null,
        };
    }
}
