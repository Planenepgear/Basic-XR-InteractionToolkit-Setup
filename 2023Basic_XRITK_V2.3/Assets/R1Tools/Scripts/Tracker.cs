#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace R1Tools.Core
{
    public class Tracker : MonoBehaviour
    {
        public bool isTracked = false;

        public int trackerId = 0;

        public TrackerAnimTool controlScript;

        public Transform offset;

        [SerializeField]
        private InputActionReference m_ActionReference;
        public InputActionReference actionReference { get => m_ActionReference; set => m_ActionReference = value; }

        public UnityEvent pressAction;
        public UnityEvent releaseAction;

        private float timeLeft = 5.0f;
        private bool lastTrackState = false;
        private bool ranOnce = false;

        protected virtual void OnEnable()
        {
            if (m_ActionReference == null || m_ActionReference.action == null)
                return;

            m_ActionReference.action.started += OnActionStarted;
            m_ActionReference.action.performed += OnActionPerformed;
            m_ActionReference.action.canceled += OnActionCanceled;
        }

        private void Start()
        {
            if (isTracked == false)
            {
                controlScript.deRegisterTracker(trackerId);
                ranOnce = true;
            }
        }

        protected virtual void OnActionStarted(InputAction.CallbackContext ctx)
        {
            //Debug.Log("Started");
        }

        protected virtual void OnActionPerformed(InputAction.CallbackContext ctx)
        {
            if (lastTrackState == false)
            {
                //Register again
                controlScript.registerTracker(trackerId);
            }

            isTracked = true;
            lastTrackState = true;
            pressAction.Invoke();
        }

        protected virtual void OnActionCanceled(InputAction.CallbackContext ctx)
        {

            isTracked = false;
            lastTrackState = false;
            releaseAction.Invoke();
        }

        private void Update()
        {
            //Hack - only check if disconnected if it is not tracked
            if (isTracked == false && transform.localPosition == Vector3.zero)
            {
                //Debug.Log("Tracker is at zero and is not tracking");
                timeLeft -= Time.deltaTime;
                if (timeLeft < 0 && ranOnce == false)
                {
                    //disconnected event
                    // Debug.Log("Disconnect event");
                    //Deregister
                    controlScript.deRegisterTracker(trackerId);
                    ranOnce = true;
                }
            }
            else
            {
                timeLeft = 5.0f;
                ranOnce = false;
            }
        }
    }
}
#endif