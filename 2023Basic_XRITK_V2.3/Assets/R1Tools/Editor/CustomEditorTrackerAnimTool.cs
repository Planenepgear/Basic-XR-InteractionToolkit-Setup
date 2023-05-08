#if UNITY_EDITOR
using R1Tools.Core;
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(TrackerAnimTool))]
public class CustomEditorTrackerAnimTool : Editor
{
    SerializedProperty targetProperty;
    private Color enabledColor = Color.green;
    private Color disabledColor = Color.magenta;

    private void OnEnable()
    {
        targetProperty = serializedObject.FindProperty("fingerMappingMode");
    }

    public override void OnInspectorGUI()
    {
        Color originalGUIColor = GUI.color;

#if VRIK_PRESENT
        GUI.color = enabledColor;
        GUILayout.Label("VRIK Enabled", EditorStyles.wordWrappedLabel);
        GUI.color = originalGUIColor;
#else
        GUI.color = disabledColor;
        GUILayout.Label("VRIK Disabled if VRIK is imported, use Window/R1Tools/Enable VRIK to enable", EditorStyles.wordWrappedLabel);
        GUI.color = originalGUIColor;
#endif
        DrawDefaultInspector();
    }
}
#endif
