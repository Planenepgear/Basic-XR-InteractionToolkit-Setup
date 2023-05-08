using UnityEngine;
#if VRIK_PRESENT
using RootMotion.FinalIK;
#endif

namespace R1Tools.Core
{
    public class AvatarModel : MonoBehaviour
    {
#if VRIK_PRESENT
    [Tooltip("The Transform representing the position and rotation of the 'CenterEyeAnchor'[Oculus] or 'Camera (head)'[SteamVR] relative to this model.")]
    public Transform centerEyeAnchor;

    [Tooltip("The Transform representing the position and rotation of the 'LeftHandAnchor'[Oculus] or 'Controller (left)'[SteamVR] relative to this model.")]
    public Transform leftHandAnchor;

    [Tooltip("The Transform representing the position and rotation of the 'RightHandAnchor'[Oculus] or 'Controller (right)'[SteamVR] relative to this model.")]
    public Transform rightHandAnchor;

    public Vector3 headLocalPosition { get; private set; }
    public Vector3 leftHandLocalPosition { get; private set; }
    public Vector3 rightHandLocalPosition { get; private set; }
    public Quaternion headLocalRotation { get; private set; }
    public Quaternion leftHandLocalRotation { get; private set; }
    public Quaternion rightHandLocalRotation { get; private set; }
    public Transform centerEyeProxy { get; private set; }
    public Transform leftHandProxy { get; private set; }
    public Transform rightHandProxy { get; private set; }

    private VRIK ik;

    void Awake()
    {
        ik = GetComponent<VRIK>();
        Animator animator = GetComponent<Animator>();

        // Find the head and hand bones
        Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
        Transform leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

        // Positions of the bones relative to their anchors
        headLocalPosition = centerEyeAnchor.InverseTransformPoint(head.position);
        leftHandLocalPosition = leftHandAnchor.InverseTransformPoint(leftHand.position);
        rightHandLocalPosition = rightHandAnchor.InverseTransformPoint(rightHand.position);

        // Rotations of the bones relative to their anchors
        headLocalRotation = Quaternion.Inverse(centerEyeAnchor.rotation) * head.rotation;
        leftHandLocalRotation = Quaternion.Inverse(leftHandAnchor.rotation) * leftHand.rotation;
        rightHandLocalRotation = Quaternion.Inverse(rightHandAnchor.rotation) * rightHand.rotation;

        centerEyeProxy = new GameObject("Center Eye Proxy").transform;
        centerEyeProxy.position = centerEyeAnchor.position;
        centerEyeProxy.rotation = centerEyeAnchor.rotation;

        leftHandProxy = new GameObject("Left Hand Proxy").transform;
        leftHandProxy.position = leftHandAnchor.position;
        leftHandProxy.rotation = leftHandAnchor.rotation;

        rightHandProxy = new GameObject("Right Hand Proxy").transform;
        rightHandProxy.position = rightHandAnchor.position;
        rightHandProxy.rotation = rightHandAnchor.rotation;

        CreateIKTargets();

        // We have calculated all the localPositions/Rotations, so we don't need those gameobjects anymore.
        Destroy(centerEyeAnchor.parent.gameObject);
    }

    /// <summary>
    /// Creates the IK targets for VRIK based on the localPositions/Rotations calculated above, parents those IK targets to the proxies.
    /// </summary>
    public void CreateIKTargets(Transform centerEyeProxy, Transform leftHandProxy, Transform rightHandProxy)
    {
        // Build and assign VRIK targets
        var headIKTarget = ik.solver.spine.headTarget;
        if (headIKTarget == null) headIKTarget = new GameObject("Head IK Target").transform;
        ik.solver.spine.headTarget = headIKTarget;
        ik.solver.spine.headTarget.parent = centerEyeProxy;
        ik.solver.spine.headTarget.localPosition = headLocalPosition;
        ik.solver.spine.headTarget.localRotation = headLocalRotation;

        var leftHandIKTarget = ik.solver.leftArm.target;
        if (leftHandIKTarget == null) leftHandIKTarget = new GameObject("Left Hand IK Target").transform;
        ik.solver.leftArm.target = leftHandIKTarget;
        ik.solver.leftArm.target.parent = leftHandProxy;
        ik.solver.leftArm.target.localPosition = leftHandLocalPosition;
        ik.solver.leftArm.target.localRotation = leftHandLocalRotation;

        var rightHandIKTarget = ik.solver.rightArm.target;
        if (rightHandIKTarget == null) rightHandIKTarget = new GameObject("Right Hand IK Target").transform;
        ik.solver.rightArm.target = rightHandIKTarget;
        ik.solver.rightArm.target.parent = rightHandProxy;
        ik.solver.rightArm.target.localPosition = rightHandLocalPosition;
        ik.solver.rightArm.target.localRotation = rightHandLocalRotation;
    }

    /// <summary>
    /// Creates the IK targets, parents them to the proxies that have already been created in Awake.
    /// </summary>
    public void CreateIKTargets()
    {
        CreateIKTargets(centerEyeProxy, leftHandProxy, rightHandProxy);
    }
#endif
    }
}
