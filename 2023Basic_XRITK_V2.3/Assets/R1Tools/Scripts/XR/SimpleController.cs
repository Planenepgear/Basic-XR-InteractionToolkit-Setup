using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace R1Tools.Core 
{
public class SimpleController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference m_ActionReference;
    public InputActionReference actionReference { get => m_ActionReference; set => m_ActionReference = value; }

    public UnityEvent<bool> pressAction;
    public UnityEvent<bool> releaseAction;

    protected virtual void OnEnable()
    {
        if (m_ActionReference == null || m_ActionReference.action == null)
            return;

        m_ActionReference.action.started += OnActionStarted;
        m_ActionReference.action.performed += OnActionPerformed;
        m_ActionReference.action.canceled += OnActionCanceled;
    }

    protected virtual void OnActionStarted(InputAction.CallbackContext ctx)
    {
        //Debug.Log("Started");
    }

    protected virtual void OnActionPerformed(InputAction.CallbackContext ctx)
    {
        pressAction.Invoke(true);
    }

    protected virtual void OnActionCanceled(InputAction.CallbackContext ctx)
    {
        releaseAction.Invoke(false);
    }

}
}