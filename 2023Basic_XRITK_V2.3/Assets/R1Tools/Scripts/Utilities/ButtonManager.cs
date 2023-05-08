#if UNITY_EDITOR
#if VRIK_PRESENT
using RootMotion;
#else
using R1Tools.Mecanim;
#endif

using UnityEngine;

namespace R1Tools.Core
{
    public class ButtonManager : MonoBehaviour
    {
        [HideInInspector]
        public TrackerAnimTool trackerScript;
        [HideInInspector]
        public GazeSender gazescript;

        public GameObject gazeGizmo;

        public GameObject standardGizmos;

        public GameObject mirror;

#if VRIK_PRESENT
    private HumanoidBaker characterBakeScript;

    private GenericBaker objectBakeScript;
#else
        public HierarchyRecorder avatarRecordScript;
        public HierarchyRecorder objectRecordScript;
#endif
        private void Reset()
        {
            if (trackerScript == null)
            {
                trackerScript = FindObjectOfType<TrackerAnimTool>();
            }
            if (gazescript == null)
            {
                gazescript = FindObjectOfType<GazeSender>();
            }
#if VRIK_PRESENT
        if (objectBakeScript == null)
        {
            objectBakeScript = trackerScript.objectBakeScript;
        }
#endif

        }

        private void Start()
        {
            if (trackerScript == null)
            {
                trackerScript = FindObjectOfType<TrackerAnimTool>();
            }
            if (gazescript == null)
            {
                gazescript = FindObjectOfType<GazeSender>();
            }
#if VRIK_PRESENT
        if (objectBakeScript == null)
        {
            objectBakeScript = trackerScript.objectBakeScript;
        }
#endif
        }

        public void buttonHeightIncrease()
        {
            if (trackerScript != null)
            {
                trackerScript.HeightIncrease();
            }
        }
        public void buttonHeightDecrease()
        {
            if (trackerScript != null)
            {
                trackerScript.HeightDecrease();
            }
        }
        public void buttonArmsGrow()
        {
            if (trackerScript != null)
            {
                trackerScript.ArmsGrow();
            }
        }
        public void buttonArmsShrink()
        {
            if (trackerScript != null)
            {
                trackerScript.ArmsShrink();
            }
        }

        public void resetGizmos()
        {
            //Stop recording too?
            //stopRecording();
            mirror.SetActive(true);
            if (gazescript != null)
            {
                gazescript.enabled = true;
                gazescript.showGizmos();
                gazeGizmo.SetActive(true);
            }
        }

        public void toggleStandardGizmos()
        {
            Debug.Log("ToggleGizmos");
            if (standardGizmos.activeSelf == true)
            {
                standardGizmos.SetActive(false);
            }
            else
            {
                standardGizmos.SetActive(true);
            }
        }

        public void showMirror(bool option)
        {
            Debug.Log("ShowMirror: " + option);
            mirror.SetActive(option);
        }

        public void toggleMirror()
        {
            if (mirror.activeSelf == true)
            {
                mirror.SetActive(false);
            }
            else
            {
                mirror.SetActive(true);
            }
        }

        public void startRecording()
        {
#if VRIK_PRESENT

        //Debug.Log("Start Recording");
        characterBakeScript = trackerScript.characterBakeScript;
        if (characterBakeScript != null)
        {
            characterBakeScript.StartBaking();
        }
        else
        {
            characterBakeScript = trackerScript.characterRoot.GetComponent<HumanoidBaker>();
            if (characterBakeScript != null)
            {
                characterBakeScript.StartBaking();
            }
            else
            {
                Debug.LogError("No Humanoid baker found, please add a humanoid baker to the Character Gameobject with VRIK Script on it");
            }

        }
        if (objectBakeScript == null)
        {
            objectBakeScript = trackerScript.objectBakeScript;
        }
        if (objectBakeScript != null)
        {
            //For now this is a test, set to do this on grab
            objectBakeScript.StartBaking();
        }

#else
            //Debug.Log("Start Recording");
            avatarRecordScript = trackerScript.avatarRecordScript;
            if (avatarRecordScript != null)
            {
                avatarRecordScript.StartBaking();
            }
            else
            {
                avatarRecordScript = trackerScript.characterRoot.GetComponent<HierarchyRecorder>();
                if (avatarRecordScript != null)
                {
                    avatarRecordScript.StartBaking();
                }
                else
                {
                    Debug.LogError("No Humanoid baker found, please add a humanoid baker to the Character Gameobject with VRIK Script on it");
                }

            }
            if (objectRecordScript == null)
            {
                objectRecordScript = trackerScript.objectRecordScript;
            }
            if (objectRecordScript != null)
            {
                //For now this is a test, set to do this on grab
                objectRecordScript.StartBaking();
            }
#endif
        }

        public void stopRecording()
        {
#if VRIK_PRESENT
        Debug.Log("Stop Recording");
        if (characterBakeScript != null)
        {
            characterBakeScript.StopBaking();
        }

        if (objectBakeScript != null)
        {
            //For not this is a test, set to do this on grab
            objectBakeScript.StopBaking();
        }
#else
            if (avatarRecordScript != null)
            {
                avatarRecordScript.StopBaking();
            }
            if (objectRecordScript == null)
            {
                objectRecordScript = trackerScript.objectRecordScript;
            }

#endif
        }
    }
}
#endif
