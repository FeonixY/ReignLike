using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件数据对象，描述一个可触发的游戏事件及其前置条件、选项等。
/// </summary>
[CreateAssetMenu(fileName = "NewEvent", menuName = "Event")]
public class Event : SerializedScriptableObject
{
    /// <summary>
    /// 事件前置条件（需完成指定事件及选项）。
    /// </summary>
    public List<EventPrerequisite> EventPrerequisites = new();

    /// <summary>
    /// 属性值前置条件（需满足指定属性条件）。
    /// </summary>
    public List<ValuePrerequisite> ValuePrerequisites = new();

    /// <summary>
    /// 是否满足所有前置条件（事件和属性值）。
    /// </summary>
    public bool IsPrerequisitesSatisfied
    {
        get
        {
            // 检查所有事件前置条件
            foreach (EventPrerequisite prerequisite in EventPrerequisites)
            {
                if (!EventManager.Instance.IsEventPrerequisiteSatisfied(prerequisite))
                {
                    return false;
                }
            }
            // 检查所有属性值前置条件
            foreach (ValuePrerequisite prerequisite in ValuePrerequisites)
            {
                if (!ValueManager.Instance.IsValuePrerequisiteSatisfied(prerequisite))
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    /// 事件名称。
    /// </summary>
    public string EventName;

    /// <summary>
    /// 事件所属时间段。
    /// </summary>
    public EventTime EventTime;

    /// <summary>
    /// 事件描述。
    /// </summary>
    [Multiline]
    public string EventDescription;

    /// <summary>
    /// 事件可选项列表。
    /// </summary>
    public List<EventOption> EventOptions = new();
}
