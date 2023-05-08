using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RootMotion.FinalIK
{

    public class HandIKTarget : MonoBehaviour
    {

        [Range(0f, 1f)] public float smoothTime = 0.1f;
        [HideInInspector] public HandPose currentPose;

        private float weight;
        private float weightV;

        private Vector3 defaultLocalPosition;
        private Quaternion defaultLocalRotation = Quaternion.identity;

        private void Start()
        {
            defaultLocalPosition = transform.localPosition;
            defaultLocalRotation = transform.localRotation;
        }

        public void ResetTransform()
        {
            transform.localPosition = defaultLocalPosition;
            transform.localRotation = defaultLocalRotation;
        }

        void LateUpdate()
        {
            if (smoothTime > 0f)
            {
                float speed = 1f / smoothTime;
                transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPosition, Time.deltaTime * speed);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, defaultLocalRotation, Time.deltaTime * speed);
            }
            else
            {
                ResetTransform();
            }

            if (currentPose != null)
            {

                weight = Mathf.SmoothDamp(weight, 1f, ref weightV, smoothTime);
                if (weight > 0.99f) weight = 1f;
                //weight = 1f;
                transform.localPosition = defaultLocalPosition;
                transform.localRotation = defaultLocalRotation;

                transform.position = Vector3.Lerp(transform.position, currentPose.transform.position, weight);
                transform.rotation = Quaternion.Slerp(transform.rotation, currentPose.transform.rotation, weight);
                //transform.rotation = currentPose.transform.rotation;

            }
            else
            {
                weight = 0f;
            }
        }

    }
}
