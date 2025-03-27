#nullable enable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ILocalizedStringProvider = global::dotnetCampus.Localizations.ILocalizedStringProvider;
using INotifyPropertyChanged = global::System.ComponentModel.INotifyPropertyChanged;
using LocalizedString = global::dotnetCampus.Localizations.LocalizedString;
using PropertyChangedEventHandler = global::System.ComponentModel.PropertyChangedEventHandler;

namespace dotnetCampus.Localizations.Assets.Templates;

[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public sealed class NotificationLocalizedValues : INotifyPropertyChanged, ILocalizedValues
{
    private ILocalizedStringProvider _provider;

    /// <summary>
    /// 获取本地化字符串提供器。
    /// </summary>
    public required ILocalizedStringProvider LocalizedStringProvider
    {
        get => _provider;
        [MemberNotNull(nameof(_provider))]
        set
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var oldValue = _provider;
            var newValue = value;
            _provider = value;
            if (oldValue == newValue)
            {
                return;
            }

            // <FLAG2>
            // 在此处为所有的属性添加 PropertyChanged 事件。
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
            // </FLAG2>
        }
    }

    // <FLAG>
    // public LocalizedString A1 => provider.Get0("A.A1");
    // public LocalizedString<int> A2 => provider.Get1<int>("A.A2");
    // public ILocalizedValues_A_A3 A3 { get; } = new LocalizedValues_A_A3(provider);
    // </FLAG>

    public event PropertyChangedEventHandler? PropertyChanged;
}
