using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace R1Tools.Core 
{
public class ColliderButton : MonoBehaviour
{
    public UnityEvent buttonEvent;
    public Color32 defaultColor;
    public Color32 pressedColor;
    public Material buttonMat;

    private void OnDestroy()
    {
        buttonMat.color = defaultColor;
    }
    void OnTriggerEnter(Collider other)
    {
        StartCoroutine(Invoke());
    }

    private IEnumerator Invoke()
    {
        yield return null;
        StartCoroutine(Fade());
        buttonEvent.Invoke();
    }

    IEnumerator Fade()
    {
        buttonMat.color = pressedColor;
        yield return new WaitForSecondsRealtime(0.2f);
        buttonMat.color = defaultColor;
        yield return null;
    }
}
}