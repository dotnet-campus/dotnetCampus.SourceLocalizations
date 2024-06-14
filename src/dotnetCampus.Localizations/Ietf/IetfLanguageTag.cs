using System.Globalization;

namespace dotnetCampus.Localizations.Ietf;

/// <summary>
/// 表示符合 IETF BCP 47 标准的语言标签。
/// </summary>
/// <remarks>
/// 关于 IETF BCP 47 标准的语言标签，请参见：https://en.wikipedia.org/wiki/IETF_language_tag
/// <para/>
/// 数据由 INNA 提供，参见：https://www.iana.org/assignments/language-subtag-registry/language-subtag-registry
/// </remarks>
public readonly partial record struct IetfLanguageTag
{
    private readonly string _ietfLanguageTag;

    /// <summary>
    /// 初始化 <see cref="IetfLanguageTag"/> 类的新实例。
    /// </summary>
    /// <param name="ietfLanguageTag">符合 IETF BCP 47 标准的语言标签。</param>
    public IetfLanguageTag(string ietfLanguageTag)
    {
        _ietfLanguageTag = ietfLanguageTag;
    }

    /// <summary>
    /// 获取当前 IETF 语言标签的字符串表示形式。
    /// </summary>
    /// <returns>当前 IETF 语言标签的字符串表示形式。</returns>
    public override string ToString() => _ietfLanguageTag;

    /// <summary>
    /// 获取当前 IETF 语言标签的字符串表示形式。
    /// </summary>
    /// <param name="ietfLanguageTag">IETF 语言标签。</param>
    /// <returns></returns>
    public static implicit operator string(IetfLanguageTag ietfLanguageTag) => ietfLanguageTag._ietfLanguageTag;

    /// <summary>
    /// 从字符串创建 <see cref="IetfLanguageTag"/> 实例。
    /// </summary>
    /// <param name="ietfLanguageTag">
    /// 符合 IETF BCP 47 标准的语言标签字符串。
    /// 如果字符串不符合 IETF BCP 47 标准，将会抛出 <see cref="ArgumentException"/> 异常。
    /// <para>特别的，对于不符合 IETF BCP 47 标准但在 Windows 系统中可用的区域名称（如 zh-CN、zh-HK、zh-TW 等），此方法会自动将其转换为 IETF 语言标签。</para>
    /// </param>
    /// <returns>IETF 语言标签。</returns>
    /// <exception cref="ArgumentException">字符串不符合 IETF BCP 47 标准。</exception>
    public static implicit operator IetfLanguageTag(string ietfLanguageTag) => FromString(ietfLanguageTag, true);

    /// <summary>
    /// 从字符串创建 <see cref="IetfLanguageTag"/> 实例。
    /// </summary>
    /// <param name="ietfLanguageTag">符合 IETF BCP 47 标准的语言标签。</param>
    /// <param name="autoFixWindowsLegacyCultureName">
    /// 在 Windows 上，<see cref="CultureInfo.CurrentUICulture"/> 获取区域名称使用的是过时的 GetUserPreferredUILanguages 函数，
    /// 此函数返回的区域名称是 Windows 自己定义的，不符合 IETF BCP 47 标准，例如 zh-CN、zh-HK、zh-TW 等。
    /// 此函数参见：https://learn.microsoft.com/en-us/windows/win32/api/winnls/nf-winnls-getuserpreferreduilanguages
    /// <para/>
    /// 所以当设置此参数为 <see langword="true"/> 时，将会自动修正 Windows 的区域名称，例如将 zh-CN 修正为 zh-hans-cn、zh-HK 修正为 zh-hant-hk 等。
    /// </param>
    /// <returns>IETF 语言标签。</returns>
    /// <exception cref="ArgumentException">字符串不符合 IETF BCP 47 标准。</exception>
    public static IetfLanguageTag FromString(string ietfLanguageTag, bool autoFixWindowsLegacyCultureName = true)
    {
        if (IetfLanguageTags.Set.Contains(ietfLanguageTag))
        {
            return new IetfLanguageTag(ietfLanguageTag);
        }

        if (autoFixWindowsLegacyCultureName)
        {
            var fixedIetfLanguageTag = FixWindowsLegacyCultureName(ietfLanguageTag);
            if (IetfLanguageTags.Set.Contains(fixedIetfLanguageTag))
            {
                return new IetfLanguageTag(fixedIetfLanguageTag);
            }
        }

        throw new ArgumentException($"The language tag '{ietfLanguageTag}' is not a valid IETF BCP 47 language tag.", nameof(ietfLanguageTag));
    }

    /// <summary>
    /// 修正 Windows 的区域名称。
    /// </summary>
    /// <param name="ietfLanguageTag"></param>
    /// <returns></returns>
    private static string FixWindowsLegacyCultureName(string ietfLanguageTag) => ietfLanguageTag switch
    {
        "zh-CN" => "zh-hans-cn",
        "zh-HK" => "zh-hant-hk",
        "zh-TW" => "zh-hant-tw",
        "zh-SG" => "zh-hans-sg",
        "zh-MO" => "zh-hant-mo",
        _ => ietfLanguageTag,
    };
}
