using System.Globalization;

namespace dotnetCampus.Localizations;

/// <summary>
/// 提供符合 BCP-47 标准的区域性回退匹配服务。<br/>
/// 功能特性：
/// <list type="bullet">
/// <item>支持旧版文化代码自动转换（如zh-CHS→zh-Hans）</item>
/// <item>多级回退匹配（特定区域→中性文化→语言代码）</item>
/// <item>区域性变体自动扩展（zh→zh-Hans→zh-Hans-CN）</item>
/// </list>
/// </summary>
public static class LocalizationFallbackProvider
{
    private static readonly Dictionary<string, string> LegacyMappings = new(StringComparer.OrdinalIgnoreCase)
    {
        // 旧版代码映射
        ["zh-CHS"] = "zh-Hans",
        ["zh-CHT"] = "zh-Hant",
    };

    /// <summary>
    /// 查找最佳匹配的文化代码
    /// </summary>
    /// <param name="requestedIetfLanguageTag">请求的目标文化代码</param>
    /// <param name="availableIetfLanguageTags">可用的文化代码集合</param>
    /// <returns>匹配结果（未找到时返回null）</returns>
    public static string? FindBestMatch(string requestedIetfLanguageTag, IEnumerable<string> availableIetfLanguageTags)
    {
        // 没有任何候选时，返回 null。
        var languageTags = availableIetfLanguageTags.ToDictionary(x => x, x => x, StringComparer.OrdinalIgnoreCase);
        if (languageTags.Count is 0)
        {
            return null;
        }

        // 直接精确匹配（允许大小写不同）时，返回精确匹配的结果。
        if (languageTags.TryGetValue(requestedIetfLanguageTag, out var originalCasingValue))
        {
            return originalCasingValue;
        }

        // 标准化请求的文化代码。
        var normalized = NormalizeCultureCode(requestedIetfLanguageTag);
        if (string.IsNullOrEmpty(normalized))
        {
            return null;
        }

        // 尝试回退匹配。
        foreach (var candidate in GetFallbackCandidates(normalized))
        {
            if (languageTags.TryGetValue(candidate, out var originalCasingValue2))
            {
                // 返回列表中原始大小写的匹配项（而不是用户传入的大小写）。
                return originalCasingValue2;
            }
        }

        // 没有找到匹配项。
        return null;
    }

    /// <summary>
    /// 标准化文化代码处理。
    /// </summary>
    private static string NormalizeCultureCode(string cultureCode)
    {
        if (string.IsNullOrWhiteSpace(cultureCode))
        {
            return cultureCode;
        }

        // 处理已知映射。
        if (LegacyMappings.TryGetValue(cultureCode, out var mapped))
        {
            return mapped;
        }

        try
        {
            // 标准化有效文化代码。
            var culture = CultureInfo.GetCultureInfo(cultureCode);
            return culture.Name;
        }
        catch (CultureNotFoundException)
        {
            // 保持原始格式用于后续处理
            return cultureCode.Trim();
        }
    }

    private static IEnumerable<string> GetFallbackCandidates(string normalizedCode)
    {
        // 第一优先级：原始标准化代码。
        yield return normalizedCode;

        // 第二优先级：区域性父级链。
        var culture = GetCultureInfoSafe(normalizedCode);
        while (culture != null && !culture.Equals(CultureInfo.InvariantCulture))
        {
            // 中性文化的区域变体
            if (culture.IsNeutralCulture)
            {
                foreach (var variant in GetRegionalVariants(culture))
                {
                    yield return variant;
                }
            }

            // 父级文化代码
            if (!Equals(culture.Parent, CultureInfo.InvariantCulture))
            {
                yield return culture.Parent.Name;
            }

            culture = culture.Parent;
        }

        // 第三优先级：语言代码。
        if (normalizedCode.Length > 2)
        {
            yield return normalizedCode.Substring(0, 2);
        }
    }

    private static IEnumerable<string> GetRegionalVariants(CultureInfo neutralCulture)
    {
        // 预设的常用区域映射
        if (LegacyMappings.TryGetValue(neutralCulture.Name, out var defaultVariant))
        {
            yield return defaultVariant;
        }

        // 动态生成的区域变体
        foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
        {
            if (culture.Parent.Name.Equals(neutralCulture.Name, StringComparison.OrdinalIgnoreCase))
            {
                yield return culture.Name;
            }
        }
    }

    private static CultureInfo? GetCultureInfoSafe(string name)
    {
        try
        {
            return CultureInfo.GetCultureInfo(name);
        }
        catch
        {
            return null;
        }
    }
}
