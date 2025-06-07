using System;
using System.ComponentModel;

/// <summary>
/// 游戏中的属性类型枚举，并带有中文描述。
/// </summary>
public enum ValueType
{
    [Description("贵族")]
    Nobility,
    [Description("财富")]
    Wealth,
    [Description("信仰")]
    Faith,
    [Description("民心")]
    People
}

/// <summary>
/// ValueType 枚举的扩展方法。
/// </summary>
public static class ValueTypeExtensions
{
    /// <summary>
    /// 将字符串解析为 ValueType 枚举。
    /// 支持英文名（不区分大小写）和 DescriptionAttribute 标注的中文描述。
    /// </summary>
    /// <param name="value">要解析的字符串</param>
    /// <returns>对应的 ValueType 枚举值</returns>
    /// <exception cref="ArgumentException">无法解析时抛出</exception>
    public static ValueType Parse(string value)
    {
        // 先尝试用英文名解析（不区分大小写）
        if (Enum.TryParse<ValueType>(value, true, out var result))
            return result;

        // 再尝试用 DescriptionAttribute（如中文描述）解析
        foreach (ValueType vt in Enum.GetValues(typeof(ValueType)))
        {
            var field = typeof(ValueType).GetField(vt.ToString());
            var desc = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (desc != null && desc.Description == value)
                return vt;
        }

        // 都无法解析则抛出异常
        throw new ArgumentException($"无法将字符串“{value}”解析为 ValueType 枚举。");
    }
}