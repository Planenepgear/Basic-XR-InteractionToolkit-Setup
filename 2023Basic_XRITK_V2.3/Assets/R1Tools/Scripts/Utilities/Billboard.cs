using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R1Tools.Core 
{
public class Billboard : MonoBehaviour
{
    private Transform maincamera;

    // Start is called before the first frame update
    void Start()
    {
        maincamera = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.forward = maincamera.forward;
    }
}
}