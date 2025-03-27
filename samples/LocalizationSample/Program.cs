using dotnetCampus.Localizations;

namespace LocalizationSample;

internal class Program
{
    public static void Main(string[] args)
    {
        var tags = Lang.SupportedLanguageTags;
        Console.WriteLine(string.Join(", ", tags));

        var a = Lang.Current.A.A2.ToString(1);
        Console.WriteLine(a);
    }
}

[LocalizedConfiguration(Default = "zh-Hans")]
internal partial class Lang;
