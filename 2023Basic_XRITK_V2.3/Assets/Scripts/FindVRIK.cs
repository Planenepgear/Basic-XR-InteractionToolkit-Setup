using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RootMotion.FinalIK;

public class FindVRIK : MonoBehaviour
{
    [SerializeField] bool isLeft;
    [SerializeField] bool isRight;
    [SerializeField] bool isController;

    [SerializeField] Transform aim;

    VRIK _vrik;

    void Awake()
    {
        _vrik = transform.parent.parent.GetComponent<VRIK>();

        //if (isController)
        //{
        //    _vrik = transform.parent.parent.GetComponent<VRIK>();
        //}
        //else
        //{
        //    _vrik = transform.parent.parent.parent.GetComponent<VRIK>();
        //}
    }

    private void OnEnable()
    {
        if (isController)
        {
            if (isLeft)
            {
                _vrik.solver.leftArm.target = aim;
            }
            else if (isRight)
            {
                _vrik.solver.rightArm.target = aim;
            }
        }
        else
        {
            if (isLeft)
            {
                _vrik.solver.leftArm.target = aim;
            }
            else if (isRight)
            {
                _vrik.solver.rightArm.target = aim;
            }
        }
    }
}
