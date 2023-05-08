#if UNITY_EDITOR
using R1Tools.Core;
using UnityEngine;

namespace R1Tools.Mecanim
{
    public class MecanimIKControl : MonoBehaviour
    {
        [Tooltip("Use this to ignore the waist tracker on snapping")]
        public bool ignoreWaist = false;
        [HideInInspector]
        protected Animator targetAnimator;
        [HideInInspector]
        public TrackerAnimTool trackerScript;

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

        private Transform characterRoot;
        private Transform head;
        private Transform chest;
        private Transform hips;
        private Transform leftHand;
        private Transform rightHand;
        private Transform leftElbow;
        private Transform rightElbow;
        private Transform leftKnee;
        private Transform rightKnee;
        private Transform leftFoot;
        private Transform rightFoot;
        [HideInInspector]
        public Transform leftLowerArm;
        [HideInInspector]
        public Transform leftUpperArm;
        [HideInInspector]
        public Transform rightLowerArm;
        [HideInInspector]
        public Transform rightUpperArm;

        private MecanimIKLinker linkScript;
        [HideInInspector]
        public bool ikActive = false;

        private Vector3 oldBodyPosition;
        private Quaternion oldBodyRotation;

#if !VRIK_PRESENT

        public void setupIK(Animator targetAnim)
        {
            targetAnimator = targetAnim;
            characterRoot = targetAnimator.transform;
            linkScript = targetAnim.gameObject.AddComponent<MecanimIKLinker>();
            linkScript.controller = this;
            calibrate();
        }

        public void calibrate()
        {
            //Hide all tracker meshes
            trackerScript.trackerVisibility(false);
            trackerScript.onCalibrate();

            //Get all bone transform targets
            head = targetAnimator.GetBoneTransform(HumanBodyBones.Head);
            chest = targetAnimator.GetBoneTransform(HumanBodyBones.Chest);
            hips = targetAnimator.GetBoneTransform(HumanBodyBones.Hips);
            leftElbow = targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            rightElbow = targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            leftHand = targetAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
            rightHand = targetAnimator.GetBoneTransform(HumanBodyBones.RightHand);
            leftKnee = targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
            rightKnee = targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
            leftFoot = targetAnimator.GetBoneTransform(HumanBodyBones.LeftFoot);
            rightFoot = targetAnimator.GetBoneTransform(HumanBodyBones.RightFoot);
            leftLowerArm = targetAnimator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            leftUpperArm = targetAnimator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            rightLowerArm = targetAnimator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            rightUpperArm = targetAnimator.GetBoneTransform(HumanBodyBones.RightUpperArm);
            //Now activate IK
            ikActive = true;
        }

        private void LateUpdate()
        {
            if (ikActive == true)
            {
                //Head
                head.rotation = headTracker.rotation;
                head.position = headTracker.position;

                //Chest
                if (chestTracker.isTracked == true)
                {
                    chest.position = chestTracker.offset.position;
                    chest.rotation = chestTracker.offset.rotation;
                }
            }
        }

        //a callback for calculating IK
        public void ikUpdate()
        {
            //if the IK is active, set the position and rotation directly to the goal. 
            if (ikActive == true)
            {
                //Waist
                if (bodyTracker.isTracked == true)
                {
                    oldBodyPosition = characterRoot.position;
                    characterRoot.position = bodyTracker.offset.position - hips.localPosition;
                    oldBodyRotation = characterRoot.rotation;
                    characterRoot.rotation = Quaternion.Euler(oldBodyRotation.eulerAngles.x, bodyTracker.offset.rotation.eulerAngles.y, oldBodyRotation.eulerAngles.z);
                    // offset to avoid showing mouth parts in view
                    characterRoot.position += gameObject.transform.forward * -0.1f;

                    //For crouching
                    hips.position = bodyTracker.offset.position;
                }

                //Arms
                if (leftElbowTracker.isTracked == true)
                {
                    targetAnimator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
                    targetAnimator.SetIKHintPosition(AvatarIKHint.LeftElbow, leftElbowTracker.offset.position);
                }
                if (rightElbowTracker.isTracked == true)
                {
                    targetAnimator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
                    targetAnimator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElbowTracker.offset.position);
                }

                //Hands

                //Left Hand
                targetAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                targetAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                targetAnimator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTracker.position);
                targetAnimator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTracker.rotation);

                //Right Hand
                targetAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                targetAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                targetAnimator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTracker.position);
                targetAnimator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTracker.rotation);

                //Knees
                if (leftKneeTracker.isTracked == true)
                {
                    targetAnimator.SetIKHintPositionWeight(AvatarIKHint.LeftKnee, 1);
                    targetAnimator.SetIKHintPosition(AvatarIKHint.LeftKnee, leftKneeTracker.offset.position);
                }
                if (rightKneeTracker.isTracked == true)
                {
                    targetAnimator.SetIKHintPositionWeight(AvatarIKHint.RightKnee, 1);
                    targetAnimator.SetIKHintPosition(AvatarIKHint.RightKnee, rightKneeTracker.offset.position);
                }

                //Feet
                if (leftFootTracker.isTracked == true)
                {
                    targetAnimator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    targetAnimator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    targetAnimator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTracker.offset.position);
                    targetAnimator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootTracker.offset.rotation);
                }
                if (rightFootTracker.isTracked == true)
                {
                    targetAnimator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    targetAnimator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    targetAnimator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTracker.offset.position);
                    targetAnimator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootTracker.offset.rotation);
                }
            }
            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                targetAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                targetAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                targetAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                targetAnimator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }
#endif
    }
}
#endif