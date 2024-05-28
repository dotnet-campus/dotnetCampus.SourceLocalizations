namespace dotnetCampus.Localizations;

/// <summary>
/// 表示一个本地化字符串，可隐式转换为字符串。
/// </summary>
/// <param name="Key">多语言键。</param>
/// <param name="Value">多语言值。</param>
public readonly record struct LocalizedString(string Key, string Value)
{
    /// <summary>
    /// 隐式转换为字符串。
    /// </summary>
    /// <param name="localizedString">要转换的本地化字符串。</param>
    public static implicit operator string(LocalizedString localizedString) => localizedString.Value;

    /// <summary>
    /// 将本地化字符串转换为字符串。
    /// </summary>
    /// <returns>转换的字符串。</returns>
    public override string ToString() => Value;
}

/// <summary>
/// 表示一个带有一个参数的本地化字符串。
/// </summary>
/// <typeparam name="T1">参数类型。</typeparam>
/// <param name="Key">多语言键。</param>
/// <param name="Value">多语言值。</param>
public readonly record struct LocalizedString<T1>(string Key, string Value)
{
    /// <summary>
    /// 格式化本地化字符串。
    /// </summary>
    /// <param name="arg1">要格式化的参数。</param>
    /// <returns>格式化后的字符串。</returns>
    public string Format(T1 arg1) => string.Format(Value, arg1);
}

/// <summary>
/// 表示一个带有两个参数的本地化字符串。
/// </summary>
/// <typeparam name="T1">第一个参数类型。</typeparam>
/// <typeparam name="T2">第二个参数类型。</typeparam>
/// <param name="Key">多语言键。</param>
/// <param name="Value">多语言值。</param>
public readonly record struct LocalizedString<T1, T2>(string Key, string Value)
{
    /// <summary>
    /// 格式化本地化字符串。
    /// </summary>
    /// <param name="arg1">要格式化的参数。</param>
    /// <param name="arg2">要格式化的参数。</param>
    /// <returns>格式化后的字符串。</returns>
    public string Format(T1 arg1, T2 arg2) => string.Format(Value, arg1, arg2);
}

/// <summary>
/// 表示一个带有三个参数的本地化字符串。
/// </summary>
/// <typeparam name="T1">第一个参数类型。</typeparam>
/// <typeparam name="T2">第二个参数类型。</typeparam>
/// <typeparam name="T3">第三个参数类型。</typeparam>
/// <param name="Key">多语言键。</param>
/// <param name="Value">多语言值。</param>
public readonly record struct LocalizedString<T1, T2, T3>(string Key, string Value)
{
    /// <summary>
    /// 格式化本地化字符串。
    /// </summary>
    /// <param name="arg1">要格式化的参数。</param>
    /// <param name="arg2">要格式化的参数。</param>
    /// <param name="arg3">要格式化的参数。</param>
    /// <returns>格式化后的字符串。</returns>
    public string Format(T1 arg1, T2 arg2, T3 arg3) => string.Format(Value, arg1, arg2, arg3);
}

/// <summary>
/// 表示一个带有四个参数的本地化字符串。
/// </summary>
/// <typeparam name="T1">第一个参数类型。</typeparam>
/// <typeparam name="T2">第二个参数类型。</typeparam>
/// <typeparam name="T3">第三个参数类型。</typeparam>
/// <typeparam name="T4">第四个参数类型。</typeparam>
/// <param name="Key">多语言键。</param>
/// <param name="Value">多语言值。</param>
public readonly record struct LocalizedString<T1, T2, T3, T4>(string Key, string Value)
{
    /// <summary>
    /// 格式化本地化字符串。
    /// </summary>
    /// <param name="arg1">要格式化的参数。</param>
    /// <param name="arg2">要格式化的参数。</param>
    /// <param name="arg3">要格式化的参数。</param>
    /// <param name="arg4">要格式化的参数。</param>
    /// <returns>格式化后的字符串。</returns>
    public string Format(T1 arg1, T2 arg2, T3 arg3, T4 arg4) => string.Format(Value, arg1, arg2, arg3, arg4);
}
