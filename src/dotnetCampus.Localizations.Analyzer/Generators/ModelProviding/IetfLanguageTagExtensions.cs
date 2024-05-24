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
}
