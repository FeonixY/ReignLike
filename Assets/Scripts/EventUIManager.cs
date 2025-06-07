using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 事件UI管理器，负责事件界面的选项生成、结局展示等。
/// </summary>
public class EventUIManager : SingletonMonoBehaviour<EventUIManager>
{
    // 当前天数文本
    public TMP_Text DayText;
    // 标题文本
    public TMP_Text Title;
    // 描述文本
    public TMP_Text Description;
    // 选项按钮预制体
    public GameObject OptionPrefab;
    // 选项布局父节点
    public Transform LayoutGroupTransform;
    // 坏结局标题
    public TMP_Text BadEndingTitle;
    // 坏结局描述
    public TMP_Text BadEndingDescription;

    // 普通结局UI文本（每种属性一个）
    public Dictionary<ValueType, TMP_Text> NormalEndings = new();
    // 各属性过低时的结局文本（标题, 描述）
    [HideInInspector]
    public Dictionary<ValueType, (string, string)> TooLowEndingTexts = new();
    // 各属性过高时的结局文本（标题, 描述）
    [HideInInspector]
    public Dictionary<ValueType, (string, string)> TooHighEndingTexts = new();
    // 普通结局文本（属性类型, 等级, 描述）
    [HideInInspector]
    public Dictionary<(ValueType, int), string> NormalEndingTexts = new();

    // 当前选项回调
    private Action<int> onOptionSelected;
    // 当前所有选项按钮
    private readonly List<Button> optionButtons = new();

    /// <summary>
    /// 显示事件UI，生成选项按钮并绑定回调与悬浮事件。
    /// </summary>
    /// <param name="title">事件标题</param>
    /// <param name="description">事件描述</param>
    /// <param name="options">选项文本数组</param>
    /// <param name="onOptionSelected">选项点击回调</param>
    /// <param name="optionValueChanges">每个选项对应的属性变化</param>
    public void ShowEventUI(
        int currentDay,
        string title,
        string description,
        string[] options,
        Action<int> onOptionSelected,
        List<Dictionary<ValueType, int>> optionValueChanges)
    {
        Clear();

        DayText.SetText($"第{currentDay}天");
        Title.SetText(title);
        Description.SetText(description);
        this.onOptionSelected = onOptionSelected;
        optionButtons.Clear();

        for (int i = 0; i < options.Length; i++)
        {
            int index = i;
            // 实例化选项按钮
            var optionInstance = Instantiate(OptionPrefab, LayoutGroupTransform);
            optionInstance.GetComponentInChildren<TMP_Text>().SetText(options[i]);
            var btn = optionInstance.GetComponentInChildren<Button>();
            if (btn != null)
            {
                optionButtons.Add(btn);
                btn.onClick.AddListener(() =>
                {
                    DisableAllOptionButtons();
                    ValueUIManager.Instance.HideValueChanges();
                    OnOptionButtonClicked(index);
                });

                // 添加鼠标悬浮事件（显示/隐藏属性变化）
                if (!optionInstance.TryGetComponent<EventTrigger>(out var trigger))
                    trigger = optionInstance.AddComponent<EventTrigger>();

                // 鼠标进入时显示变化
                var entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
                entryEnter.callback.AddListener((_) =>
                {
                    if (optionValueChanges != null && index < optionValueChanges.Count)
                    {
                        ValueUIManager.Instance.ShowValueChanges(optionValueChanges[index]);
                    }
                });
                trigger.triggers.Add(entryEnter);

                // 鼠标离开时隐藏变化
                var entryExit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
                entryExit.callback.AddListener((_) =>
                {
                    ValueUIManager.Instance.HideValueChanges();
                });
                trigger.triggers.Add(entryExit);
            }
        }
    }

    /// <summary>
    /// 禁用所有选项按钮（防止多次点击）。
    /// </summary>
    private void DisableAllOptionButtons()
    {
        foreach (var btn in optionButtons)
        {
            if (btn != null)
                btn.interactable = false;
        }
    }

    /// <summary>
    /// 清空界面，销毁所有选项按钮。
    /// </summary>
    private void Clear()
    {
        Title.SetText("");
        Description.SetText("");
        foreach (Transform child in LayoutGroupTransform)
        {
            Destroy(child.gameObject);
        }
        optionButtons.Clear();
    }

    /// <summary>
    /// 选项按钮点击回调。
    /// </summary>
    /// <param name="index">被点击的选项索引</param>
    private void OnOptionButtonClicked(int index) => onOptionSelected?.Invoke(index);

    /// <summary>
    /// 显示属性过低的结局界面。
    /// </summary>
    /// <param name="valueType">属性类型</param>
    public void ShowTooLowEnding(ValueType valueType)
    {
        if (TooLowEndingTexts.TryGetValue(valueType, out var endingText))
        {
            Clear();
            BadEndingTitle.SetText(endingText.Item1);
            BadEndingDescription.SetText(endingText.Item2);
            BadEndingTitle.gameObject.SetActive(true);
            BadEndingDescription.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 显示属性过高的结局界面。
    /// </summary>
    /// <param name="valueType">属性类型</param>
    public void ShowTooHighEnding(ValueType valueType)
    {
        if (TooHighEndingTexts.TryGetValue(valueType, out var endingText))
        {
            Clear();
            BadEndingTitle.SetText(endingText.Item1);
            BadEndingDescription.SetText(endingText.Item2);
            BadEndingTitle.gameObject.SetActive(true);
            BadEndingDescription.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 显示普通结局（根据每个属性的等级展示对应文本）。
    /// </summary>
    /// <param name="valueLevels">每个属性的等级</param>
    public void ShowNormalEnding(Dictionary<ValueType, int> valueLevels)
    {
        Clear();

        foreach (var sv in valueLevels)
        {
            // 查找对应等级的结局文本
            var normalEndingText = NormalEndingTexts
                .FirstOrDefault(net => net.Key.Item1 == sv.Key && net.Key.Item2 == sv.Value);

            if (normalEndingText.Value != null && NormalEndings.TryGetValue(sv.Key, out var textComponent))
            {
                textComponent.SetText(normalEndingText.Value);
                textComponent.gameObject.SetActive(true);
            }
        }
    }
}
