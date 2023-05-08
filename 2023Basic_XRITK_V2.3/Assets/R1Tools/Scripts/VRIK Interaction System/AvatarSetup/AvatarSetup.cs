#if VRIK_PRESENT
#endif
using UnityEngine;

namespace R1Tools.Core
{
    public class AvatarSetup : MonoBehaviour
    {

        [Tooltip("The avatar model to set up for VRIK.")]
        public AvatarModel model;

        [Tooltip("The Transfrom that is sycned to the 'CenterEyeProxy'[Oculus] or 'Camera (head)'[SteamVR] in a networked game.")]
        public Transform centerEyeProxy;

        [Tooltip("The Transfrom that is sycned to the 'LeftHandProxy'[Oculus] or 'Controller (left)'[SteamVR] in a networked game.")]
        public Transform leftHandProxy;

        [Tooltip("The Transfrom that is sycned to the 'RightHandProxy'[Oculus] or 'Controller (right)'[SteamVR] in a networked game.")]
        public Transform rightHandProxy;

        void Start()
        {
#if VRIK_PRESENT
        model.CreateIKTargets(centerEyeProxy, leftHandProxy, rightHandProxy);
#endif
        }
    }
}
