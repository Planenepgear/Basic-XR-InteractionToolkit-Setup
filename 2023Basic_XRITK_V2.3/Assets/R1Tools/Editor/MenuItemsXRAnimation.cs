#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace R1Tools.Editor
{
    public class MenuItemsXRAnimation : MonoBehaviour
    {
        // Add a menu item to create custom GameObjects.
        // Priority 1 ensures it is grouped with the other menu items of the same kind
        // and propagated to the hierarchy dropdown and hierarchy context menus.
        /*
        [MenuItem("GameObject/XR Animation/Create XR Animation Rig", false, 10)]
        static void CreateCustomGameObject(MenuCommand menuCommand)
        {
            GameObject XrAnimationRig =
                   (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>
                   ("Assets/Plugins/R1Tools/Prefabs/XR Animation Rig.prefab"));

            Undo.RegisterCreatedObjectUndo(XrAnimationRig, "Create " + XrAnimationRig.name);
            Selection.activeObject = XrAnimationRig;
        }
        */



        [MenuItem("GameObject/XR Animation/Create XR Animation Rig + Gizmos", false, 10)]
        static void CreateCustomGameObject1(MenuCommand menuCommand)
        {
            GameObject R1ToolsGizmos =
            (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>
            ("Assets/R1Tools/Prefabs/R1 Tools Gizmos.prefab"));

            Undo.RegisterCreatedObjectUndo(R1ToolsGizmos, "Create " + R1ToolsGizmos.name);

            GameObject XrAnimationRig =
           (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>
           ("Assets/R1Tools/Prefabs/XR Animation Rig.prefab"));

            Undo.RegisterCreatedObjectUndo(XrAnimationRig, "Create " + XrAnimationRig.name);

            if (Selection.activeTransform != null)
            {
                //Set the parent of the objects
                R1ToolsGizmos.transform.position = Selection.activeTransform.position;
                XrAnimationRig.transform.position = Selection.activeTransform.position;
            }

            Selection.activeObject = XrAnimationRig;
        }

        /*
        [MenuItem("GameObject/XR Animation/Create Mirror", false, 10)]
        static void CreateCustomGameObject2(MenuCommand menuCommand)
        {
            GameObject Mirror =
                   (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>
                   ("Assets/Plugins/R1Tools/Prefabs/MirrorPlane.prefab"));

            Undo.RegisterCreatedObjectUndo(Mirror, "Create " + Mirror.name);
            Selection.activeObject = Mirror;
        }
        */
    }
}
#endif
