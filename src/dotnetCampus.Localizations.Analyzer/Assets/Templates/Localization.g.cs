#nullable enable

namespace dotnetCampus.Localizations.Assets.Templates;

partial class Localization
{
    /// <summary>
    /// 获取默认的本地化字符串集。
    /// </summary>
    public static ILocalizedValues Default { get; } = new LspPlaceholder("default", null);

    /// <summary>
    /// 获取当前的本地化字符串集。
    /// </summary>
    public static ILocalizedValues Current { get; private set; } = new LspPlaceholder("current", null);
}
