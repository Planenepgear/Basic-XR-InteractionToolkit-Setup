using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.FinalIK
{

    public class VRIKInteractableObject : MonoBehaviour
    {

        public HandPose[] handPoses { get; private set; }
        protected bool isPickedUp { get; private set; }

        protected HandPose currentPose { get; private set; }
        protected HandIKTarget currentIKTarget { get; private set; }

        private void Start()
        {
            handPoses = GetComponentsInChildren<HandPose>();
        }

        public HandPose GetPose(Handedness handedness, HandIKTarget ikTarget)
        {
            foreach (HandPose pose in handPoses)
            {
                if (pose.handedness == handedness) pose.OnPickUp(ikTarget.transform);
            }

            return GetClosestPose(handedness, ikTarget.transform);
        }

        private HandPose GetClosestPose(Handedness handedness, Transform target)
        {

            float minError = Mathf.Infinity;
            int closest = -1;

            for (int i = 0; i < handPoses.Length; i++)
            {
                if (handPoses[i].handedness == handedness)
                {
                    float angle = Quaternion.Angle(handPoses[i].transform.rotation, target.rotation);

                    if (angle < minError)
                    {
                        closest = i;
                        minError = angle;
                    }
                }
            }

            if (closest == -1) return null;
            return handPoses[closest];
        }

        public virtual void PickUp(HandPose pose, HandIKTarget ikTarget)
        {
            /*
            isPickedUp = true;
            pose.OnPickUp(ikTarget.transform);
            currentPose = pose;
            currentIKTarget = ikTarget;
            */
        }

        public virtual void Drop()
        {
            /*
            isPickedUp = false;
            currentPose = null;
            currentIKTarget = null;
            */
        }
    }
}
