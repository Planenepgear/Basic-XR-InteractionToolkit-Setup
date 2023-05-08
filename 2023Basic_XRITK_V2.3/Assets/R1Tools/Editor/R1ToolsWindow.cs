#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace R1Tools.Editor
{
    [InitializeOnLoad]
    public class R1ToolsWindow : EditorWindow
    {
        void OnGUI()
        {
            EditorGUILayout.LabelField("Click the buttons to enable VRIK support (Only do this if you have imported it to the project first)", EditorStyles.wordWrappedLabel);
            GUILayout.Space(5);
            if (GUILayout.Button("Add VRIK Support"))
            {
                InsertR1DefineSymbols(0);
            }
            if (GUILayout.Button("Remove VRIK Support"))
            {
                InsertR1DefineSymbols(2);
            }
        }

        public static readonly string[] VrikSymbols = new string[] {
        "VRIK_PRESENT"
    };

        private static void InsertR1DefineSymbols(int mode)
        {
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            List<string> allDefines = definesString.Split(';').ToList();
            switch (mode)
            {
                case 0:
                    allDefines.AddRange(VrikSymbols.Except(allDefines));
                    break;
                case 1:
                    //allDefines.AddRange(HptkSymbols.Except(allDefines));
                    break;
                case 2:
                    allDefines.Remove("VRIK_PRESENT");
                    break;
                case 3:
                    // allDefines.Remove("HPTK_PRESENT");
                    break;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(
                EditorUserBuildSettings.selectedBuildTargetGroup,
                string.Join(";", allDefines.ToArray()));

            EditorUtility.RequestScriptReload();
        }
    }
}
#endif