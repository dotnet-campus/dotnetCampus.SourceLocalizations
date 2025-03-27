#nullable enable
using global::System.Collections.Frozen;

namespace dotnetCampus.Localizations.Assets.Templates;

/// <inheritdoc />
[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public class LocalizedStringProvider(ILocalizedStringProvider? fallback) : ILocalizedStringProvider
{
    /// <inheritdoc />
    public string IetfLanguageTag => "default";

    /// <inheritdoc />
    public string this[string key]
    {
        get
        {
            if (_strings.TryGetValue(key,out var value) && value != null)
            {
                return value;
            }
            if (fallback != null)
            {
                return fallback[key];
            }
            return "";
        }
    }

    private readonly global::System.Collections.Frozen.FrozenDictionary<string, string> _strings = new global::System.Collections.Generic.Dictionary<string, string>
    {
        // <FLAG>
        { "A.A1", "文字" },
        { "A.A2", "错误码：{0}" },
        { "A.A3", "错误：{0}" },
        // </FLAG>
    }.ToFrozenDictionary();
}
