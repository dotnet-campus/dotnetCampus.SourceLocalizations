#nullable enable

using ILocalizedStringProvider = global::dotnetCampus.Localizations.ILocalizedStringProvider;
using LocalizedString = global::dotnetCampus.Localizations.LocalizedString;

namespace dotnetCampus.Localizations.Assets.Templates;

[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public interface ILocalized_Root : ILocalizedStringProvider
{
    // <FLAG>
    // ILocalized_Root_A A => (ILocalized_Root_A)this;
    // LocalizedString A1 => this.Get0("A.A1");
    // </FLAG>
}
