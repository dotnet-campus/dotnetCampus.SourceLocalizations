#nullable enable

using global::System.Collections.Frozen;

namespace dotnetCampus.Localizations.Assets.Templates;

/// <inheritdoc />
[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public class LocalizationValues(ILocalizedValues? fallback) : ILocalizedValues
{
    /// <inheritdoc />
    public string IetfLanguageTag => "default";

    /// <inheritdoc />
    public string this[string key] => _strings[key] ?? fallback![key];

    private readonly FrozenDictionary<string, string> _strings = new Dictionary<string, string>
    {
        // <FLAG>
        { "A.A1", "文字" },
        { "A.A2", "错误码：{0}" },
        { "A.A3", "错误：{0}" },
        // </FLAG>
    }.ToFrozenDictionary();
}
