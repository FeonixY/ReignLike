using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// CSV 生成器：根据 CSV 文件自动生成 C# 类文件，并返回类定义文本。
/// </summary>
public static class CSVGenerator
{
    /// <summary>
    /// 处理 CSV 文件，生成对应的 C# 类文件，并返回类定义字符串。
    /// </summary>
    /// <param name="csvFile">输入的 CSV 文件（TextAsset）</param>
    /// <returns>生成的类定义字符串（用于预览）</returns>
    public static string ProcessCSV(TextAsset csvFile)
    {
        // 按行分割，去除空行
        List<string> lines = csvFile.text.Split('\n').Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
        if (lines.Count < 3) return ""; // 至少需要三行：字段名、类型、数据

        // 解析字段名和类型
        string[] fields = lines[0].Split(',').Select(f => f.Trim()).ToArray();
        string[] types = lines[1].Split(',').Select(t => t.Trim()).ToArray();

        // 获取类名（去除扩展名）
        string className = Path.GetFileNameWithoutExtension(csvFile.name);

        StringBuilder sb = new();

        // 生成类定义
        sb.AppendLine($"public class {className}Table");
        sb.AppendLine("{");
        for (int i = 0; i < types.Length && i < fields.Length; i++)
        {
            sb.AppendLine($"    public {types[i]} {fields[i]};");
        }
        sb.AppendLine("}");

        // 生成文件保存路径
        string dir = Path.Combine(Application.dataPath, "Scripts/ConfigClass");
        string path = Path.Combine(dir, className + "Table.cs");

        // 确保目录存在
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        // 写入文件
        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        AssetDatabase.Refresh();

        Debug.Log($"已生成类文件：{path}");
        return sb.ToString();
    }
}