using R1Tools.Core;
using UnityEngine;

namespace RootMotion.FinalIK
{

    public enum PoseLock
    {
        Locked = 0,
        LockedInXZ,
        LockedInYZ,
        LockedInXY,
        Unlocked
    }

    [System.Serializable]
    public struct PoseLocks
    {
        public PoseLock linear;
        public PoseLock angular;
        public bool updateLinear;
        public bool updateAngular;
        public float linearMinLimit;
        public float linearMaxLimit;

        public bool isUnlocked
        {
            get
            {
                return linear == PoseLock.Unlocked && angular == PoseLock.Unlocked;
            }
        }

        public Vector3 linearAxis
        {
            get
            {
                return PoseLockToAxis(linear);
            }
        }

        public Vector3 angularAxis
        {
            get
            {
                return PoseLockToAxis(angular);
            }
        }

        public static Vector3 PoseLockToAxis(PoseLock poseLock)
        {
            switch (poseLock)
            {
                case PoseLock.LockedInXZ: return Vector3.up;
                case PoseLock.LockedInYZ: return Vector3.right;
                case PoseLock.LockedInXY: return Vector3.forward;
                case PoseLock.Locked: return Vector3.zero;
                default: return Vector3.one;
            }
        }
    }

    [System.Serializable]
    public enum Handedness
    {
        Left,
        Right
    }

    public class HandPose : MonoBehaviour
    {
        public Handedness handedness;
        public Transform[] bones { get; private set; }
        public Transform pivot;

        public PoseLocks poseLocks;

        private Vector3 defaultPivotLocalPos;
        private Quaternion defaultPivotLocalRot = Quaternion.identity;


        void Awake()
        {
            var newPivot = new GameObject("Pivot");
            newPivot.transform.position = pivot.transform.position;
            newPivot.transform.rotation = pivot.transform.rotation;
            newPivot.transform.parent = pivot;
            pivot = newPivot.transform;
            transform.parent = pivot;

            bones = (Transform[])transform.GetComponentsInChildren<Transform>();

            defaultPivotLocalPos = pivot.localPosition;
            defaultPivotLocalRot = pivot.localRotation;
        }

        public void OnPickUp(Transform ikTarget)
        {
            UpdateAngular(ikTarget);
            UpdateLinear(ikTarget);
        }

        public void OnUpdate(Transform ikTarget)
        {
            if (poseLocks.updateAngular) UpdateAngular(ikTarget);
            if (poseLocks.updateLinear) UpdateLinear(ikTarget);
        }


        private void UpdateAngular(Transform ikTarget)
        {
#if VRIK_PRESENT
            switch (poseLocks.angular)
            {
                case PoseLock.Locked:
                    pivot.localRotation = defaultPivotLocalRot;
                    break;
                case PoseLock.Unlocked:
                    pivot.localRotation = defaultPivotLocalRot;
                    pivot.rotation = R1QuaTools.FromToRotation(transform.rotation, ikTarget.rotation) * pivot.rotation;
                    break;
                default:
                    pivot.localRotation = defaultPivotLocalRot;

                    Vector3 axis = poseLocks.angularAxis;
                    Vector3 x = pivot.rotation * axis;
                    pivot.rotation = R1QuaTools.FromToRotation(transform.rotation, ikTarget.rotation) * pivot.rotation;

                    pivot.rotation = Quaternion.FromToRotation(pivot.rotation * axis, x) * pivot.rotation;
                    break;
            }
#endif
        }

        private void UpdateLinear(Transform ikTarget)
        {
            switch (poseLocks.linear)
            {
                case PoseLock.Locked:
                    pivot.localPosition = defaultPivotLocalPos;

                    break;
                case PoseLock.Unlocked:
                    pivot.position += ikTarget.position - transform.position;
                    break;
                default:
                    pivot.localPosition = defaultPivotLocalPos;

                    Vector3 axis = poseLocks.linearAxis;
                    Vector3 delta = Vector3.Project(ikTarget.position - transform.position, pivot.rotation * axis);
                    pivot.position += delta;

                    Vector3 defaultToCurrent = pivot.localPosition - defaultPivotLocalPos;
                    float dot = Vector3.Dot(defaultToCurrent, axis);
                    float limit = dot > 0f ? poseLocks.linearMaxLimit : poseLocks.linearMinLimit;
                    defaultToCurrent = Vector3.ClampMagnitude(defaultToCurrent, limit);
                    pivot.localPosition = defaultPivotLocalPos + defaultToCurrent;
                    break;
            }
        }
    }
}
