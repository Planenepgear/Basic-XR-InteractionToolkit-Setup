#if UNITY_EDITOR
using UnityEditor;

namespace R1Tools.Editor
{
    public static class R1ToolsWindowOpener
    {
        [MenuItem("Window/R1Tools/Enable VRIK Support")]
        private static void Open()
        {
            var window = EditorWindow.GetWindow<R1ToolsWindow>(title: "R1Tools: Enable VRIK Support");
            window.Show();
        }
    }
}
#endif