// ReSharper disable once CheckNamespace
namespace dotnetCampus.Localizations.Helpers;

public static class LocalizationHelper
{
    public static string? MatchWithFallback(string requestedIetfLanguageTag, IEnumerable<string> availableIetfLanguageTags)
    {
        return availableIetfLanguageTags.FirstOrDefault();
    }
}
