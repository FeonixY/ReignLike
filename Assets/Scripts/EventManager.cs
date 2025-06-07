using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 事件管理器，负责事件的分类、状态管理和前置条件判断。
/// </summary>
public class EventManager : SingletonMonoBehaviour<EventManager>
{
    /// <summary>
    /// 所有事件及其完成状态（true=已完成，false=未完成）。
    /// </summary>
    public Dictionary<Event, bool> Events = new();

    /// <summary>
    /// 早期事件及其完成状态。
    /// </summary>
    public Dictionary<Event, bool> EarlyEvents { get; private set; } = new();

    /// <summary>
    /// 中期事件及其完成状态。
    /// </summary>
    public Dictionary<Event, bool> MiddleEvents { get; private set; } = new();

    /// <summary>
    /// 晚期事件及其完成状态。
    /// </summary>
    public Dictionary<Event, bool> LateEvents { get; private set; } = new();

    /// <summary>
    /// 记录每个事件玩家选择的选项索引。
    /// </summary>
    private readonly Dictionary<Event, int> eventResults = new();

    /// <summary>
    /// 启动时对事件进行分类。
    /// </summary>
    private void Start()
    {
        CategorizeEvents();
    }

    /// <summary>
    /// 按事件时间（早/中/晚）分类，将事件分配到对应字典。
    /// </summary>
    private void CategorizeEvents()
    {
        EarlyEvents.Clear();
        MiddleEvents.Clear();
        LateEvents.Clear();

        foreach (var e in Events)
        {
            var evtTime = e.Key.EventTime;
            if ((evtTime & EventTime.Early) != 0)
                EarlyEvents.Add(e.Key, e.Value);
            if ((evtTime & EventTime.Middle) != 0)
                MiddleEvents.Add(e.Key, e.Value);
            if ((evtTime & EventTime.Late) != 0)
                LateEvents.Add(e.Key, e.Value);
        }
    }

    /// <summary>
    /// 记录事件结果，并将事件标记为已完成。
    /// </summary>
    /// <param name="result">事件及玩家选择的选项索引</param>
    public void AddEventResult(KeyValuePair<Event, int> result)
    {
        var evt = result.Key;
        var evtTime = evt.EventTime;

        // 标记对应阶段的事件为已完成
        if ((evtTime & EventTime.Early) != 0 && EarlyEvents.ContainsKey(evt))
            EarlyEvents[evt] = true;
        if ((evtTime & EventTime.Middle) != 0 && MiddleEvents.ContainsKey(evt))
            MiddleEvents[evt] = true;
        if ((evtTime & EventTime.Late) != 0 && LateEvents.ContainsKey(evt))
            LateEvents[evt] = true;

        // 记录玩家选择的选项
        eventResults[evt] = result.Value;
    }

    /// <summary>
    /// 判断某个事件前置条件是否满足（即指定事件已完成且选项匹配）。
    /// </summary>
    /// <param name="ep">事件前置条件</param>
    /// <returns>是否满足条件</returns>
    public bool IsEventPrerequisiteSatisfied(EventPrerequisite ep)
    {
        if (eventResults.TryGetValue(ep.Event, out int selectedOption))
        {
            return selectedOption == ep.RequiredOption;
        }
        return false;
    }
}
