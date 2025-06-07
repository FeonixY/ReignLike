using UnityEditor;
using UnityEngine;

/// <summary>
/// CSV配置工具编辑器窗口，支持选择CSV文件并生成类与数据预览。
/// </summary>
public class CSVEditorWindow : EditorWindow
{
    private TextAsset csvFile;
    private string previewText;

    [MenuItem("Tools/CSV配置表工具")]
    private static void Open()
    {
        // 打开或聚焦窗口
        var window = GetWindow<CSVEditorWindow>();
        window.titleContent = new GUIContent("CSV配置工具");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("选择CSV文件", EditorStyles.boldLabel);
        csvFile = (TextAsset)EditorGUILayout.ObjectField(csvFile, typeof(TextAsset), false);

        using (new EditorGUI.DisabledScope(csvFile == null))
        {
            if (GUILayout.Button("生成类 & 数据"))
            {
                previewText = CSVGenerator.ProcessCSV(csvFile);
            }
        }

        if (!string.IsNullOrEmpty(previewText))
        {
            EditorGUILayout.LabelField("预览:");
            using var scroll = new EditorGUILayout.ScrollViewScope(Vector2.zero, GUILayout.Height(200));
            EditorGUILayout.TextArea(previewText, GUILayout.ExpandHeight(true));
        }
    }
}
