#if VRIK_PRESENT
using RootMotion.FinalIK;
#endif
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace R1Tools.Core
{
    public class GrabLinker : MonoBehaviour
    {
#if VRIK_PRESENT
        public VRIKInteractionSystem VrikInteractionScript;
        public Handedness localHandedness;
#endif
        private XRDirectInteractor localInteractor;

        public bool grabbing = false;


        void Start()
        {
#if VRIK_PRESENT
            if ((GetComponent<XRDirectInteractor>()) != null)
            {
                localInteractor = GetComponent<XRDirectInteractor>();
            }
#endif
        }

        public void defaultPose()
        {

        }

        public void grab()
        {
            if (grabbing == true)
            {
                return;
            }
            grabbing = true;
#if VRIK_PRESENT
            VrikInteractionScript.StartGrab(localInteractor.selectTarget.gameObject.transform, localHandedness);
#endif

        }

        public void unGrab()
        {
            grabbing = false;
#if VRIK_PRESENT
            VrikInteractionScript.Ungrab(localHandedness);
#endif
        }
    }
}