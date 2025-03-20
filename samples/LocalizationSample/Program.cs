using dotnetCampus.Localizations;

namespace LocalizationSample;

internal class Program
{
    public static void Main(string[] args)
    {
        // var a = Lang.Current.A.A2.ToString(1);
        // Console.WriteLine(a);
    }
}

[LocalizedConfiguration(Default = "zh-hans", Current = "en")]
internal partial class Lang;
