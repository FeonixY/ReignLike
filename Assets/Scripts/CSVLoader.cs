using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

/// <summary>
/// 通用CSV加载器，将CSV文本内容解析为对象列表。
/// </summary>
public static class CSVLoader
{
    /// <summary>
    /// 加载CSV文件并将每一行数据映射为T类型对象。
    /// CSV格式要求：第一行为字段名，第二行为类型，第三行及以后为数据。
    /// </summary>
    /// <typeparam name="T">目标数据类型</typeparam>
    /// <param name="csvFile">CSV文件（TextAsset）</param>
    /// <returns>对象列表</returns>
    public static List<T> Load<T>(TextAsset csvFile) where T : new()
    {
        // 按行分割，跳过前两行（字段名、类型）
        var lines = csvFile.text.Split('\n').Skip(2);
        var fields = csvFile.text.Split('\n')[0].Split(',');
        var types = csvFile.text.Split('\n')[1].Split(',');
        var list = new List<T>();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] rawValues = SplitCSVLine(line);
            T obj = new();

            // 依次为每个字段赋值
            for (int i = 0; i < fields.Length && i < rawValues.Length; i++)
            {
                string val = rawValues[i];
                string type = types[i];
                string field = fields[i];
                SetValue(obj, field, val, type);
            }

            list.Add(obj);
        }

        return list;
    }

    /// <summary>
    /// 按CSV规则分割一行，支持引号包裹的逗号。
    /// </summary>
    /// <param name="line">CSV数据行</param>
    /// <returns>分割后的字段数组</returns>
    static string[] SplitCSVLine(string line)
    {
        List<string> list = new();
        StringBuilder sb = new();
        bool inQuote = false;

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuote = !inQuote;
            }
            else if (c == ',' && !inQuote)
            {
                list.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.Append(c);
            }
        }

        list.Add(sb.ToString());
        return list.ToArray();
    }

    /// <summary>
    /// 反射赋值：将字符串val按type类型转换后赋给obj的field字段。
    /// </summary>
    /// <typeparam name="T">目标对象类型</typeparam>
    /// <param name="obj">目标对象</param>
    /// <param name="field">字段名</param>
    /// <param name="val">字符串值</param>
    /// <param name="type">类型字符串（如int、float、bool、string）</param>
    static void SetValue<T>(T obj, string field, string val, string type)
    {
        string fieldName = field.Trim();
        var fi = typeof(T).GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
        if (fi == null)
        {
            Debug.LogWarning($"字段未找到: '{field}'（Trim后: '{fieldName}'） in {typeof(T)}");
            return;
        }

        val = val.Trim().Trim('"');
        type = type.Trim().ToLower();

        object parsed = type switch
        {
            "int" => int.TryParse(val, out int i) ? i : 0,
            "float" => float.TryParse(val, out float f) ? f : 0f,
            "bool" => bool.TryParse(val, out bool b) && b,
            _ => val // 默认按string处理
        };
        fi.SetValue(obj, parsed);
    }
}