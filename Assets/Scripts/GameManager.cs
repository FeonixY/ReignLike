using Sirenix.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 游戏主流程管理器，负责事件循环、事件选择、结局判定等。
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    private EventManager eventManager; // 事件管理器
    private ValueManager valueManager; // 属性管理器
    private int currentDay = 1;       // 当前天数
    private int? selectedOptionIndex = null; // 玩家当前选择的选项索引

    /// <summary>
    /// 初始化，启动主循环。
    /// </summary>
    private void Start()
    {
        eventManager = EventManager.Instance;
        valueManager = ValueManager.Instance;
        GameLoop();
    }

    /// <summary>
    /// 游戏主循环：每天选择一个可用事件，处理玩家选择，判断是否触发结局。
    /// </summary>
    private async void GameLoop()
    {
        // 只要还有可用事件就继续循环
        while (TryGetEventsForToday(out List<Event> availableEvents))
        {
            Debug.Log($"第{currentDay}天");

            // 随机选择一个事件
            Event selectedEvent = availableEvents[Random.Range(0, availableEvents.Count)];
            Debug.Log($"事件：{selectedEvent.EventName}");

            // 显示事件UI并等待玩家选择
            ShowEventUIAndGetOptions(selectedEvent);
            bool isEnding = await WaitForPlayerSelectOption(selectedEvent);

            if (isEnding)
            {
                // 触发结局，直接结束循环
                return;
            }

            currentDay++;
            await Task.Delay(1000);
        }

        // 没有可用事件，显示普通结局
        ShowNormalEnd();
    }

    /// <summary>
    /// 获取当天可用事件（按天数分阶段，过滤已完成和前置条件不满足的事件）。
    /// </summary>
    /// <param name="result">输出：可用事件列表</param>
    /// <returns>是否有可用事件</returns>
    private bool TryGetEventsForToday(out List<Event> result)
    {
        result = new();
        if (eventManager == null)
            return false;

        List<Event> events = new();

        // 按天数分阶段获取事件
        int i = eventManager.Events.Count / 3;
        if (currentDay <= i)
            events.AddRange(eventManager.EarlyEvents.Where(kv => !kv.Value).Select(kv => kv.Key));
        else if (currentDay > i && currentDay <= 2 * i)
            events.AddRange(eventManager.MiddleEvents.Where(kv => !kv.Value).Select(kv => kv.Key));
        else
            events.AddRange(eventManager.LateEvents.Where(kv => !kv.Value).Select(kv => kv.Key));

        // 过滤前置条件不满足的事件
        foreach (var e in events)
        {
            if (e.IsPrerequisitesSatisfied)
            {
                result.Add(e);
            }
        }

        return !result.IsNullOrEmpty();
    }

    /// <summary>
    /// 显示事件UI，并设置选项回调。
    /// </summary>
    /// <param name="selectedEvent">当前事件</param>
    private void ShowEventUIAndGetOptions(Event selectedEvent)
    {
        // 组装选项文本
        string[] options = new string[selectedEvent.EventOptions.Count];
        for (int i = 0; i < selectedEvent.EventOptions.Count; i++)
        {
            options[i] = selectedEvent.EventOptions[i].OptionText;
        }

        // 组装每个选项对应的属性变化
        var optionValueChanges = selectedEvent.EventOptions
            .Select(opt => opt.ValueChanges)
            .ToList();

        // 显示UI，设置选项回调
        EventUIManager.Instance.ShowEventUI(
            currentDay,
            selectedEvent.EventName,
            selectedEvent.EventDescription,
            options,
            (index) => { selectedOptionIndex = index; },
            optionValueChanges
        );
    }

    /// <summary>
    /// 等待玩家选择事件选项，并处理属性变化与结局判定。
    /// </summary>
    /// <param name="selectedEvent">当前事件</param>
    /// <returns>是否触发结局</returns>
    private async Task<bool> WaitForPlayerSelectOption(Event selectedEvent)
    {
        selectedOptionIndex = null;

        // 等待玩家选择
        while (selectedOptionIndex == null)
        {
            await Task.Yield();
        }

        var option = selectedEvent.EventOptions[selectedOptionIndex.Value];
        Debug.Log($"选择：{option.OptionText}");

        // 记录事件结果
        eventManager.AddEventResult(new KeyValuePair<Event, int>(selectedEvent, selectedOptionIndex.Value));

        // 处理属性变化，判断是否触发极端结局
        foreach (var change in option.ValueChanges)
        {
            if (valueManager.TryUpdateValue(change.Key, change.Value, out bool isTooHigh))
            {
                if (isTooHigh)
                {
                    EventUIManager.Instance.ShowTooHighEnding(change.Key);
                    return true; // 触发TooHigh结局
                }
                else
                {
                    EventUIManager.Instance.ShowTooLowEnding(change.Key);
                    return true; // 触发TooLow结局
                }
            }
        }

        selectedOptionIndex = null;
        return false; // 没有触发结局，继续游戏循环
    }

    /// <summary>
    /// 显示普通结局（所有属性未触发极端结局时调用）。
    /// </summary>
    private void ShowNormalEnd()
    {
        EventUIManager.Instance.ShowNormalEnding(valueManager.PrepareShowNormalEnding());
    }
}