#if VRIK_PRESENT
using UnityEngine;
#endif
using R1Tools.Core;

namespace RootMotion.FinalIK
{

    /// <summary>
    /// Calibrates VRIK for the HMD and additional trackers.
    /// </summary>
    public static class Calibrator
    {
#if VRIK_PRESENT
        /// <summary>
        /// The settings for VRIK tracker calibration.
        /// </summary>
        [System.Serializable]
        public class Settings
        {
            /// <summary>
            /// Multiplies character scale.
            /// </summary>
            [Tooltip("Multiplies character scale")]
            public float scaleMlp = 1f;

            /// <summary>
            /// Local axis of the HMD facing forward.
            /// </summary>
            [Tooltip("Local axis of the HMD facing forward.")]
            public Vector3 headTrackerForward = Vector3.forward;

            /// <summary>
			/// Local axis of the HMD facing up.
            /// </summary>
            [Tooltip("Local axis of the HMD facing up.")]
            public Vector3 headTrackerUp = Vector3.up;

            /// <summary>
			/// Local axis of the body tracker towards the player's forward direction.
            /// </summary>
			[Tooltip("Local axis of the body tracker towards the player's forward direction.")]
            public Vector3 bodyTrackerForward = Vector3.forward;

            /// <summary>
            /// Local axis of the body tracker towards the up direction.
            /// </summary>
            [Tooltip("Local axis of the body tracker towards the up direction.")]
            public Vector3 bodyTrackerUp = Vector3.up;

            /// <summary>
            /// Local axis of the hand trackers pointing from the wrist towards the palm.
            /// </summary>
            [Tooltip("Local axis of the hand trackers pointing from the wrist towards the palm.")]
            public Vector3 handTrackerForward = Vector3.forward;

            /// <summary>
            /// Local axis of the hand trackers pointing in the direction of the surface normal of the back of the hand.
            /// </summary>
            [Tooltip("Local axis of the hand trackers pointing in the direction of the surface normal of the back of the hand.")]
            public Vector3 handTrackerUp = Vector3.up;

            /// <summary>
            /// Local axis of the foot trackers towards the player's forward direction.
            /// </summary>
            [Tooltip("Local axis of the foot trackers towards the player's forward direction.")]
            public Vector3 footTrackerForward = Vector3.forward;

            /// <summary>
            /// Local axis of the foot tracker towards the up direction.
            /// </summary>
            [Tooltip("Local axis of the foot tracker towards the up direction.")]
            public Vector3 footTrackerUp = Vector3.up;

            [Space(10f)]
            /// <summary>
			/// Offset of the head bone from the HMD in (headTrackerForward, headTrackerUp) space relative to the head tracker.
            /// </summary>
			[Tooltip("Offset of the head bone from the HMD in (headTrackerForward, headTrackerUp) space relative to the head tracker.")]
            public Vector3 headOffset;

            /// <summary>
            /// Offset of the hand bones from the hand trackers in (handTrackerForward, handTrackerUp) space relative to the hand trackers.
            /// </summary>
            [Tooltip("Offset of the hand bones from the hand trackers in (handTrackerForward, handTrackerUp) space relative to the hand trackers.")]
            public Vector3 handOffset;

            /// <summary>
            /// Forward offset of the foot bones from the foot trackers.
            /// </summary>
            [Tooltip("Forward offset of the foot bones from the foot trackers.")]
            public float footForwardOffset;

            /// <summary>
            /// Inward offset of the foot bones from the foot trackers.
            /// </summary>
            [Tooltip("Inward offset of the foot bones from the foot trackers.")]
            public float footInwardOffset;

            /// <summary>
            /// Used for adjusting foot heading relative to the foot trackers.
            /// </summary>
            [Tooltip("Used for adjusting foot heading relative to the foot trackers.")]
            [Range(-180f, 180f)]
            public float footHeadingOffset;

            /// <summary>
            /// Pelvis target position weight. If the body tracker is on the backpack or somewhere else not very close to the pelvis of the player, position weight needs to be reduced to allow some bending for the spine.
            /// </summary>
            [Range(0f, 1f)] public float pelvisPositionWeight = 1f;

            /// <summary>
            /// Pelvis target rotation weight. If the body tracker is on the backpack or somewhere else not very close to the pelvis of the player, rotation weight needs to be reduced to allow some bending for the spine.
            /// </summary>
            [Range(0f, 1f)] public float pelvisRotationWeight = 1f;
        }

        /// <summary>
        /// Recalibrates only the avatar scale. Can be called only if the avatar has already been calibrated.
        /// </summary>
        public static void RecalibrateScale(VRIK ik, Settings settings)
        {
            float sizeF = (ik.solver.spine.headTarget.position.y - ik.references.root.position.y) / (ik.references.head.position.y - ik.references.root.position.y);
            ik.references.root.localScale *= sizeF * settings.scaleMlp;
        }

        /// <summary>
        /// Calibrates VRIK to the specified trackers using the VRIKTrackerCalibrator.Settings.
        /// </summary>
        /// <param name="ik">Reference to the VRIK component.</param>
        /// <param name="settings">Calibration settings.</param>
        /// <param name="headTracker">The HMD.</param>
        /// <param name="bodyTracker">(Optional) A tracker placed anywhere on the body of the player, preferrably close to the pelvis, on the belt area.</param>
        /// <param name="chestTracker">(Optional) A tracker placed on the chest of the player.</param>
        /// <param name="leftHandTracker">(Optional) A tracker or hand controller device placed anywhere on or in the player's left hand.</param>
        /// <param name="rightHandTracker">(Optional) A tracker or hand controller device placed anywhere on or in the player's right hand.</param>
        /// <param name="leftElbowtracker">(Optional) A tracker or hand controller device placed anywhere on the player's left elbow.</param>
        /// <param name="rightElbowtracker">(Optional) A tracker or hand controller device placed anywhere on the player's right elbow.</param>
        /// <param name="leftKneeTracker">(Optional) A tracker placed anywhere on the knee of the player's left leg.</param>
        /// <param name="rightKneeTracker">(Optional) A tracker placed anywhere on the knee of the player's left leg.</param>
        /// <param name="leftFootTracker">(Optional) A tracker placed anywhere on the ankle or toes of the player's left leg.</param>
        /// <param name="rightFootTracker">(Optional) A tracker placed anywhere on the ankle or toes of the player's right leg.</param>
        public static CalibrationData Calibrate(VRIK ik, Settings settings, Transform headTracker, bool useLocomotion, Transform bodyTracker = null, Transform chestTracker = null, Transform leftHandTracker = null, Transform rightHandTracker = null, Transform leftElbowTracker = null, Transform rightElbowTracker = null, Transform leftKneeTracker = null, Transform rightKneeTracker = null, Transform leftFootTracker = null, Transform rightFootTracker = null)
        {
            Debug.Log(useLocomotion);
            if (ik != null)
            {
                if (!ik.solver.initiated)
                {
                    Debug.LogError("Can not calibrate before VRIK has initiated.");
                    return null;
                }

                if (headTracker == null)
                {
                    Debug.LogError("Can not calibrate VRIK without the head tracker.");
                    return null;
                }

                CalibrationData data = new CalibrationData();

                ik.solver.FixTransforms();

                // Root position and rotation
                Vector3 headPos = headTracker.position + headTracker.rotation * Quaternion.LookRotation(settings.headTrackerForward, settings.headTrackerUp) * settings.headOffset;
                ik.references.root.position = new Vector3(headPos.x, ik.references.root.position.y, headPos.z);
                Vector3 headForward = headTracker.rotation * settings.headTrackerForward;
                headForward.y = 0f;
                ik.references.root.rotation = Quaternion.LookRotation(headForward);

                // Head
                Transform headTarget = ik.solver.spine.headTarget == null ? (new GameObject("Head Target")).transform : ik.solver.spine.headTarget;
                headTarget.position = headPos;
                headTarget.rotation = ik.references.head.rotation;
                headTarget.parent = headTracker;
                ik.solver.spine.headTarget = headTarget;

                // Size
                float sizeF = (headTarget.position.y - ik.references.root.position.y) / (ik.references.head.position.y - ik.references.root.position.y);
                ik.references.root.localScale *= sizeF * settings.scaleMlp;

                // Body
                if (bodyTracker != null)
                {
                    if (bodyTracker.gameObject.activeInHierarchy == true)
                    {
                        Transform pelvisTarget = ik.solver.spine.pelvisTarget == null ? (new GameObject("Pelvis Target")).transform : ik.solver.spine.pelvisTarget;
                        pelvisTarget.position = ik.references.pelvis.position;
                        pelvisTarget.rotation = ik.references.pelvis.rotation;
                        pelvisTarget.parent = bodyTracker;
                        ik.solver.spine.pelvisTarget = pelvisTarget;

                        ik.solver.spine.pelvisPositionWeight = settings.pelvisPositionWeight;
                        ik.solver.spine.pelvisRotationWeight = settings.pelvisRotationWeight;

                        ik.solver.plantFeet = false;
                        ik.solver.spine.maxRootAngle = 180f;
                    }
                }
                else if (leftFootTracker != null && rightFootTracker != null)
                {
                    if (leftFootTracker.gameObject.activeInHierarchy == true && rightFootTracker.gameObject.activeInHierarchy == true)
                    {
                        ik.solver.spine.maxRootAngle = 0f;
                    }
                }

                // Chest
                if (chestTracker != null)
                {
                    if (chestTracker.gameObject.activeInHierarchy == true)
                    {
                        ik.solver.spine.chestGoal = chestTracker;
                        ik.solver.spine.chestGoalWeight = 1f;
                    }
                }

                // Left Hand
                if (leftHandTracker != null)
                {
                    if (leftHandTracker.gameObject.activeInHierarchy == true)
                    {
                        Transform leftHandTarget = ik.solver.leftArm.target == null ? (new GameObject("Left Hand Target")).transform : ik.solver.leftArm.target;
                        leftHandTarget.position = leftHandTracker.position + leftHandTracker.rotation * Quaternion.LookRotation(settings.handTrackerForward, settings.handTrackerUp) * settings.handOffset;
                        Vector3 leftHandUp = Vector3.Cross(ik.solver.leftArm.wristToPalmAxis, ik.solver.leftArm.palmToThumbAxis);
                        leftHandTarget.rotation = R1QuaTools.MatchRotation(leftHandTracker.rotation * Quaternion.LookRotation(settings.handTrackerForward, settings.handTrackerUp), settings.handTrackerForward, settings.handTrackerUp, ik.solver.leftArm.wristToPalmAxis, leftHandUp);
                        leftHandTarget.parent = leftHandTracker;
                        ik.solver.leftArm.target = leftHandTarget;
                        ik.solver.leftArm.positionWeight = 1f;
                        ik.solver.leftArm.rotationWeight = 1f;
                    }
                    else
                    {
                        ik.solver.leftArm.positionWeight = 0f;
                        ik.solver.leftArm.rotationWeight = 0f;
                    }
                }
                else
                {
                    ik.solver.leftArm.positionWeight = 0f;
                    ik.solver.leftArm.rotationWeight = 0f;
                }

                // Right Hand
                if (rightHandTracker != null)
                {
                    if (rightHandTracker.gameObject.activeInHierarchy == true)
                    {
                        Transform rightHandTarget = ik.solver.rightArm.target == null ? (new GameObject("Right Hand Target")).transform : ik.solver.rightArm.target;
                        rightHandTarget.position = rightHandTracker.position + rightHandTracker.rotation * Quaternion.LookRotation(settings.handTrackerForward, settings.handTrackerUp) * settings.handOffset;
                        Vector3 rightHandUp = -Vector3.Cross(ik.solver.rightArm.wristToPalmAxis, ik.solver.rightArm.palmToThumbAxis);
                        rightHandTarget.rotation = R1QuaTools.MatchRotation(rightHandTracker.rotation * Quaternion.LookRotation(settings.handTrackerForward, settings.handTrackerUp), settings.handTrackerForward, settings.handTrackerUp, ik.solver.rightArm.wristToPalmAxis, rightHandUp);
                        rightHandTarget.parent = rightHandTracker;
                        ik.solver.rightArm.target = rightHandTarget;
                        ik.solver.rightArm.positionWeight = 1f;
                        ik.solver.rightArm.rotationWeight = 1f;
                    }
                    else
                    {
                        ik.solver.rightArm.positionWeight = 0f;
                        ik.solver.rightArm.rotationWeight = 0f;
                    }
                }
                else
                {
                    ik.solver.rightArm.positionWeight = 0f;
                    ik.solver.rightArm.rotationWeight = 0f;
                }


                //Elbows
                if (leftElbowTracker != null)
                {
                    if (leftElbowTracker.gameObject.activeInHierarchy == true)
                    {
                        CalibrateElbow(settings, leftElbowTracker, ik.solver.leftArm, true);
                    }
                }
                if (rightElbowTracker != null)
                {
                    if (rightElbowTracker.gameObject.activeInHierarchy == true)
                    {
                        CalibrateElbow(settings, rightElbowTracker, ik.solver.rightArm, false);
                    }
                }

                //Knees
                if (leftKneeTracker != null)
                {
                    if (leftKneeTracker.gameObject.activeInHierarchy == true)
                    {
                        CalibrateKnee(settings, leftKneeTracker, ik.solver.leftLeg, true);
                    }
                }
                if (rightKneeTracker != null)
                {
                    if (rightKneeTracker.gameObject.activeInHierarchy == true)
                    {
                        CalibrateKnee(settings, rightKneeTracker, ik.solver.rightLeg, false);
                    }
                }

                // Legs
                if (leftFootTracker != null)
                {
                    if (leftFootTracker.gameObject.activeInHierarchy == true)
                    {
                        CalibrateLeg(settings, leftFootTracker, ik.solver.leftLeg, (ik.references.leftToes != null ? ik.references.leftToes : ik.references.leftFoot), ik.references.root.forward, true);
                    }
                }
                if (rightFootTracker != null)
                {
                    if (rightFootTracker.gameObject.activeInHierarchy == true)
                    {
                        CalibrateLeg(settings, rightFootTracker, ik.solver.rightLeg, (ik.references.rightToes != null ? ik.references.rightToes : ik.references.rightFoot), ik.references.root.forward, false);
                    }
                }

                // Root controller
                bool addRootController = bodyTracker != null || (leftFootTracker != null && rightFootTracker != null);
                var rootController = ik.references.root.GetComponent<RootController>();

                if (addRootController)
                {
                    if (rootController == null) rootController = ik.references.root.gameObject.AddComponent<RootController>();
                    rootController.Calibrate();
                }
                else
                {
                    if (rootController != null) GameObject.Destroy(rootController);
                }

                // Additional solver settings
                ik.solver.spine.minHeadHeight = 0f;
                //ik.solver.locomotion.weight = bodyTracker == null && leftFootTracker == null && rightFootTracker == null ? 1f : 0f;
                //Sort Locomotion
                ik.solver.locomotion.weight = useLocomotion ? 1f : 0f;

                // Fill in Calibration Data
                data.scale = ik.references.root.localScale.y;
                data.head = new CalibrationData.Target(ik.solver.spine.headTarget);
                data.pelvis = new CalibrationData.Target(ik.solver.spine.pelvisTarget);
                data.leftHand = new CalibrationData.Target(ik.solver.leftArm.target);
                data.rightHand = new CalibrationData.Target(ik.solver.rightArm.target);
                data.leftFoot = new CalibrationData.Target(ik.solver.leftLeg.target);
                data.rightFoot = new CalibrationData.Target(ik.solver.rightLeg.target);
                data.leftLegGoal = new CalibrationData.Target(ik.solver.leftLeg.bendGoal);
                data.rightLegGoal = new CalibrationData.Target(ik.solver.rightLeg.bendGoal);
                data.pelvisTargetRight = rootController != null ? rootController.pelvisTargetRight : Vector3.zero;
                data.pelvisPositionWeight = ik.solver.spine.pelvisPositionWeight;
                data.pelvisRotationWeight = ik.solver.spine.pelvisRotationWeight;

                return data;
            }
            else
            {
                Debug.Log("No VRIK script found, please add one to the character you are trying to animate");
                CalibrationData data = new CalibrationData();
                return data;
            }
        }

        private static void CalibrateLeg(Settings settings, Transform tracker, IKSolverVR.Leg leg, Transform lastBone, Vector3 rootForward, bool isLeft)
        {
            string name = isLeft ? "Left" : "Right";
            Transform target = leg.target == null ? (new GameObject(name + " Foot Target")).transform : leg.target;

            // Space of the tracker heading
            Quaternion trackerSpace = tracker.rotation * Quaternion.LookRotation(settings.footTrackerForward, settings.footTrackerUp);
            Vector3 f = trackerSpace * Vector3.forward;
            f.y = 0f;
            trackerSpace = Quaternion.LookRotation(f);

            // Target position
            float inwardOffset = isLeft ? settings.footInwardOffset : -settings.footInwardOffset;
            target.position = tracker.position + trackerSpace * new Vector3(inwardOffset, 0f, settings.footForwardOffset);
            target.position = new Vector3(target.position.x, lastBone.position.y, target.position.z);

            // Target rotation
            target.rotation = lastBone.rotation;

            // Rotate target forward towards tracker forward
            Vector3 footForward = AxisTools.GetAxisVectorToDirection(lastBone, rootForward);
            if (Vector3.Dot(lastBone.rotation * footForward, rootForward) < 0f) footForward = -footForward;
            Vector3 fLocal = Quaternion.Inverse(Quaternion.LookRotation(target.rotation * footForward)) * f;
            float angle = Mathf.Atan2(fLocal.x, fLocal.z) * Mathf.Rad2Deg;
            float headingOffset = isLeft ? settings.footHeadingOffset : -settings.footHeadingOffset;
            target.rotation = Quaternion.AngleAxis(angle + headingOffset, Vector3.up) * target.rotation;

            target.parent = tracker;
            leg.target = target;

            leg.positionWeight = 1f;
            leg.rotationWeight = 1f;
        }
        private static void CalibrateKnee(Settings settings, Transform tracker, IKSolverVR.Leg leg, bool isLeft)
        {
            leg.bendGoal = tracker;
            leg.bendGoalWeight = 1f;
        }
        private static void CalibrateElbow(Settings settings, Transform tracker, IKSolverVR.Arm arm, bool isLeft)
        {
            arm.bendGoal = tracker;
            arm.bendGoalWeight = 1f;
        }

        /// <summary>
        /// When VRIK is calibrated by calibration settings, will store CalibrationData that can be used to set up another character with the exact same calibration.
        /// </summary>
        [System.Serializable]
        public class CalibrationData
        {
            [System.Serializable]
            public class Target
            {
                public bool used;
                public Vector3 localPosition;
                public Quaternion localRotation;

                public Target(Transform t)
                {
                    this.used = t != null;
                    if (!this.used) return;

                    this.localPosition = t.localPosition;
                    this.localRotation = t.localRotation;
                }

                public void SetTo(Transform t)
                {
                    if (!used) return;
                    t.localPosition = localPosition;
                    t.localRotation = localRotation;
                }
            }

            public float scale;
            public Target head, leftHand, rightHand, pelvis, leftFoot, rightFoot, leftLegGoal, rightLegGoal;
            public Vector3 pelvisTargetRight;
            public float pelvisPositionWeight;
            public float pelvisRotationWeight;
        }

        /// <summary>
        /// Calibrates VRIK to the specified trackers using CalibrationData from a previous calibration. Requires this character's bone orientations to match with the character's that was used in the previous calibration.
        /// </summary>
        /// <param name="ik">Reference to the VRIK component.</param>
        /// <param name="data">Use calibration data from a previous calibration.</param>
        /// <param name="headTracker">The HMD.</param>
        /// <param name="bodyTracker">(Optional) A tracker placed anywhere on the body of the player, preferrably close to the pelvis, on the belt area.</param>
        /// <param name="chestTracker">(Optional) A tracker placed on the chest of the player.</param>
        /// <param name="leftHandTracker">(Optional) A tracker or hand controller device placed anywhere on or in the player's left hand.</param>
        /// <param name="rightHandTracker">(Optional) A tracker or hand controller device placed anywhere on or in the player's right hand.</param>
        /// <param name="leftElbowtracker">(Optional) A tracker or hand controller device placed anywhere on the player's left elbow.</param>
        /// <param name="rightElbowtracker">(Optional) A tracker or hand controller device placed anywhere on the player's right elbow.</param>
        /// <param name="leftKneeTracker">(Optional) A tracker placed anywhere on the knee of the player's left leg.</param>
        /// <param name="rightKneeTracker">(Optional) A tracker placed anywhere on the knee of the player's left leg.</param>
        /// <param name="leftFootTracker">(Optional) A tracker placed anywhere on the ankle or toes of the player's left leg.</param>
        /// <param name="rightFootTracker">(Optional) A tracker placed anywhere on the ankle or toes of the player's right leg.</param>
        public static void Calibrate(VRIK ik, CalibrationData data, Transform headTracker, Transform bodyTracker = null, Transform chestTracker = null, Transform leftHandTracker = null, Transform rightHandTracker = null, Transform leftElbowTracker = null, Transform rightElbowTracker = null, Transform leftKneeTracker = null, Transform rightKneeTracker = null, Transform leftFootTracker = null, Transform rightFootTracker = null)
        {
            if (!ik.solver.initiated)
            {
                Debug.LogError("Can not calibrate before VRIK has initiated.");
                return;
            }

            if (headTracker == null)
            {
                Debug.LogError("Can not calibrate VRIK without the head tracker.");
                return;
            }

            ik.solver.FixTransforms();

            // Head
            Transform headTarget = ik.solver.spine.headTarget == null ? (new GameObject("Head Target")).transform : ik.solver.spine.headTarget;
            headTarget.parent = headTracker;
            data.head.SetTo(headTarget);
            ik.solver.spine.headTarget = headTarget;

            // Size
            ik.references.root.localScale = data.scale * Vector3.one;

            // Body
            if (bodyTracker != null)
            {
                Transform pelvisTarget = ik.solver.spine.pelvisTarget == null ? (new GameObject("Pelvis Target")).transform : ik.solver.spine.pelvisTarget;
                pelvisTarget.parent = bodyTracker;
                data.pelvis.SetTo(pelvisTarget);
                ik.solver.spine.pelvisTarget = pelvisTarget;

                ik.solver.spine.pelvisPositionWeight = data.pelvisPositionWeight;
                ik.solver.spine.pelvisRotationWeight = data.pelvisRotationWeight;

                ik.solver.plantFeet = true;
                ik.solver.spine.maxRootAngle = 180f;
            }
            else if (leftFootTracker != null && rightFootTracker != null)
            {
                ik.solver.spine.maxRootAngle = 0f;
            }

            // Chest
            if (chestTracker != null)
            {
                ik.solver.spine.chestGoal = chestTracker;
                ik.solver.spine.chestGoalWeight = 1f;
            }

            // Left Hand
            if (leftHandTracker != null)
            {
                Transform leftHandTarget = ik.solver.leftArm.target == null ? (new GameObject("Left Hand Target")).transform : ik.solver.leftArm.target;
                leftHandTarget.parent = leftHandTracker;
                data.leftHand.SetTo(leftHandTarget);
                ik.solver.leftArm.target = leftHandTarget;
                ik.solver.leftArm.positionWeight = 1f;
                ik.solver.leftArm.rotationWeight = 1f;
            }
            else
            {
                ik.solver.leftArm.positionWeight = 0f;
                ik.solver.leftArm.rotationWeight = 0f;
            }

            // Right Hand
            if (rightHandTracker != null)
            {
                Transform rightHandTarget = ik.solver.rightArm.target == null ? (new GameObject("Right Hand Target")).transform : ik.solver.rightArm.target;
                rightHandTarget.parent = rightHandTracker;
                data.rightHand.SetTo(rightHandTarget);
                ik.solver.rightArm.target = rightHandTarget;
                ik.solver.rightArm.positionWeight = 1f;
                ik.solver.rightArm.rotationWeight = 1f;
            }
            else
            {
                ik.solver.rightArm.positionWeight = 0f;
                ik.solver.rightArm.rotationWeight = 0f;
            }

            //Elbows
            if (leftElbowTracker != null) CalibrateElbow(data, leftElbowTracker, ik.solver.leftArm, true);
            if (rightElbowTracker != null) CalibrateElbow(data, rightElbowTracker, ik.solver.rightArm, false);

            //Knees
            if (leftKneeTracker != null) CalibrateKnee(data, leftKneeTracker, ik.solver.leftLeg, true);
            if (rightKneeTracker != null) CalibrateKnee(data, rightKneeTracker, ik.solver.rightLeg, false);

            // Legs
            if (leftFootTracker != null) CalibrateLeg(data, leftFootTracker, ik.solver.leftLeg, (ik.references.leftToes != null ? ik.references.leftToes : ik.references.leftFoot), ik.references.root.forward, true);
            if (rightFootTracker != null) CalibrateLeg(data, rightFootTracker, ik.solver.rightLeg, (ik.references.rightToes != null ? ik.references.rightToes : ik.references.rightFoot), ik.references.root.forward, false);


            // Root controller
            bool addRootController = bodyTracker != null || (leftFootTracker != null && rightFootTracker != null);
            var rootController = ik.references.root.GetComponent<RootController>();

            if (addRootController)
            {
                if (rootController == null) rootController = ik.references.root.gameObject.AddComponent<RootController>();
                rootController.Calibrate(data);
            }
            else
            {
                if (rootController != null) GameObject.Destroy(rootController);
            }

            // Additional solver settings
            ik.solver.spine.minHeadHeight = 0f;
            ik.solver.locomotion.weight = bodyTracker == null && leftFootTracker == null && rightFootTracker == null ? 1f : 0f;
        }

        private static void CalibrateLeg(CalibrationData data, Transform tracker, IKSolverVR.Leg leg, Transform lastBone, Vector3 rootForward, bool isLeft)
        {
            string name = isLeft ? "Left" : "Right";
            Transform target = leg.target == null ? (new GameObject(name + " Foot Target")).transform : leg.target;

            target.parent = tracker;

            if (isLeft) data.leftFoot.SetTo(target);
            else data.rightFoot.SetTo(target);

            leg.target = target;

            leg.positionWeight = 1f;
            leg.rotationWeight = 1f;

        }
        private static void CalibrateElbow(CalibrationData data, Transform tracker, IKSolverVR.Arm arm, bool isLeft)
        {
            arm.bendGoal = tracker;
            arm.bendGoalWeight = 1f;
        }
        private static void CalibrateKnee(CalibrationData data, Transform tracker, IKSolverVR.Leg leg, bool isLeft)
        {
            leg.bendGoal = tracker;
            leg.bendGoalWeight = 1f;
        }
#endif
    }
}
