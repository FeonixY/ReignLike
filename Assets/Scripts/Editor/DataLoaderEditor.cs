using UnityEditor;
using UnityEngine;

/// <summary>
/// DataLoader 的自定义 Inspector，提供一键加载数据的按钮。
/// </summary>
[CustomEditor(typeof(DataLoader))]
public class DataLoaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DataLoader loader = target as DataLoader;
        if (GUILayout.Button("加载数据"))
        {
            loader.LoadData();
        }
    }
}