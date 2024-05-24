using System.Collections.Frozen;
using System.ComponentModel;
using dotnetCampus.Localizations;

namespace LocalizationSample;

internal class Program
{
    public static void Main(string[] args)
    {
    }
}

[LocalizedConfiguration(Default = "zh-hans", Current = "en")]
internal partial class Lang;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface ILocalized_Root : ILocalizedStringProvider
{
    ILocalized_Root_A A => (ILocalized_Root_A)this;
}

[EditorBrowsable(EditorBrowsableState.Never)]
public interface ILocalized_Root_A : ILocalizedStringProvider
{
    LocalizedString A1 => this.Get0("A.A1");

    LocalizedString<int> A2 => this.Get1<int>("A.A2");

    LocalizedString<object> A3 => this.Get1<object>("A.A3");
}

public class Lang_ZhHans(ILocalized_Root? fallback) : ILocalized_Root,
    ILocalized_Root_A
{
    private readonly FrozenDictionary<string, string> _strings = new Dictionary<string, string>
    {
        { "A.A1", "文字" },
        { "A.A2", "错误码：{0}" },
        { "A.A3", "错误：{0}" },
    }.ToFrozenDictionary();

    public string this[string key] => _strings[key] ?? fallback![key];

    public string IetfLanguageTag => "zh-hans";
}

public class Lang_En(ILocalized_Root? fallback) : ILocalized_Root,
    ILocalized_Root_A
{
    private readonly FrozenDictionary<string, string> _strings = new Dictionary<string, string>
    {
        { "A.A1", "Words" },
        { "A.A2", "Error code: {0}" },
        { "A.A3", "Error: {0}" },
    }.ToFrozenDictionary();

    public string this[string key] => _strings[key] ?? fallback![key];

    public string IetfLanguageTag => "en";
}
