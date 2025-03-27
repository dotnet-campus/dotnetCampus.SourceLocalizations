global using dotnetCampus.Localizations.Assets.Sources;
using dotnetCampus.Localizations.Assets.Templates;

namespace dotnetCampus.Localizations.Assets.Sources;

internal sealed class PlaceholderLocalizedValues(ILocalizedStringProvider provider) : ILocalizedValues
{
    public ILocalizedStringProvider LocalizedStringProvider => provider;

    public static implicit operator ImmutableLocalizedValues(PlaceholderLocalizedValues value) => null!;
    public static implicit operator PlaceholderLocalizedValues(ImmutableLocalizedValues value) => null!;
    public static implicit operator NotificationLocalizedValues(PlaceholderLocalizedValues value) => null!;
}
