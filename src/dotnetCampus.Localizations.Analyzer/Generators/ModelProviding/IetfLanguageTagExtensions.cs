using System.Collections.Immutable;

namespace dotnetCampus.Localizations.Generators.ModelProviding;

/// <summary>
/// IETF 语言标签扩展方法。
/// </summary>
public static class IetfLanguageTagExtensions
{
    /// <summary>
    /// 将 IETF 语言标签转换为适用于 C# 标识符的字符串。
    /// </summary>
    /// <param name="ietfLanguageTag">IETF 语言标签。</param>
    /// <returns>适用于 C# 标识符的字符串。</returns>
    public static string IetfLanguageTagToIdentifier(string ietfLanguageTag)
    {
        Span<char> identifier = stackalloc char[ietfLanguageTag.Length];
        var isPartStart = true;
        var identifierIndex = 0;

        foreach (var c in ietfLanguageTag)
        {
            if (c is '-')
            {
                isPartStart = true;
                continue;
            }

            if (isPartStart)
            {
                identifier[identifierIndex++] = char.ToUpperInvariant(c);
                isPartStart = false;
            }
            else
            {
                identifier[identifierIndex++] = char.ToLowerInvariant(c);
            }
        }

        return identifier.Slice(0, identifierIndex).ToString();
    }

    /// <summary>
    /// 将 <see cref="LocalizationFileModel"/> 按照 IETF 语言标签分组。
    /// </summary>
    /// <param name="models">要分组的 <see cref="LocalizationFileModel"/> 集合。</param>
    /// <returns>枚举的每一项都是一个元组，包含 IETF 语言标签和对应的 <see cref="LocalizationFileModel"/> 集合。</returns>
    public static IEnumerable<(string IetfLanguageTag, ImmutableArray<LocalizationFileModel> Models)> GroupByIetfLanguageTag(this IEnumerable<LocalizationFileModel> models)
    {
        var groups = new Dictionary<string, List<LocalizationFileModel>>(StringComparer.OrdinalIgnoreCase);

        foreach (var model in models)
        {
            if (!groups.TryGetValue(model.IetfLanguageTag, out var group))
            {
                group = [];
                groups[model.IetfLanguageTag] = group;
            }

            group.Add(model);
        }

        foreach (var group in groups.Values)
        {
            yield return (group[0].IetfLanguageTag, [..group]);
        }
    }
}
