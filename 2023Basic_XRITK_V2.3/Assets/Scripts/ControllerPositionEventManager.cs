using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerPositionEventManager : MonoBehaviour
{
    public delegate void controllerPositionPass();
    public static event controllerPositionPass controllerPositionGet;

    [Header("Hand Tracking")]
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;

    [Header("Motion Controllers")]
    [SerializeField] GameObject LeftController;
    [SerializeField] GameObject RightController;


    public void ControllerOff()
    {
        leftHand.SendMessage("SwitchToHand");
        rightHand.SendMessage("SwitchToHand");
    }
}
