namespace dotnetCampus.Localizations.Utils.IO;

/// <summary>
/// 嵌入的文本资源的数据。
/// </summary>
/// <param name="FileName">文件的名称（含扩展名）。</param>
/// <param name="TypeName">文件的名称（不含扩展名），或者也很可能是类型名称。</param>
/// <param name="Namespace">文件的命名空间。</param>
/// <param name="Content">文件的文本内容。</param>
internal readonly record struct EmbeddedSourceFile(
    string FileName,
    string TypeName,
    string Namespace,
    string Content);
