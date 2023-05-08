#if VRIK_PRESENT
#endif
using UnityEngine;

namespace RootMotion.FinalIK
{

    public class VRIKInteractionSystem : MonoBehaviour
    {
#if VRIK_PRESENT
        public HandPose[] handPoses { get; private set; }

        [Header("VRIK")]

        public VRIK ik;

        private HandModel handModelLeft;
        private HandIKTarget handIKTargetLeft;
        private HandModel handModelRight;
        private HandIKTarget handIKTargetRight;

        private VRIKInteractableObject objLeft, objRight;
        public HandPose handPoseLeft, handPoseRight;
        private bool initiated;

        protected virtual void Update()
        {
            if (initiated) return;
            if (!ik.solver.initiated) return;
            if (ik.solver.spine.headTarget == null) return;
            if (ik.solver.leftArm.target == null) return;
            if (ik.solver.rightArm.target == null) return;

            handModelLeft = ik.references.leftHand.gameObject.AddComponent<HandModel>();
            handModelRight = ik.references.rightHand.gameObject.AddComponent<HandModel>();

            handIKTargetLeft = ik.solver.leftArm.target.gameObject.AddComponent<HandIKTarget>();
            handIKTargetRight = ik.solver.rightArm.target.gameObject.AddComponent<HandIKTarget>();

            initiated = true;
        }


        public void StartGrab(Transform target, Handedness handedness)
        {
            if (!initiated)
            {
                Debug.LogError("Attempting to start grabbing before VRIKInteractionSystem has initiated.");
                return;
            }

            var obj = target.GetComponent<VRIKInteractableObject>();

            if (obj == null)
            {
                Debug.LogError("No VRIKInteractableObject found on " + target.name);
                return;
            }

            var ikTarget = handedness == Handedness.Left ? handIKTargetLeft : handIKTargetRight;
            var handModel = handedness == Handedness.Left ? handModelLeft : handModelRight;

            var pose = obj.GetPose(handedness, ikTarget);

            if (pose == null)
            {
                Debug.LogError("No HandPose with handedness: " + handedness.ToString() + " found on " + target.name);
                return;
            }

            if (handedness == Handedness.Left)
            {
                objLeft = obj;
                //handPoseLeft = pose;
            }
            else
            {
                objRight = obj;
                //handPoseRight = pose;
            }


            //obj.PickUp(pose, ikTarget);
            //handModel.currentPose = pose;
            //ikTarget.currentPose = pose;
            handModel.currentPose = pose;
            ikTarget.currentPose = pose;
        }

        public void Ungrab(Handedness handedness)
        {
            if (!initiated)
            {
                Debug.LogError("Attempting to start grabbing before VRIKInteractionSystem has initiated.");
                return;
            }

            var ikTarget = handedness == Handedness.Left ? handIKTargetLeft : handIKTargetRight;
            var handModel = handedness == Handedness.Left ? handModelLeft : handModelRight;

            handModel.currentPose = null;
            ikTarget.currentPose = null;

            if (handedness == Handedness.Left)
            {
                if (objLeft != null)
                {
                    objLeft.Drop();
                    objLeft = null;
                }
                if (handPoseLeft != null) handPoseLeft = null;
            }
            else
            {
                if (objRight != null)
                {
                    objRight.Drop();
                    objRight = null;
                }
                if (handPoseRight != null) handPoseRight = null;
            }
        }

        protected virtual void LateUpdate()
        {
            if (!initiated) return;

            if (handPoseLeft != null) handPoseLeft.OnUpdate(handIKTargetLeft.transform);
            if (handPoseRight != null) handPoseRight.OnUpdate(handIKTargetRight.transform);
            handModelLeft.currentPose = handPoseLeft;
            handIKTargetLeft.currentPose = handPoseLeft;
            handModelRight.currentPose = handPoseRight;
            handIKTargetRight.currentPose = handPoseRight;

        }
#endif
    }
}
