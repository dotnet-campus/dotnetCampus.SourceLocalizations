namespace dotnetCampus.Localizations;

/// <summary>
/// 在一个分部静态类上标记，可以为此静态类生成本地化语言项。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class LocalizedConfigurationAttribute : Attribute
{
    /// <summary>
    /// 指定默认语言。当任何一个语言项未找到时，将使用此语言项。
    /// </summary>
    public required string Default { get; init; }

    /// <summary>
    /// 指定开发时所用的当前语言。可以在语言项的文档注释中查看此语言项的文本。
    /// </summary>
    public required string Current { get; init; }
}
