using System.Collections.Immutable;
using dotnetCampus.Localizations.Assets.Templates;
using dotnetCampus.Localizations.Generators.ModelProviding;
using dotnetCampus.Localizations.Utils.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static dotnetCampus.Localizations.Generators.ModelProviding.IetfLanguageTagExtensions;

namespace dotnetCampus.Localizations.Generators.CodeTransforming;

/// <summary>
/// 提供一个与语言文件格式无关的本地化项到 C# 代码的转换器。
/// </summary>
public class LocalizationCodeTransformer
{
    /// <summary>
    /// 获取所有的本地化项。
    /// </summary>
    public ImmutableArray<LocalizationItem> LocalizationItems { get; }

    /// <summary>
    /// 以树形式表示的所有本地化项。
    /// </summary>
    /// <remarks>
    /// 这个属性所表示的节点是根节点，其键和值都是 null，但是其子节点包含了所有的本地化项。
    /// </remarks>
    private LocalizationTreeNode Tree { get; }

    /// <summary>
    /// 创建 <see cref="LocalizationCodeTransformer"/> 的新实例。
    /// </summary>
    /// <param name="fileModels">读取出来的所有语言项。</param>
    public LocalizationCodeTransformer(ImmutableArray<LocalizationFileModel> fileModels)
    {
        LocalizationItems =
        [
            ..fileModels
                .Select(x => (Content: x.Content, Reader: (x.FileFormat switch
                {
                    "toml" => (ILocalizationFileReader)new TomlLocalizationFileReader(),
                    "yaml" => (ILocalizationFileReader)new YamlLocalizationFileReader(),
                    _ => throw new NotSupportedException($"Unsupported localization file format: {x.FileFormat}"),
                })))
                .SelectMany(x => x.Reader.Read(x.Content))
        ];
        Tree = LocalizationTreeNode.FromList(LocalizationItems);
    }

    #region Language Key Interfaces

    public string ToInterfaceCodeText() => $"""
#nullable enable

using global::dotnetCampus.Localizations;

using ILocalizedStringProvider = global::dotnetCampus.Localizations.ILocalizedStringProvider;
using LocalizedString = global::dotnetCampus.Localizations.LocalizedString;

namespace {GeneratorInfo.RootNamespace};
{RecursiveConvertLocalizationTreeNodeToKeyInterfaceCode(Tree, 0)}
""";

    private string RecursiveConvertLocalizationTreeNodeToKeyInterfaceCode(LocalizationTreeNode node, int depth)
    {
        if (node.Children.Count is 0)
        {
            return "";
        }

        var nodeTypeName = depth is 0
            ? ""
            : "_" + string.Join("_", node.FullIdentifierKey);
        var propertyLines = node.Children.Select(x =>
        {
            var identifierKey = string.Join("_", x.FullIdentifierKey);
            if (x.Children.Count is 0)
            {
                if (x.Item.ValueArgumentTypes.Length is 0)
                {
                    return $$"""
    /// <summary>
    /// {{ConvertValueToComment(x.Item.SampleValue)}}
    /// </summary>
    LocalizedString {{x.IdentifierKey}} { get; }
""";
                }
                else
                {
                    var genericTypes = string.Join(", ", x.Item.ValueArgumentTypes);
                    return $$"""
    /// <summary>
    /// {{ConvertValueToComment(x.Item.SampleValue)}}
    /// </summary>
    LocalizedString<{{genericTypes}}> {{x.IdentifierKey}} { get; }
""";
                }
            }
            else
            {
                return $$"""
    ILocalizedValues_{{identifierKey}} {{x.IdentifierKey}} { get; }
""";
            }
        });
        return $$"""

[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
public{{(depth is 0 ? " partial" : "")}} interface ILocalizedValues{{nodeTypeName}}
{
{{string.Join("\n\n", propertyLines)}}
}
{{string.Concat(node.Children.Select(x => RecursiveConvertLocalizationTreeNodeToKeyInterfaceCode(x, depth + 1)))}}
""";
    }

    private string ConvertValueToComment(string? value)
    {
        if (string.IsNullOrEmpty(value) || !value.Contains('\n'))
        {
            return value ?? "";
        }

        var lines = value!.Replace("\r", "").Split('\n');
        return string.Join("<br/>\n    /// ", lines);
    }

    #endregion

    #region Language Value Implementations

    public string ToImplementationCodeText(string typeName) => $"""
#nullable enable

using global::dotnetCampus.Localizations;

using ILocalizedStringProvider = global::dotnetCampus.Localizations.ILocalizedStringProvider;
using LocalizedString = global::dotnetCampus.Localizations.LocalizedString;

namespace {GeneratorInfo.RootNamespace};
{RecursiveConvertLocalizationTreeNodeToKeyImplementationCode(Tree, 0, typeName)}
""";

    private string RecursiveConvertLocalizationTreeNodeToKeyImplementationCode(LocalizationTreeNode node, int depth, string typeName)
    {
        if (node.Children.Count is 0)
        {
            return "";
        }

        var nodeKeyName = depth is 0
            ? ""
            : "." + string.Join("_", node.IdentifierKey);
        var nodeTypeName = depth is 0
            ? ""
            : "_" + string.Join("_", node.FullIdentifierKey);
        var propertyLines = node.Children.Select(x =>
        {
            var identifierKey = string.Join("_", x.FullIdentifierKey);
            if (x.Children.Count is 0)
            {
                if (x.Item.ValueArgumentTypes.Length is 0)
                {
                    return $"""
    /// <inheritdoc />
    public LocalizedString {x.IdentifierKey} => provider.Get0("{x.Item.Key}");
""";
                }
                else
                {
                    var genericTypes = string.Join(", ", x.Item.ValueArgumentTypes);
                    return $"""
    /// <inheritdoc />
    public LocalizedString<{genericTypes}> {x.IdentifierKey} => provider.Get{x.Item.ValueArgumentTypes.Length}<{genericTypes}>("{x.Item.Key}");
""";
                }
            }
            else
            {
                return $$"""
    public ILocalizedValues_{{identifierKey}} {{x.IdentifierKey}} { get; } = new LocalizedValues_{{identifierKey}}(provider);
""";
            }
        });
        return $$"""

[global::System.Diagnostics.DebuggerDisplay("[{LocalizedStringProvider.IetfLanguageTag}] {{typeName}}{{nodeKeyName}}.???")]
[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
internal sealed partial class LocalizedValues{{nodeTypeName}}(ILocalizedStringProvider provider) : ILocalizedValues{{nodeTypeName}}
{
    /// <summary>
    /// 获取本地化字符串提供器。
    /// </summary>
    public ILocalizedStringProvider LocalizedStringProvider => provider;

{{string.Join("\n\n", propertyLines)}}

    /// <summary>
    /// 获取非完整本地化字符串键的字符串表示。
    /// </summary>
    public override string ToString() => "{{typeName}}{{nodeKeyName}}.";
}
{{string.Concat(node.Children.Select(x => RecursiveConvertLocalizationTreeNodeToKeyImplementationCode(x, depth + 1, typeName)))}}
""";
    }

    #endregion

    #region Language Value Provider

    public string ToProviderCodeText(string rootNamespace, string ietfLanguageTag)
    {
        var typeName = IetfLanguageTagToIdentifier(ietfLanguageTag);
        var template = GeneratorInfo.GetEmbeddedTemplateFile<LocalizedStringProvider>();
        var code = template.Content
            .Replace($"namespace {template.Namespace};", $"namespace {GeneratorInfo.RootNamespace};")
            .Replace($"class {nameof(LocalizedStringProvider)}", $"class {nameof(LocalizedStringProvider)}_{typeName}")
            .Replace("""IetfLanguageTag => "default";""", $"""IetfLanguageTag => "{ietfLanguageTag}";""");
        var dictLines = LocalizationItems.Select(x => ConvertKeyValueToValueCodeLine(x.Key, x.Value));
        code = TemplateRegexes.FlagRegex.Replace(code, string.Concat(dictLines));
        return code;
    }

    private string ConvertKeyValueToValueCodeLine(string key, string value)
    {
        var escapedValue = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value)).ToFullString();
        return $"\n        {{ \"{key}\", {escapedValue} }},";
    }

    private string ConvertKeyValueToProperty(string key, string value)
    {
        return "";
    }

    private IEnumerable<string> EnumerateConvertTreeNodeToInterfaceNames(IEnumerable<LocalizationTreeNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.Children.Count is 0)
            {
                // 叶子节点，不提供接口。
            }
            else
            {
                // 非叶子节点，提供接口。
                yield return string.Join("_", node.FullIdentifierKey);
                foreach (var child in EnumerateConvertTreeNodeToInterfaceNames(node.Children))
                {
                    yield return child;
                }
            }
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// 格式无关的本地化项树节点。
    /// </summary>
    /// <param name="item">本地化项。</param>
    /// <param name="identifierKey">适用于 C# 标识符的当前节点的键。</param>
    /// <param name="fullIdentifierKey">适用于 C# 标识符的当前节点的完整键（包含从根到此节点的完整键路径，以“_”分隔）。</param>
    private class LocalizationTreeNode(LocalizationItem item, string identifierKey, string fullIdentifierKey)
    {
        /// <summary>
        /// 本地化项。
        /// </summary>
        public LocalizationItem Item => item;

        /// <summary>
        /// 适用于 C# 标识符的当前节点的键。
        /// </summary>
        public string IdentifierKey => identifierKey;

        /// <summary>
        /// 适用于 C# 标识符的当前节点的完整键（包含从根到此节点的完整键路径，以“_”分隔）。
        /// </summary>
        public string FullIdentifierKey => fullIdentifierKey;

        /// <summary>
        /// 子节点。
        /// </summary>
        public List<LocalizationTreeNode> Children { get; } = [];

        /// <summary>
        /// 寻找本地化项在树中的节点，如果不存在则创建。
        /// </summary>
        /// <param name="localizationItem">本地化项。</param>
        /// <returns>本地化项在树中的节点。</returns>
        public LocalizationTreeNode GetOrCreateDescendant(LocalizationItem localizationItem)
        {
            var parts = localizationItem.Key.Split(['.'], StringSplitOptions.RemoveEmptyEntries);
            var current = this;
            for (var i = 0; i < parts.Length; i++)
            {
                var part = parts[i];
                var child = current.Children.FirstOrDefault(x => x.IdentifierKey == part);
                if (child is null)
                {
                    child = new LocalizationTreeNode(localizationItem, part, string.Join("_", parts.Take(i + 1)));
                    current.Children.Add(child);
                }
                current = child;
            }
            return current;
        }

        /// <summary>
        /// 从本地化项列表创建树。
        /// </summary>
        /// <param name="localizationItemList">本地化项列表。</param>
        /// <returns>树的根节点。</returns>
        public static LocalizationTreeNode FromList(IReadOnlyList<LocalizationItem> localizationItemList)
        {
            var root = new LocalizationTreeNode(default, default!, default!);
            foreach (var item in localizationItemList)
            {
                var keyParts = item.Key.Split(['.'], StringSplitOptions.RemoveEmptyEntries);

                if (keyParts.Length is 0)
                {
                    continue;
                }

                if (keyParts.Length is 1)
                {
                    root.Children.Add(new LocalizationTreeNode(item, item.Key, item.Key));
                    continue;
                }

                _ = root.GetOrCreateDescendant(item);
            }
            return root;
        }
    }

    #endregion
}
