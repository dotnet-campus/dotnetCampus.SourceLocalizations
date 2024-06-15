#nullable enable

using ILocalizedStringProvider = global::dotnetCampus.Localizations.ILocalizedStringProvider;
using LocalizedString = global::dotnetCampus.Localizations.LocalizedString;

namespace dotnetCampus.Localizations.Assets.Templates;

[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public partial interface ILocalizedValues : ILocalizedStringProvider
{
    // <FLAG>
    // ILocalizedValues A => (ILocalizedValues_A)this;
    // LocalizedString A1 => this.Get0("A.A1");
    // </FLAG>
}
