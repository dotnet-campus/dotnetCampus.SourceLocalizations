namespace dotnetCampus.Localizations.Ietf;

/// <summary>
/// 表示一个语言子标签。
/// </summary>
internal record LanguageSubtagItem
{
    public string? Type { get; init; }
    public string? Subtag { get; init; }
    public string? Tag { get; set; }
    public string? Description { get; init; }
    public DateTimeOffset Added { get; init; }
    public string? PreferredValue { get; init; }
    public string? Prefix { get; init; }
    public string? SuppressScript { get; init; }
    public string? Macrolanguage { get; init; }
    public string? Scope { get; init; }
    public string? Comment { get; init; }
}
