using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.Hands;

public class FindManager : MonoBehaviour
{
    [SerializeField] bool isLeft;
    [SerializeField] bool isRight;

    HandsAndControllersManagerV2 controllersManager;

    void Awake()
    {
        controllersManager = transform.parent.parent.parent.parent.parent.GetComponent<HandsAndControllersManagerV2>();
    }

    void Start()
    {
        if (isLeft)
        {
            controllersManager.TrackedHandPosL = gameObject.transform;
        }
        else
        {
            controllersManager.TrackedHandPosR = gameObject.transform;
        }
    }
}
