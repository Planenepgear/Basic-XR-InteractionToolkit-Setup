using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using VRTK;

namespace RootMotion.FinalIK
{

    /// <summary>
    /// Extending the VRIKInteractionSystem to make it work with VRTK.
    /// </summary>
    public class VRIK_VRTK_InteractionSystem : VRIKInteractionSystem
    {
        /*
                [Header("VRTK")]

                public VRTK_InteractGrab grabLeft;
                public VRTK_InteractGrab grabRight;

                void Start()
                {
                    // Register to get calls from the left controller when objects are picked up/dropped
                    grabLeft.ControllerStartGrabInteractableObject += OnStartGrabLeft;
                    grabLeft.ControllerStartUngrabInteractableObject += OnStartUngrabLeft;

                    // Register to get calls from the right controller when objects are picked up/dropped
                    grabRight.ControllerStartGrabInteractableObject += OnStartGrabRight;
                    grabRight.ControllerStartUngrabInteractableObject += OnStartUngrabRight;
                }

                void OnStartGrabLeft(object sender, ObjectInteractEventArgs e)
                {
                    StartGrab(e.target.transform, Handedness.Left);
                }

                void OnStartGrabRight(object sender, ObjectInteractEventArgs e)
                {
                    StartGrab(e.target.transform, Handedness.Right);
                }

                void OnStartUngrabLeft(object sender, ObjectInteractEventArgs e)
                {
                    Ungrab(Handedness.Left);
                }

                void OnStartUngrabRight(object sender, ObjectInteractEventArgs e)
                {
                    Ungrab(Handedness.Right);
                }
                */
    }

}
