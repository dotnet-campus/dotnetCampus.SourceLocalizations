#nullable enable
using ILocalizedStringProvider = global::dotnetCampus.Localizations.ILocalizedStringProvider;
using INotifyPropertyChanged = global::System.ComponentModel.INotifyPropertyChanged;
using LocalizedString = global::dotnetCampus.Localizations.LocalizedString;
using PropertyChangedEventHandler = global::System.ComponentModel.PropertyChangedEventHandler;

namespace dotnetCampus.Localizations.Assets.Templates;

[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public sealed class ImmutableLocalizedValues(ILocalizedStringProvider provider) : ILocalizedValues
{
    /// <summary>
    /// 获取本地化字符串提供器。
    /// </summary>
    public ILocalizedStringProvider LocalizedStringProvider => provider;

    // <FLAG>
    // public LocalizedString A1 => provider.Get0("A.A1");
    // public LocalizedString<int> A2 => provider.Get1<int>("A.A2");
    // public ILocalizedValues_A_A3 A3 { get; } = new LocalizedValues_A_A3(provider);
    // </FLAG>
}
