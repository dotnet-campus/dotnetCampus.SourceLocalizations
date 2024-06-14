using System.Collections.Immutable;
using System.Globalization;
using dotnetCampus.Localizations.Ietf;

namespace dotnetCampus.Localizations.Utils;

internal static class LanguageSubtagRegistryParser
{
    public static ImmutableArray<LanguageSubtagItem> ParseRegistryFile(string text)
    {
        var items = new List<LanguageSubtagItem>();
        string? type = default;
        string? subtag = default;
        string? tag = default;
        string? description = default;
        string? added = default;
        string? deprecated = default;
        string? preferredValue = default;
        string? prefix = default;
        string? suppressScript = default;
        string? macrolanguage = default;
        string? scope = default;
        string? comment = default;

        ref string? lastField = ref type;

        var reader = new StringReader(text);
        var line = reader.ReadLine();
        while (true)
        {
            if (line is null || line.StartsWith("%%"))
            {
                // Add last item.
                if (type is not null)
                {
                    var addedDate = DateTimeOffset.Parse(added, CultureInfo.InvariantCulture);
                    var deprecatedDate = deprecated is null ? default : DateTimeOffset.Parse(deprecated, CultureInfo.InvariantCulture);
                    if (addedDate > deprecatedDate)
                    {
                        // Only add non-deprecated items.
                        items.Add(new LanguageSubtagItem
                        {
                            Type = type,
                            Subtag = subtag,
                            Tag = tag,
                            Description = description,
                            Added = DateTimeOffset.Parse(added, CultureInfo.InvariantCulture),
                            PreferredValue = preferredValue,
                            Prefix = prefix,
                            SuppressScript = suppressScript,
                            Macrolanguage = macrolanguage,
                            Scope = scope,
                            Comment = comment,
                        });
                    }

                    type = subtag = description = added = preferredValue = prefix = suppressScript = macrolanguage = scope = comment = default;
                }

                if (line is null)
                {
                    break;
                }

                // Skip comments.
                line = reader.ReadLine();
                continue;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                // Skip empty lines.
                continue;
            }

            if (line.StartsWith("  "))
            {
                // This is a continuation line.
                lastField = $"{lastField} {line.Trim()}";
                line = reader.ReadLine();
                continue;
            }

            var separatorIndex = line.IndexOf(':');
            if (separatorIndex is -1)
            {
                throw new FormatException($"No separator found in line: {line}");
            }

            var key = line.Substring(0, separatorIndex).Trim();
            var value = line.Substring(separatorIndex + 1).Trim();

            switch (key)
            {
                case "File-Date":
                    // This is the first line.
                    line = reader.ReadLine();
                    continue;
                case "Type":
                    type = value;
                    lastField = ref type;
                    break;
                case "Subtag":
                    subtag = value;
                    lastField = ref subtag;
                    break;
                case "Tag":
                    tag = value;
                    lastField = ref tag;
                    break;
                case "Description":
                    description = value;
                    lastField = ref description;
                    break;
                case "Added":
                    added = value;
                    lastField = ref added;
                    break;
                case "Deprecated":
                    deprecated = value;
                    lastField = ref deprecated;
                    break;
                case "Preferred-Value":
                    preferredValue = value;
                    lastField = ref preferredValue;
                    break;
                case "Prefix":
                    prefix = value;
                    lastField = ref prefix;
                    break;
                case "Suppress-Script":
                    suppressScript = value;
                    lastField = ref suppressScript;
                    break;
                case "Macrolanguage":
                    macrolanguage = value;
                    lastField = ref macrolanguage;
                    break;
                case "Scope":
                    scope = value;
                    lastField = ref scope;
                    break;
                case "Comments":
                    comment = value;
                    lastField = ref comment;
                    break;
                default:
                    throw new FormatException($"Unknown key: {key}");
            }

            line = reader.ReadLine();
        }

        return [..items];
    }
}
