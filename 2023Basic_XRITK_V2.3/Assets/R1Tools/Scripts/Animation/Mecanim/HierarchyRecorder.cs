#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEngine;

namespace R1Tools.Mecanim
{
    public class HierarchyRecorder : MonoBehaviour
    {

        /*
         * 
         * Please import the Unity official FBX Exporter.

            Please export the character as animated FBX.

            Please import the output FBX in Humanoid format.

            If it goes well you will get a Humanoid type animation clip.
         * 
         */

        public AnimationClip clip;
        [Tooltip("x number of frames to record at, 1 is default, any higher will record less frames e.g. 2 = record once every 2 frames and so on")]
        [Header("Record every 1 frame, default is 1, higher records less frequently")]
        public int recordingFrequency = 1;
        [Tooltip("This is the framerate the clip will be saved with as playback speed, this isn't the amount of frames the clip will have")]
        public int clipPlaybackFramerate = 60;
        [Tooltip("Apply keyframe reduction to the saved clip")]
        public bool keyframeReduction = true;
        private GameObjectRecorder m_Recorder;

        private bool isrecording = false;
        private bool wasRecording = false;

        private int interval = 3;


        private CurveFilterOptions filterOptions = new CurveFilterOptions()
        {
            unrollRotation = true,
            keyframeReduction = false,
            positionError = 0.5f,
            rotationError = 0.5f,
            scaleError = 0.5f,
            floatError = 0.5f
        };

        void Start()
        {
            filterOptions.keyframeReduction = keyframeReduction;
            // Create recorder and record the script GameObject.
            m_Recorder = new GameObjectRecorder(gameObject);

            // Bind all the Transforms on the GameObject and all its children.
            m_Recorder.BindComponentsOfType<Transform>(gameObject, true);
        }

        public void StartBaking()
        {
            if (clip == null)
            {
                Debug.LogError("Missing Animation Clip to record to, please assign one to Hierarchy Recorder");
                return;
            }
            Debug.Log("Starting Recording");
            isrecording = true;
            wasRecording = true;
        }

        public void StopBaking()
        {
            Debug.Log("Stopping Recording");
            isrecording = false;
        }

        void LateUpdate()
        {

            if (clip == null)
                return;
            if (isrecording == true)
            {
                if (Time.frameCount % interval == 0)
                {
                    //Debug.Log("Time frame was: " + Time.frameCount);
                    // Take a snapshot and record all the bindings values for this frame.
                    m_Recorder.TakeSnapshot(Time.deltaTime);
                }
            }
            else if (wasRecording == true)
            {
                // "record" is off, but we were recording:
                // save to clip and clear recording.
                Debug.Log("Stopping recording and saving clip");
                m_Recorder.SaveToClip(clip, clipPlaybackFramerate, filterOptions);
                //m_Recorder.ResetRecording();
                wasRecording = false;
            }
        }

        void OnDisable()
        {
            if (clip == null)
                return;

            if (m_Recorder.isRecording)
            {
                // Save the recorded session to the clip.
                m_Recorder.SaveToClip(clip);
            }
        }
    }
}
#endif