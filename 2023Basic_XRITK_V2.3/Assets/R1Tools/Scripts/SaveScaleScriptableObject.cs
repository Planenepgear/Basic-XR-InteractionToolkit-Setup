using UnityEngine;

namespace R1Tools.Editor
{
    [CreateAssetMenu(fileName = "Data", menuName = "R1Tools/CharacterScaleScriptableObject", order = 1)]
    public class SaveScaleScriptableObject : ScriptableObject
    {
        public Vector3 characterScale;
    }
}
