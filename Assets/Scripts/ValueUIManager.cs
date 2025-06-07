using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理所有属性（ValueType）相关的数值UI显示与动画。
/// </summary>
public class ValueUIManager : SingletonMonoBehaviour<ValueUIManager>
{
    /// <summary>
    /// 当前各属性的主Slider（用于实时显示属性值）
    /// </summary>
    public Dictionary<ValueType, Slider> MainSliders = new();

    /// <summary>
    /// 用于预览变化的Slider（用于悬浮选项时显示变化后的属性值）
    /// </summary>
    public Dictionary<ValueType, Slider> PreviewSliders = new();

    /// <summary>
    /// 各属性Slider对应的CanvasGroup（用于渐变显示/隐藏）
    /// </summary>
    public Dictionary<ValueType, CanvasGroup> SliderCanvasGroups = new();

    /// <summary>
    /// 更新指定属性的主Slider数值。
    /// </summary>
    /// <param name="valueType">要更新的属性类型</param>
    public void UpdateValue(ValueType valueType)
    {
        if (MainSliders.TryGetValue(valueType, out Slider slider) && slider != null)
        {
            if (ValueManager.Instance.PlayerValues.TryGetValue(valueType, out int value))
            {
                slider.value = value;
            }
        }
    }

    /// <summary>
    /// 显示数值变化（如选项悬浮时），并渐入预览Slider。
    /// </summary>
    /// <param name="changes">各属性的变化量</param>
    public void ShowValueChanges(Dictionary<ValueType, int> changes)
    {
        foreach (KeyValuePair<ValueType, Slider> kvp in PreviewSliders)
        {
            ValueType valueType = kvp.Key;
            Slider slider = kvp.Value;
            if (slider == null) continue;
            if (!SliderCanvasGroups.TryGetValue(valueType, out CanvasGroup canvasGroup)) continue;

            if (changes.TryGetValue(valueType, out int delta))
            {
                // 预览Slider显示变化后的数值
                if (ValueManager.Instance.PlayerValues.TryGetValue(valueType, out int baseValue))
                    slider.value = baseValue + delta;
                // 渐入显示
                StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0.2f));
            }
        }
    }

    /// <summary>
    /// 隐藏所有预览变化（渐出所有预览Slider）。
    /// </summary>
    public void HideValueChanges()
    {
        StopAllCoroutines();
        foreach (CanvasGroup canvasGroup in SliderCanvasGroups.Values)
        {
            StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 0.2f));
        }
    }

    /// <summary>
    /// 渐变显示/隐藏CanvasGroup的透明度。
    /// </summary>
    /// <param name="canvasGroup">目标CanvasGroup</param>
    /// <param name="targetAlpha">目标透明度</param>
    /// <param name="duration">渐变时长（秒）</param>
    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float targetAlpha, float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;
        while (time < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
