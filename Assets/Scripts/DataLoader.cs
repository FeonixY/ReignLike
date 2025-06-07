using UnityEngine;

/// <summary>
/// 数据加载器：负责从CSV文件加载结局数据并导入到EventUIManager。
/// </summary>
public class DataLoader : MonoBehaviour
{
    /// <summary>
    /// 事件UI管理器引用。
    /// </summary>
    public EventUIManager EventUIManager;

    /// <summary>
    /// 坏结局CSV文件。
    /// </summary>
    public TextAsset BadEndingsTableCSV;

    /// <summary>
    /// 普通结局CSV文件。
    /// </summary>
    public TextAsset NormalEndingsTableCSV;

    /// <summary>
    /// 加载CSV数据并填充到EventUIManager的结局字典。
    /// </summary>
    /// 

    private void Awake()
    {
        LoadData();
    }

    public void LoadData()
    {
        // 加载坏结局数据
        var badEndings = CSVLoader.Load<BadEndingsTable>(BadEndingsTableCSV);

        foreach (var ending in badEndings)
        {
            // 解析属性类型
            var valueType = ValueTypeExtensions.Parse(ending.Value);
            var tuple = (ending.Title, ending.Description);

            // 根据IsTooHigh分配到高/低结局字典
            if (ending.IsTooHigh)
            {
                EventUIManager.TooHighEndingTexts ??= new();
                EventUIManager.TooHighEndingTexts[valueType] = tuple;
                Debug.Log($"Loaded too high ending for {valueType}: {tuple.Title} - {tuple.Description}");
            }
            else
            {
                EventUIManager.TooLowEndingTexts ??= new();
                EventUIManager.TooLowEndingTexts[valueType] = tuple;
                Debug.Log($"Loaded too low ending for {valueType}: {tuple.Title} - {tuple.Description}");
            }
        }

        // 加载普通结局数据
        var normalEndings = CSVLoader.Load<NormalEndingsTable>(NormalEndingsTableCSV);

        foreach (var ending in normalEndings)
        {
            // 解析属性类型
            var valueType = ValueTypeExtensions.Parse(ending.Value);
            EventUIManager.NormalEndingTexts ??= new();
            EventUIManager.NormalEndingTexts[(valueType, ending.Level)] = ending.Description;
            Debug.Log($"Loaded normal ending for {valueType}: Level {ending.Level} - {ending.Description}");
        }
    }
}
