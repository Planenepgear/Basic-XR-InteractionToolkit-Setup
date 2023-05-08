#if UNITY_EDITOR
using System.Collections;
using UnityEngine;

#if VRIK_PRESENT
using RootMotion.Demos;
#endif

namespace R1Tools.Core
{
    public class GazeSender : MonoBehaviour
    {
        private bool hideAllGizmos = false;

        [Tooltip("Use this to hide all the gizmos by default")]
        public bool hideStandardGizmos = false;
        [Tooltip("Automatically hide the mirror on snap")]
        public bool autoHideMirror = true;
        [Tooltip("The amount of seconds to show the gizmos before hiding them")]
        public float secondsToShowToolsOnCalibrate = 2f;
        // Set this value here as the standard!\/
        [Tooltip("How fast the gaze modal should activate at")]
        public float gazeSpeed = 0.01f;

        [HideInInspector]
        public Transform m_camera;

        [HideInInspector]
        public GameObject pointDetector;


#if VRIK_PRESENT
    [HideInInspector]
    public CalibratorController calibratorScript;
#endif

        [HideInInspector]
        public TrackerAnimTool trackerScript;

        [HideInInspector]
        public GameObject standardGizmos;

        [HideInInspector]
        public GameObject allGizmos;

        [HideInInspector]
        public Material progressMaterial;

        [HideInInspector]
        public bool hovering = false;

        public GameObject gazeObject;

        [HideInInspector]
        public GazeReceiver currentGazeScript;

        [HideInInspector]
        public Collider objectHit;

        private Collider colliderCurrent;

        private float rayDistance = 50f;

        private float progress = 0.01f;

        private ButtonManager buttonManagerScript;

        private void OnDisable()
        {
            hovering = false;
            progressMaterial.SetFloat("_Cutoff", 0.01f);
        }
        void Reset()
        {
            lookForGizmos();
        }

        private void Start()
        {
            lookForGizmos();
        }

        public void showGizmos()
        {
            //Stop all running wait to hide gizmo coroutines!
            StopAllCoroutines();
            buttonManagerScript.showMirror(true);
            allGizmos.SetActive(true);
            standardGizmos.SetActive(true);
            trackerScript.trackerVisibility(true);
        }

        public void hideGazeGizmos()
        {
            progressMaterial.SetFloat("_Cutoff", 0.01f);
            gazeObject = GameObject.Find("R1GazeGizmo");
            gazeObject.SetActive(false);
            //Disable the pointDetector sphere also
            pointDetector.SetActive(false);
        }

        public void lookForGizmos()
        {
            if (allGizmos == null)
            {
                allGizmos = GameObject.Find("R1 Tools Gizmos");
            }
            if (standardGizmos == null)
            {
                if (allGizmos != null)
                {
                    standardGizmos = allGizmos.transform.Find("R1 Tools Standard Gizmos").gameObject;
                }
            }
            if (buttonManagerScript == null)
            {
                buttonManagerScript = GameObject.FindObjectOfType<ButtonManager>();
            }

        }

        IEnumerator currentlyHovering()
        {
            float localFocusSpeed = gazeSpeed;
            hovering = true;
            currentGazeScript.showGizmo(true);
            while (progress < 1)
            {
                progress += localFocusSpeed * (Time.deltaTime / 2);
                localFocusSpeed += (Time.deltaTime / 2);
                progressMaterial.SetFloat("_Cutoff", progress);
                yield return null; // Allow this loop to end every frame
            }
            yield return null; //Come back here to activate hit events

            //Start the hide coroutine immediately here!
            StartCoroutine(waitThenHideGizmos());
            trackerScript.calibrate();
            currentGazeScript.onObservationHit();
            progressMaterial.SetFloat("_Cutoff", 0.01f);
            currentGazeScript.showGizmo(false);

            //Disable the pointDetector sphere also
            pointDetector.SetActive(false);
        }

        public IEnumerator waitThenHideGizmos()
        {
            //Hide all gizmos here
            yield return new WaitForSecondsRealtime(secondsToShowToolsOnCalibrate);
            //Disable the pointDetector sphere also
            pointDetector.SetActive(false);
            if (autoHideMirror == true)
            {
                buttonManagerScript.showMirror(false);
            }

            if (hideAllGizmos == true)
            {

                if (allGizmos != null)
                {
                    allGizmos.SetActive(false);
                    //Disable the script here, done
                    enabled = false;
                }
                else
                {
                    Debug.Log("R1 Tools Gizmos is missing from the scene, please add it from the Prefabs folder of Plugins/R1Tools");
                }
            }
            else if (hideStandardGizmos == true)
            {
                if (standardGizmos != null)
                {
                    standardGizmos.SetActive(false);
                    enabled = false;
                }
                else
                {
                    Debug.Log("R1 Tools Standard Gizmos is missing from the scene, please add it from the Prefabs folder of Plugins/R1Tools");
                }
            }
            yield return null;
        }

        void Update()
        {
            RaycastHit hit;
            Ray ray = new Ray(m_camera.position, m_camera.forward);
            Debug.DrawRay(m_camera.position, m_camera.forward * rayDistance, Color.green);
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                objectHit = hit.collider;
                if (objectHit.GetComponent<GazeReceiver>())
                {
                    currentGazeScript = objectHit.gameObject.GetComponent<GazeReceiver>();
                }
                else
                {
                    //Bail out of raycasting this time
                    return;
                }

                if (currentGazeScript != null)
                {
                    if (colliderCurrent == null)
                    {
                        //Check if we haven't hit anything yet
                        colliderCurrent = objectHit;
                    }

                    if (objectHit != colliderCurrent)
                    {
                        StopCoroutine("currentlyHovering");
                        //New object hit, clear all values here and reset
                        currentGazeScript = colliderCurrent.gameObject.GetComponent<GazeReceiver>();

                        progressMaterial.SetFloat("_Cutoff", 0.01f);
                        //If we hit a new object

                        progress = 0f;
                        hovering = false;

                    }

                    colliderCurrent = objectHit;
                    pointDetector.SetActive(true);
                    //Always set position of the pointDetector
                    pointDetector.transform.position = hit.point;

                    if (hovering == false)
                    {
                        //New object hit so activate everything, hovering will be set true here also!
                        progressMaterial.SetFloat("_Cutoff", 0.01f);
                        //Start this if we hit something new
                        StartCoroutine("currentlyHovering");

                        //Debug.Log(objectHit);
                    }
                }

                // Do something with the object that was hit by the raycast.
            }
            else
            {
                if (hovering != false)
                {
                    //Maybe reset here too
                    pointDetector.SetActive(false);
                    //Always stop if we look away
                    if (currentGazeScript != null)
                    {
                        currentGazeScript.showGizmo(false);
                    }
                    StopCoroutine("currentlyHovering");
                    progress = 0f;
                    hovering = false;
                    progressMaterial.SetFloat("_Cutoff", 0.01f);

                }
            }
        }
    }
}
#endif