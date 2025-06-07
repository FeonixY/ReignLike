using System;
using Sirenix.OdinInspector;

/// <summary>
/// 用于描述属性值前置条件。
/// </summary>
[Serializable]
public class ValuePrerequisite
{
    [LabelText("数据类型")]
    public ValueType ValueType;

    [LabelText("比较方式")]
    public CompareType CompareType;

    [LabelText("目标数值")]
    public int TargetValue;

    public bool IsSatisfied(int currentValue)
    {
        return CompareType switch
        {
            CompareType.Greater => currentValue > TargetValue,
            CompareType.GreaterOrEqual => currentValue >= TargetValue,
            CompareType.Less => currentValue < TargetValue,
            CompareType.LessOrEqual => currentValue <= TargetValue,
            _ => false
        };
    }
}

public enum CompareType
{
    Greater,
    GreaterOrEqual,
    Less,
    LessOrEqual
}