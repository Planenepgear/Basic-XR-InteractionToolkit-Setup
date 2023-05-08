#if UNITY_EDITOR
#if VRIK_PRESENT
using RootMotion;
using RootMotion.FinalIK;
using UnityEditor;
using R1Tools.Editor;
#endif
using R1Tools.Mecanim;
using RootMotion.Demos;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
namespace R1Tools.Core
{
#if VRIK_PRESENT
    [System.Serializable]
    [RequireComponent(typeof(CalibratorController))]
#endif
    public class TrackerAnimTool : MonoBehaviour
    {
#if VRIK_PRESENT
        [Tooltip("If this isn't set, if the Humanoid Baker script is on your animated character, it will be detected")]
        [Header("Add your VRIK Character root here")]
        public Transform characterRoot;
#else
        [Tooltip("This needs to be set")]
        [Header("This needs to be set as the character root")]
        public Transform characterRoot;
#endif
        [Tooltip("Use this for an automatic snap, may produce off scaling")]
        [SerializeField]
        private bool autoSnap = false;

        [HideInInspector]
        public GazeSender gazeScript;

        #region VRIK
        [HideInInspector]
        public CalibratorController calibrateScript;

#if VRIK_PRESENT

        [Tooltip("Please assign a VRIK Script")]
        [HideInInspector]
        public VRIK vrikScript;

        [Tooltip("If this isn't set, if the Humanoid Baker script is on your animated character, it will be detected")]
        public HumanoidBaker characterBakeScript;

        [Tooltip("If this isn't set, the first Generic Baker script found in the scene will be used")]
        public GenericBaker objectBakeScript;

        [Tooltip("Use this file to store the calibrated height of your character (optional)")]
        public SaveScaleScriptableObject characterScaleReference;
#endif
        [Tooltip("The amount to resize your character by on calibrate, by default leave this at 0 unless you need to adjust scale")]
        //[Range(0.01f, 10f)]
        public float scaleAmount = 0.04f;

        private float heightScale = 1;
        private float armScale = 1;

        [HideInInspector]
        public MecanimIKControl mecanimScript;
        [HideInInspector]
        public HierarchyRecorder avatarRecordScript;

        public HierarchyRecorder objectRecordScript;
        #endregion

        private Transform characterLeftWristBone;
        private Transform characterLeftMiddleFingerBone;

        [HideInInspector]
        public GameObject trackerBallPrefab;
        [HideInInspector]
        public GameObject trackerPrefab;

        private Animator targetAnimator;

        [Serializable]
        public struct defaultKit
        {
            public Transform leftController;
            public Transform rightController;
            public Transform headset;

            public Transform leftControllerOffset;
            public Transform rightControllerOffset;
            public Transform headsetOffset;
        }

        //Use to avoid heaped garbage
        public static List<InputDevice> devices = new List<InputDevice>();

        [System.Serializable]
        public class identifiedTracker
        {
            public string trackerID;
            public trackerRole tRole;
            public InputDevice trackedDevice;
        }

        [HideInInspector]
        public defaultKit ControllersAndHeadset;

        private GameObject[] defaultKitTrackerBall = new GameObject[3];

        [HideInInspector]
        [SerializeField]
        private Transform[] target = new Transform[11];

        [HideInInspector]
        public GameObject[] trackerMesh = new GameObject[11];

        public enum trackerRole
        {
            LeftFoot, //0
            RightFoot, //1
            LeftShoulder, //2
            RightShoulder, //3
            LeftElbow, //4
            RightElbow, //5
            LeftKnee, //6
            RightKnee, //7
            Waist, //8
            Chest, //9
            Camera, //10
            Keyboard, //11
            HeldInHand, //12?
        }

        void Reset()
        {
#if VRIK_PRESENT
            if (calibrateScript == null)
            {
                calibrateScript = GetComponent<CalibratorController>();
            }
            if (objectBakeScript == null)
            {
                objectBakeScript = GameObject.FindObjectOfType<GenericBaker>();
            }
#endif
            AutoDetectMainReferences();
        }

        void OnValidate()
        {
#if VRIK_PRESENT
            if (scaleAmount <= 0)
            {
                scaleAmount = 0.01f;
            }
#endif
            //When the user amends something, run this
            if (targetAnimator != null)
            {
                checkHandScale();
            }
        }

        #region General

        private void Start()
        {
            if (characterRoot == null)
            {
                Debug.LogError("Please assign a character root gameobject to TrackerAnimTool.cs on the XR Animation Rig");
            }
            else
            {
#if VRIK_PRESENT


                vrikScript = characterRoot.GetComponent<VRIK>();

                if (vrikScript != null)
                {
                    characterBakeScript = vrikScript.gameObject.GetComponent<HumanoidBaker>();
                    calibrateScript.setVrik(vrikScript);
                    targetAnimator = vrikScript.gameObject.GetComponent<Animator>();
                }
                else
                {
                    Debug.LogError("Please assign a VRIK Script to your character root");
                }

#else
            targetAnimator = characterRoot.GetComponent<Animator>();
            if (targetAnimator == null)
            {
                Debug.LogError("Character Root was not assigned or Character Root did not contain an Animator, please add one and restart Play Mode");
            }
#endif
                AutoDetectMainReferences();
                instantiateControllerAndHeadsetPrefabs();

                //If we want to auto snap
                if (autoSnap == true)
                {
                    //Start coroutine to wait one second to snap
                    StartCoroutine(autoSnapper());
                }
            }

        }

        private IEnumerator autoSnapper()
        {
            yield return new WaitForSecondsRealtime(1f);
            calibrate();
            gazeScript.hideGazeGizmos();
        }

        #endregion

        #region Tracker Events

        public void deRegisterTracker(int deregTrackerId)
        {
#if VRIK_PRESENT
            if (vrikScript != null)
            {
                if (vrikScript.solver.initiated)
                {
                    switch (deregTrackerId)
                    {
                        //Left Foot
                        case 0:
                            vrikScript.solver.leftLeg.positionWeight = 0;
                            vrikScript.solver.leftLeg.rotationWeight = 0;
                            break;
                        //Right Foot
                        case 1:
                            vrikScript.solver.rightLeg.positionWeight = 0;
                            vrikScript.solver.rightLeg.rotationWeight = 0;
                            break;
                        //Left Shoulder
                        case 2:
                            //Not used by VRIK
                            break;
                        //Right Shoulder
                        case 3:
                            //Not used by VRIK
                            break;
                        //Left Elbow
                        case 4:
                            vrikScript.solver.leftArm.bendGoalWeight = 0;
                            break;
                        //Right Elbow
                        case 5:
                            vrikScript.solver.rightArm.bendGoalWeight = 0;
                            break;
                        //Left Knee
                        case 6:
                            vrikScript.solver.leftLeg.bendGoalWeight = 0;
                            break;
                        //Right Knee
                        case 7:
                            vrikScript.solver.rightLeg.bendGoalWeight = 0;
                            break;
                        //Waist
                        case 8:
                            vrikScript.solver.spine.pelvisPositionWeight = 0;
                            vrikScript.solver.spine.pelvisRotationWeight = 0;
                            break;
                        //Chest
                        case 9:
                            vrikScript.solver.spine.chestGoalWeight = 0;
                            break;
                        //Keyboard
                        case 10:
                            //Keyboard not used by VRIK
                            break;
                    }
                }
                else
                {

                }
            }
#endif
        }

        public void registerTracker(int regTrackerId)
        {
#if VRIK_PRESENT
            if (vrikScript != null)
            {
                if (vrikScript.solver.initiated)
                {
                    Debug.Log("Register tracker: " + regTrackerId);
                    switch (regTrackerId)
                    {
                        //Left Foot
                        case 0:
                            vrikScript.solver.leftLeg.positionWeight = 1;
                            vrikScript.solver.leftLeg.rotationWeight = 1;
                            break;
                        //Right Foot
                        case 1:
                            vrikScript.solver.rightLeg.positionWeight = 1;
                            vrikScript.solver.rightLeg.rotationWeight = 1;
                            break;
                        //Left Shoulder
                        case 2:
                            //Not used by VRIK
                            break;
                        //Right Shoulder
                        case 3:
                            //Not used by VRIK
                            break;
                        //Left Elbow
                        case 4:
                            vrikScript.solver.leftArm.bendGoalWeight = 1;
                            break;
                        //Right Elbow
                        case 5:
                            vrikScript.solver.rightArm.bendGoalWeight = 1;
                            break;
                        //Left Knee
                        case 6:
                            vrikScript.solver.leftLeg.bendGoalWeight = 1;
                            break;
                        //Right Knee
                        case 7:
                            vrikScript.solver.rightLeg.bendGoalWeight = 1;
                            break;
                        //Waist
                        case 8:
                            vrikScript.solver.spine.pelvisPositionWeight = 1;
                            vrikScript.solver.spine.pelvisRotationWeight = 1;
                            break;
                        //Chest
                        case 9:
                            vrikScript.solver.spine.chestGoalWeight = 1;
                            break;
                        //Keyboard
                        case 10:
                            //Keyboard not used by VRIK
                            break;
                    }
                }
                else
                {

                }
            }
#endif
        }

        #endregion

        public void calibrate()
        {
#if VRIK_PRESENT
            calibrateScript.calibrate();
#else
            mecanimScript.setupIK(targetAnimator);
#endif
        }

        public void onCalibrate()
        {
#if VRIK_PRESENT
            characterBakeScript = vrikScript.gameObject.GetComponent<HumanoidBaker>();

            //resizeHand();
            if (characterScaleReference != null && characterRoot != null)
            {
                EditorUtility.SetDirty(characterScaleReference);
                characterScaleReference.characterScale = characterRoot.localScale;
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.Log("No Character Scale Reference file found, the scale of this character will not be saved during this session");
            }
#endif
            checkHandScale();
        }

        private void checkHandScale()
        {
            if (targetAnimator != null)
            {
                //Default hand distance for wrist to middle finger is: 0.1854926
                characterLeftWristBone = targetAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
                characterLeftMiddleFingerBone = targetAnimator.GetBoneTransform(HumanBodyBones.LeftMiddleDistal);
            }
            else
            {
                //Debug.LogError("Character requires an Animator component at its root with an Avatar assigned for finger animation to work");
            }
        }

        public void resizeHand()
        {

        }

        public void HeightIncrease()
        {
#if VRIK_PRESENT
            heightScale = heightScale + scaleAmount;
            vrikScript.solver.leftLeg.legLengthMlp = heightScale;
            vrikScript.solver.rightLeg.legLengthMlp = heightScale;
#else
            //old use for mecanim
            heightScale = characterRoot.localScale.y + scaleAmount;
            characterRoot.localScale = new Vector3(heightScale, heightScale, heightScale);
#endif
        }
        public void HeightDecrease()
        {
#if VRIK_PRESENT
            heightScale = heightScale - scaleAmount;
            vrikScript.solver.leftLeg.legLengthMlp = heightScale;
            vrikScript.solver.rightLeg.legLengthMlp = heightScale;
#else
            //old use for mecanim
            heightScale = characterRoot.localScale.y - scaleAmount;
            characterRoot.localScale = new Vector3(heightScale, heightScale, heightScale);
#endif
        }
        public void ArmsGrow()
        {
#if VRIK_PRESENT
            armScale = armScale + scaleAmount;
            vrikScript.solver.leftArm.armLengthMlp = armScale;
            vrikScript.solver.rightArm.armLengthMlp = armScale;
#else
            //old use for mecanim
            armScale = mecanimScript.leftLowerArm.localScale.y + scaleAmount;
            mecanimScript.leftLowerArm.localScale = mecanimScript.leftUpperArm.localScale = mecanimScript.rightLowerArm.localScale = mecanimScript.rightUpperArm.localScale
                = new Vector3(armScale, armScale, armScale);
#endif
        }
        public void ArmsShrink()
        {
#if VRIK_PRESENT
            armScale = armScale - scaleAmount;
            vrikScript.solver.leftArm.armLengthMlp = armScale;
            vrikScript.solver.rightArm.armLengthMlp = armScale;
#else
            //old use for mecanim
            armScale = mecanimScript.leftLowerArm.localScale.y - scaleAmount;
            mecanimScript.leftLowerArm.localScale = mecanimScript.leftUpperArm.localScale = mecanimScript.rightLowerArm.localScale = mecanimScript.rightUpperArm.localScale
                = new Vector3(armScale, armScale, armScale);
#endif
        }
        public void AutoDetectMainReferences()
        {
#if VRIK_PRESENT
            if (vrikScript != null)
            {
                characterRoot = vrikScript.references.root;
            }

            if (vrikScript == null && characterRoot != null)
            {
                vrikScript = characterRoot.GetComponent<VRIK>();
            }
#endif
            if (characterRoot != null)
            {
                var animator = characterRoot.GetComponentInChildren<Animator>();
                if (animator == null || !animator.isHuman)
                {
                    Debug.LogWarning("No Humanoid Biped animator found, please set references manually or add humanoid animator to your character");
                    return;
                }
            }
        }

        public void instantiateControllerAndHeadsetPrefabs()
        {
            if (ControllersAndHeadset.leftController != null)
            {
                defaultKitTrackerBall[0] = (GameObject)GameObject.Instantiate(trackerBallPrefab, ControllersAndHeadset.leftController.position, Quaternion.identity, ControllersAndHeadset.leftController);
                defaultKitTrackerBall[0].name = "LeftControllerSphere";
            }
            if (ControllersAndHeadset.rightController != null)
            {
                defaultKitTrackerBall[1] = (GameObject)GameObject.Instantiate(trackerBallPrefab, ControllersAndHeadset.rightController.position, Quaternion.identity, ControllersAndHeadset.rightController);
                defaultKitTrackerBall[1].name = "RightControllerSphere";
            }
            if (ControllersAndHeadset.headset != null)
            {
                defaultKitTrackerBall[2] = (GameObject)GameObject.Instantiate(trackerBallPrefab, ControllersAndHeadset.headset.position, Quaternion.identity, ControllersAndHeadset.headset);
                defaultKitTrackerBall[2].name = "HeadsetSphere";
            }
        }

        public void trackerVisibility(bool setting)
        {
            for (int i = 0; i < defaultKitTrackerBall.Length; i++)
            {
                if (defaultKitTrackerBall[i] != null)
                {
                    defaultKitTrackerBall[i].SetActive(setting);
                }
            }
            //Loop through the targets array to shut down all those meshes or set them in activetrackerprefab list in a hidden array set in prefab?
            for (int x = 0; x < trackerMesh.Length; x++)
            {
                if (trackerMesh[x] != null)
                {
                    trackerMesh[x].SetActive(setting);
                }
            }
        }
    }


    [System.Serializable]
    public class fingerBone
    {
        public Transform[] targetBones = new Transform[0];
    }
}
#endif