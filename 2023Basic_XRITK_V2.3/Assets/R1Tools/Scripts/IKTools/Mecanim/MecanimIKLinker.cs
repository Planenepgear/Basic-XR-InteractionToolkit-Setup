#if UNITY_EDITOR
using UnityEngine;

namespace R1Tools.Mecanim
{
    public class MecanimIKLinker : MonoBehaviour
    {
#if !VRIK_PRESENT
        public MecanimIKControl controller;

        void OnAnimatorIK()
        {
            controller.ikUpdate();
        }
#endif
    }
}
#endif
