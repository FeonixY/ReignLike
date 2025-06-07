using System.Collections.Generic;
using Sirenix.Serialization;

/// <summary>
/// 事件选项类，描述单个选项的文本及其对各属性的影响。
/// </summary>
public class EventOption
{
    /// <summary>
    /// 选项显示文本（如“同意”、“拒绝”等）。
    /// </summary>
    public string OptionText;

    /// <summary>
    /// 该选项对各属性（ValueType）的数值变化。
    /// Key：属性类型，Value：变化量（可正可负）。
    /// </summary>
    [OdinSerialize]
    public Dictionary<ValueType, int> ValueChanges = new();
}