#if UNITY_EDITOR
#if VRIK_PRESENT
using RootMotion.FinalIK;
#endif

using R1Tools.Core;
using UnityEngine;

namespace RootMotion.Demos
{
    public class CalibratorController : MonoBehaviour
    {
        [Tooltip("Use this to ignore the waist tracker on snapping")]
        public bool ignoreWaist = false;
#if VRIK_PRESENT
        [HideInInspector]
        public VRIK ik;
        [Tooltip("The settings for VRIK calibration.")] public Calibrator.Settings settings;
#endif
        [HideInInspector]
        [Tooltip("The HMD.")] public Transform headTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker placed anywhere on the body of the player, preferrably close to the pelvis, on the belt area.")] public Tracker bodyTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker placed on the chest of the player.")] public Tracker chestTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's left hand.")] public Transform leftHandTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker or hand controller device placed anywhere on or in the player's right hand.")] public Transform rightHandTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker or hand controller device placed anywhere on the player's left elbow.")] public Tracker leftElbowTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker or hand controller device placed anywhere on the player's right elbow.")] public Tracker rightElbowTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker placed anywhere on the knee of the player's left leg.")] public Tracker leftKneeTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker placed anywhere on the knee of the player's right leg.")] public Tracker rightKneeTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's left leg.")] public Tracker leftFootTracker;
        [HideInInspector]
        [Tooltip("(Optional) A tracker placed anywhere on the ankle or toes of the player's right leg.")] public Tracker rightFootTracker;


        [HideInInspector]
        public TrackerAnimTool trackerScript;

#if VRIK_PRESENT
        [Header("Data stored by Calibration")]
        public Calibrator.CalibrationData data = new Calibrator.CalibrationData();

        public void setVrik(VRIK newIK)
        {
            ik = newIK;
        }

        public void calibrate()
        {
            // Calibrate the character, store data of the calibration

            if (ignoreWaist)
            {
                //Check if legs/feet trackers are on, else send null
                if (leftFootTracker.isTracked == true && rightFootTracker.isTracked == true && leftKneeTracker.isTracked == true && rightKneeTracker.isTracked == true)
                {
                    data = Calibrator.Calibrate(ik, settings, headTracker, false, null, chestTracker.offset, leftHandTracker, rightHandTracker, leftElbowTracker.offset, rightElbowTracker.offset, leftKneeTracker.offset, rightKneeTracker.offset, leftFootTracker.offset, rightFootTracker.offset);
                }
                else
                {
                    data = Calibrator.Calibrate(ik, settings, headTracker, true, null, chestTracker.offset, leftHandTracker, rightHandTracker, leftElbowTracker.offset, rightElbowTracker.offset, leftKneeTracker.offset, rightKneeTracker.offset, leftFootTracker.offset, rightFootTracker.offset);
                }
            }
            else
            {
                //Check if legs/feet trackers are on, else send null
                if (leftFootTracker.isTracked == true && rightFootTracker.isTracked == true && leftKneeTracker.isTracked == true && rightKneeTracker.isTracked == true)
                {
                    data = Calibrator.Calibrate(ik, settings, headTracker, false, bodyTracker.offset, chestTracker.offset, leftHandTracker, rightHandTracker, leftElbowTracker.offset, rightElbowTracker.offset, leftKneeTracker.offset, rightKneeTracker.offset, leftFootTracker.offset, rightFootTracker.offset);
                }
                else
                {
                    data = Calibrator.Calibrate(ik, settings, headTracker, true, bodyTracker.offset, chestTracker.offset, leftHandTracker, rightHandTracker, leftElbowTracker.offset, rightElbowTracker.offset, leftKneeTracker.offset, rightKneeTracker.offset, leftFootTracker.offset, rightFootTracker.offset);
                }
            }

            //hide all tracker meshes
            trackerScript.trackerVisibility(false);
            trackerScript.onCalibrate();
        }
#endif
    }
}

#endif