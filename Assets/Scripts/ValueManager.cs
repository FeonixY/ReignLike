using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理玩家所有属性值的单例类，负责属性的初始化、变更、等级计算及前置条件判断。
/// </summary>
public class ValueManager : SingletonMonoBehaviour<ValueManager>
{
    /// <summary>
    /// 玩家当前所有属性的数值（0~100）。
    /// </summary>
    [ReadOnly]
    public Dictionary<ValueType, int> PlayerValues = new();

    /// <summary>
    /// 初始化所有属性为50，并同步UI。
    /// </summary>
    private void Start()
    {
        foreach (ValueType vt in System.Enum.GetValues(typeof(ValueType)))
        {
            PlayerValues[vt] = 50;
            ValueUIManager.Instance.UpdateValue(vt);
        }
    }

    /// <summary>
    /// 尝试更新指定属性的数值，并同步UI。
    /// </summary>
    /// <param name="valueType">要更新的属性类型</param>
    /// <param name="change">变化量（可正可负）</param>
    /// <param name="isTooHigh">是否达到上限（100）</param>
    /// <returns>属性是否达到上下限（0或100）</returns>
    public bool TryUpdateValue(ValueType valueType, int change, out bool isTooHigh)
    {
        if (PlayerValues.ContainsKey(valueType))
        {
            PlayerValues[valueType] += change;

            // 限制数值在0~100区间
            if (PlayerValues[valueType] < 0)
            {
                PlayerValues[valueType] = 0;
            }
            else if (PlayerValues[valueType] > 100)
            {
                PlayerValues[valueType] = 100;
            }

            Debug.Log($"Updated {valueType}: {PlayerValues[valueType]}");
            ValueUIManager.Instance.UpdateValue(valueType);
            isTooHigh = PlayerValues[valueType] == 100;
        }
        else
        {
            isTooHigh = true;
        }

        // 返回是否达到上下限
        return PlayerValues.TryGetValue(valueType, out int v) && (v == 0 || v == 100);
    }

    /// <summary>
    /// 生成用于结局展示的属性等级字典。
    /// </summary>
    /// <returns>每个属性的等级（1~4）</returns>
    public Dictionary<ValueType, int> PrepareShowNormalEnding()
    {
        var result = new Dictionary<ValueType, int>(PlayerValues.Count);
        foreach (var pair in PlayerValues)
        {
            result.Add(pair.Key, GetValueLevel(pair.Key));
        }
        return result;
    }

    /// <summary>
    /// 判断某个数值前置条件是否满足。
    /// </summary>
    /// <param name="pre">数值前置条件对象</param>
    /// <returns>是否满足条件</returns>
    public bool IsValuePrerequisiteSatisfied(ValuePrerequisite pre)
    {
        if (PlayerValues.TryGetValue(pre.ValueType, out int value))
        {
            return pre.IsSatisfied(value);
        }
        return false;
    }

    /// <summary>
    /// 获取指定属性当前等级（1~4）。
    /// 1：75~99，2：50~74，3：25~49，4：1~24
    /// </summary>
    /// <param name="valueType">属性类型</param>
    /// <returns>等级（1~4），无效返回-1</returns>
    private int GetValueLevel(ValueType valueType)
    {
        if (!PlayerValues.TryGetValue(valueType, out int value))
            return -1;

        if (value >= 75 && value < 100)
            return 1;
        else if (value >= 50 && value < 75)
            return 2;
        else if (value >= 25 && value < 50)
            return 3;
        else if (value > 0 && value < 25)
            return 4;
        else
            return -1;
    }
}
