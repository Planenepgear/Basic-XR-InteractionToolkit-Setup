using System.Collections;
using UnityEngine;

namespace R1Tools.Xrit 
{
#if UNITY_XRIT_AVAILABLE
using UnityEngine.XR.Interaction.Toolkit;
#endif

#if UNITY_XRIT_AVAILABLE
public class OffsetGrabbableXrInteractable : XRGrabInteractable
#else

public class OffsetGrabbableXrInteractable : MonoBehaviour

#endif

{
#if UNITY_XRIT_AVAILABLE
    private Vector3 initialAttachLocalPos;
    private Quaternion initialAttachLocalRot;

    private void Start()
    {
        if (!attachTransform)
        {
            GameObject grab = new GameObject("Grab Offset");
            grab.transform.SetParent(transform, false);
            attachTransform = grab.transform;
        }

        initialAttachLocalPos = attachTransform.localPosition;
        initialAttachLocalRot = attachTransform.localRotation;
    }

    private IEnumerator waitThenSetMovementType()
    {
        yield return new WaitForSecondsRealtime(0.1f);

        movementType = MovementType.Instantaneous;

        yield return null;
    }

    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor)
        {
            //attachTransform.position = interactor.attachTransform.position;
            //attachTransform.rotation = interactor.attachTransform.rotation;

            attachTransform.position = args.interactorObject.transform.position;
            attachTransform.rotation = args.interactorObject.transform.rotation;
        }
        else
        {
            // attachTransform.localPosition = initialAttachLocalPos;
            //attachTransform.localRotation = initialAttachLocalRot;
        }
        //movementType = MovementType.Instantaneous;
        StartCoroutine(waitThenSetMovementType());
        base.OnSelectEntering(args);
        Grab();
    }

    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        //movementType = MovementType.Kinematic;
        base.OnSelectExiting(args);
        Drop();
    }

#else
    private void Start()
    {
        //Do nothing here
        Debug.Log("no XRIT");
    }
#endif
}
}