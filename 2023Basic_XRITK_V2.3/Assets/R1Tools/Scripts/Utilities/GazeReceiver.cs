#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Events;

namespace R1Tools.Core
{
    public class GazeReceiver : MonoBehaviour
    {
        public UnityEvent hitEvent;

        [HideInInspector]
        public GazeSender gazeScript;

        public GameObject gazeGizmo;

        public void showGizmo(bool setting)
        {
            gazeGizmo.SetActive(setting);
        }

        public void onObservationHit()
        {
            if (hitEvent != null)
            {
                hitEvent.Invoke();

            }
        }
    }
}
#endif