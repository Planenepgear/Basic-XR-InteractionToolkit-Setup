using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK
{

    public class HandModel : MonoBehaviour
    {

        public bool animated;
        [Range(0f, 1f)] public float weight = 1f;
        [Range(0f, 1f)] public float smoothTime = 0.1f;
        [HideInInspector] public HandPose currentPose;

        public Transform[] bones { get; private set; }

        private Quaternion[] defaultLocalRotations = new Quaternion[0];
        private Quaternion[] fromRotations = new Quaternion[0];
        private Quaternion[] lastLocalRotations = new Quaternion[0];

        private float lerpWeight;
        private float lerpV;
        private HandPose lastPose;

        void Start()
        {
            bones = (Transform[])transform.GetComponentsInChildren<Transform>();
            defaultLocalRotations = new Quaternion[bones.Length];
            fromRotations = new Quaternion[bones.Length];
            lastLocalRotations = new Quaternion[bones.Length];

            for (int i = 1; i < bones.Length; i++)
            {
                defaultLocalRotations[i] = bones[i].localRotation;
                fromRotations[i] = bones[i].localRotation;
                lastLocalRotations[i] = bones[i].localRotation;
            }
        }

        void LateUpdate()
        {
            if (weight <= 0f) return;

            if (!animated)
            {
                for (int i = 1; i < bones.Length; i++)
                {
                    bones[i].localRotation = defaultLocalRotations[i];
                }
            }

            if (currentPose != lastPose)
            {
                for (int i = 1; i < bones.Length; i++)
                {
                    fromRotations[i] = lastLocalRotations[i];
                }

                lerpWeight = 0f;
                lastPose = currentPose;
            }

            lerpWeight = Mathf.SmoothDamp(lerpWeight, 1f, ref lerpV, smoothTime);

            for (int i = 1; i < bones.Length; i++)
            {
                Quaternion localR = currentPose != null ? Quaternion.Lerp(fromRotations[i], currentPose.bones[i].localRotation, lerpWeight) : Quaternion.Lerp(fromRotations[i], defaultLocalRotations[i], lerpWeight);

                bones[i].localRotation = weight >= 1f ? localR : Quaternion.Lerp(fromRotations[i], localR, weight);

                lastLocalRotations[i] = bones[i].localRotation;
            }
        }
    }
}