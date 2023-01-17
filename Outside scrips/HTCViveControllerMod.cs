using System.Collections.Generic;
using UnityEngine.Scripting;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

using PoseControl = UnityEngine.XR.OpenXR.Input.PoseControl;

namespace UnityEngine.XR.OpenXR.Features.Interactions
{
    /// <summary>
    /// This <see cref="OpenXRInteractionFeature"/> enables the use of HTC Vive Controllers interaction profiles in OpenXR.
    /// </summary>
#if UNITY_EDITOR
    [UnityEditor.XR.OpenXR.Features.OpenXRFeature(UiName = "HTC Vive Controller Mod",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.WSA },
        Company = "Unity",
        Desc = "Allows for mapping input to the HTC Vive Controller interaction profile.",
        DocumentationLink = Constants.k_DocumentationManualURL + "features/htcvivecontrollerprofile.html",
        OpenxrExtensionStrings = "XR_HTCX_vive_tracker_interaction",
        Version = "0.0.1",
        Category = UnityEditor.XR.OpenXR.Features.FeatureCategory.Interaction,
        FeatureId = featureId)]
#endif
    public class HTCViveControllerMod : OpenXRInteractionFeature
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.unity.openxr.feature.input.htcvivemod";

        /// <summary>
        /// An Input System device based off the <a href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#_htc_vive_controller_profile">HTC Vive Controller</a>.
        /// </summary>
        [Preserve, InputControlLayout(displayName = "HTC Vive Controller (OpenXR)", commonUsages = new[] { "LeftHand", "RightHand" })]
        public class ViveController : XRControllerWithRumble
        {
            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the HTC Vive Controller Profile select OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "Secondary", "selectbutton" }, usage = "SystemButton")]
            public ButtonControl select { get; private set; }

            /// <summary>
            /// A [AxisControl](xref:UnityEngine.InputSystem.Controls.AxisControl) that represents information from the <see cref="HTCViveControllerProfile.squeeze"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "GripAxis", "squeeze" }, usage = "Grip")]
            public AxisControl grip { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the <see cref="HTCViveControllerProfile.squeeze"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "GripButton", "squeezeClicked" }, usage = "GripButton")]
            public ButtonControl gripPressed { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the <see cref="HTCViveControllerProfile.menu"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "Primary", "menubutton" }, usage = "MenuButton")]
            public ButtonControl menu { get; private set; }

            /// <summary>
            /// A [AxisControl](xref:UnityEngine.InputSystem.Controls.AxisControl) that represents information from the <see cref="HTCViveControllerProfile.trigger"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(alias = "triggeraxis", usage = "Trigger")]
            public AxisControl trigger { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the <see cref="HTCViveControllerProfile.triggerClick"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(alias = "triggerbutton", usage = "TriggerButton")]
            public ButtonControl triggerPressed { get; private set; }

            /// <summary>
            /// A [Vector2Control](xref:UnityEngine.InputSystem.Controls.Vector2Control) that represents information from the <see cref="HTCViveControllerProfile.trackpad"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "Primary2DAxis", "touchpadaxes", "touchpad" }, usage = "Primary2DAxis")]
            public Vector2Control trackpad { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the <see cref="HTCViveControllerProfile.trackpadClick"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "joystickorpadpressed", "touchpadpressed" }, usage = "Primary2DAxisClick")]
            public ButtonControl trackpadClicked { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) that represents information from the <see cref="HTCViveControllerProfile.trackpadTouch"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(aliases = new[] { "joystickorpadtouched", "touchpadtouched" }, usage = "Primary2DAxisTouch")]
            public ButtonControl trackpadTouched { get; private set; }

            /// <summary>
            /// A <see cref="PoseControl"/> that represents information from the <see cref="HTCViveControllerProfile.grip"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(offset = 0, aliases = new[] { "device", "gripPose" }, usage = "Device")]
            public PoseControl devicePose { get; private set; }

            /// <summary>
            /// A <see cref="PoseControl"/> that represents information from the <see cref="HTCViveControllerProfile.aim"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(offset = 0, alias = "aimPose", usage = "Pointer")]
            public PoseControl pointer { get; private set; }

            /// <summary>
            /// A [ButtonControl](xref:UnityEngine.InputSystem.Controls.ButtonControl) required for backwards compatibility with the XRSDK layouts. This represents the overall tracking state of the device. This value is equivalent to mapping devicePose/isTracked.
            /// </summary>
            [Preserve, InputControl(offset = 26)]
            new public ButtonControl isTracked { get; private set; }

            /// <summary>
            /// A [IntegerControl](xref:UnityEngine.InputSystem.Controls.IntegerControl) required for back compatibility with the XRSDK layouts. This represents the bit flag set indicating what data is valid. This value is equivalent to mapping devicePose/trackingState.
            /// </summary>
            [Preserve, InputControl(offset = 28)]
            new public IntegerControl trackingState { get; private set; }

            /// <summary>
            /// A [Vector3Control](xref:UnityEngine.InputSystem.Controls.Vector3Control) required for back compatibility with the XRSDK layouts. This is the device position. For the Oculus Touch device, this is both the grip and the pointer position. This value is equivalent to mapping devicePose/position.
            /// </summary>
            [Preserve, InputControl(offset = 32, alias = "gripPosition")]
            new public Vector3Control devicePosition { get; private set; }

            /// <summary>
            /// A [QuaternionControl](xref:UnityEngine.InputSystem.Controls.QuaternionControl) required for backwards compatibility with the XRSDK layouts. This is the device orientation. For the Oculus Touch device, this is both the grip and the pointer rotation. This value is equivalent to mapping devicePose/rotation.
            /// </summary>
            [Preserve, InputControl(offset = 44, alias = "gripOrientation")]
            new public QuaternionControl deviceRotation { get; private set; }

            /// <summary>
            /// A [Vector3Control](xref:UnityEngine.InputSystem.Controls.Vector3Control) required for back compatibility with the XRSDK layouts. This is the pointer position. This value is equivalent to mapping pointerPose/position.
            /// </summary>
            [Preserve, InputControl(offset = 92)]
            public Vector3Control pointerPosition { get; private set; }

            /// <summary>
            /// A [QuaternionControl](xref:UnityEngine.InputSystem.Controls.QuaternionControl) required for backwards compatibility with the XRSDK layouts. This is the pointer rotation. This value is equivalent to mapping pointerPose/rotation.
            /// </summary>
            [Preserve, InputControl(offset = 104, alias = "pointerOrientation")]
            public QuaternionControl pointerRotation { get; private set; }

            /// <summary>
            /// A <see cref="HapticControl"/> that represents the <see cref="HTCViveControllerProfile.haptic"/> binding.
            /// </summary>
            [Preserve, InputControl(usage = "Haptic")]
            public HapticControl haptic { get; private set; }

            /// <inheritdoc cref="OpenXRDevice"/>
            protected override void FinishSetup()
            {
                base.FinishSetup();
                select = GetChildControl<ButtonControl>("select");
                grip = GetChildControl<AxisControl>("grip");
                gripPressed = GetChildControl<ButtonControl>("gripPressed");
                menu = GetChildControl<ButtonControl>("menu");
                trigger = GetChildControl<AxisControl>("trigger");
                triggerPressed = GetChildControl<ButtonControl>("triggerPressed");
                trackpad = GetChildControl<Vector2Control>("trackpad");
                trackpadClicked = GetChildControl<ButtonControl>("trackpadClicked");
                trackpadTouched = GetChildControl<ButtonControl>("trackpadTouched");

                pointer = GetChildControl<PoseControl>("pointer");
                pointerPosition = GetChildControl<Vector3Control>("pointerPosition");
                pointerRotation = GetChildControl<QuaternionControl>("pointerRotation");

                devicePose = GetChildControl<PoseControl>("devicePose");
                isTracked = GetChildControl<ButtonControl>("isTracked");
                trackingState = GetChildControl<IntegerControl>("trackingState");
                devicePosition = GetChildControl<Vector3Control>("devicePosition");
                deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");

                haptic = GetChildControl<HapticControl>("haptic");
            }
        }

        [InputControlLayout(isGenericTypeOfDevice = true, displayName = "XR Tracker")]
        public class XRTracker : TrackedDevice
        {
        }

        [Preserve, InputControlLayout(displayName = "HTC Vive Tracker (OpenXR)", commonUsages = new[] { "Left Foot", "Right Foot", "Left Shoulder", "Right Shoulder", "Left Elbow", "Right Elbow", "Left Knee", "Right Knee", "Waist", "Chest", "Camera", "Keyboard" })]
        public class XRViveTracker : XRTracker
        {
            /// <summary>
            /// A <see cref="PoseControl"/> that represents information from the <see cref="HTCViveTrackerProfile.grip"/> OpenXR binding.
            /// </summary>
            [Preserve, InputControl(offset = 0, aliases = new[] { "device", "gripPose" }, usage = "Device", noisy = true)]
            public PoseControl devicePose { get; private set; }

            /// <summary>
            /// A [Vector3Control](xref:UnityEngine.InputSystem.Controls.Vector3Control) required for back compatibility with the XRSDK layouts. This is the device position. For the Oculus Touch device, this is both the grip and the pointer position. This value is equivalent to mapping devicePose/position.
            /// </summary>
            [Preserve, InputControl(offset = 8, alias = "gripPosition", noisy = true)]
            new public Vector3Control devicePosition { get; private set; }

            /// <summary>
            /// A [QuaternionControl](xref:UnityEngine.InputSystem.Controls.QuaternionControl) required for backwards compatibility with the XRSDK layouts. This is the device orientation. For the Oculus Touch device, this is both the grip and the pointer rotation. This value is equivalent to mapping devicePose/rotation.
            /// </summary>
            [Preserve, InputControl(offset = 20, alias = "gripOrientation", noisy = true)]
            new public QuaternionControl deviceRotation { get; private set; }

            [Preserve, InputControl(offset = 60)]
            new public ButtonControl isTracked { get; private set; }

            [Preserve, InputControl(offset = 64)]
            new public IntegerControl trackingState { get; private set; }

            /// <inheritdoc cref="OpenXRDevice"/>
            protected override void FinishSetup()
            {
                base.FinishSetup();
                devicePose = GetChildControl<PoseControl>("devicePose");
                devicePosition = GetChildControl<Vector3Control>("devicePosition");
                deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
                isTracked = GetChildControl<ButtonControl>("isTracked");
                trackingState = GetChildControl<IntegerControl>("trackingState");

                var capabilities = description.capabilities;
                var deviceDescriptor = XRDeviceDescriptor.FromJson(capabilities);

                if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftFoot) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Left Foot");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightFoot) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Right Foot");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftShoulder) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Left Shoulder");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightShoulder) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Right Shoulder");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftElbow) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Left Elbow");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightElbow) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Right Elbow");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftKnee) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Left Knee");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightKnee) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Right Knee");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerWaist) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Waist");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerChest) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Chest");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerCamera) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Camera");
                else if ((deviceDescriptor.characteristics & (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerKeyboard) != 0)
                    InputSystem.InputSystem.SetDeviceUsage(this, "Keyboard");

                Debug.Log("Device added");
            }
        }

        /// <summary>The interaction profile string used to reference the <a href="https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#_htc_vive_controller_profile">HTC Vive Controller</a>.</summary>
        public const string profile = "/interaction_profiles/htc/vive_controller";
        public const string profileTracker = "/interaction_profiles/htc/vive_tracker_htcx";

        /// <summary>
        /// Constant for a boolean interaction binding '.../input/system/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string system = "/input/system/click";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/squeeze/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string squeeze = "/input/squeeze/click";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/menu/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string menu = "/input/menu/click";
        /// <summary>
        /// Constant for a float interaction binding '.../input/trigger/value' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trigger = "/input/trigger/value";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trigger/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string triggerClick = "/input/trigger/click";
        /// <summary>
        /// Constant for a Vector2 interaction binding '.../input/trackpad' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trackpad = "/input/trackpad";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trackpad/click' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trackpadClick = "/input/trackpad/click";
        /// <summary>
        /// Constant for a boolean interaction binding '.../input/trackpad/touch' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string trackpadTouch = "/input/trackpad/touch";
        /// <summary>
        /// Constant for a pose interaction binding '.../input/grip/pose' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string grip = "/input/grip/pose";
        /// <summary>
        /// Constant for a pose interaction binding '.../input/aim/pose' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string aim = "/input/aim/pose";
        /// <summary>
        /// Constant for a haptic interaction binding '.../output/haptic' OpenXR Input Binding. Used by input subsystem to bind actions to physical inputs.
        /// </summary>
        public const string haptic = "/output/haptic";

        private const string kDeviceLocalizedNameRight = "HTC Vive Controller OpenXR Right";
        private const string kDeviceLocalizedNameLeft = "HTC Vive Controller OpenXR Left";
        private const string kDeviceLocalizedNameTracker = "HTC Vive Tracker OpenXR";

        /// <summary>
        /// Registers the <see cref="ViveController"/> layout with the Input System.
        /// </summary>
        protected override void RegisterDeviceLayout()
        {
            InputSystem.InputSystem.RegisterLayout(typeof(ViveController),
                        matches: new InputDeviceMatcher()
                        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                        .WithProduct(kDeviceLocalizedNameRight));
            InputSystem.InputSystem.RegisterLayout(typeof(ViveController),
                        matches: new InputDeviceMatcher()
                        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                        .WithProduct(kDeviceLocalizedNameLeft));

            InputSystem.InputSystem.RegisterLayout<XRTracker>();

            InputSystem.InputSystem.RegisterLayout(typeof(XRViveTracker),
                        matches: new InputDeviceMatcher()
                        .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                        .WithProduct(kDeviceLocalizedNameTracker));
        }

        /// <summary>
        /// Removes the <see cref="ViveController"/> layout from the Input System.
        /// </summary>
        protected override void UnregisterDeviceLayout()
        {
            InputSystem.InputSystem.RemoveLayout(nameof(ViveController));
            InputSystem.InputSystem.RemoveLayout(nameof(XRViveTracker));
            InputSystem.InputSystem.RemoveLayout(nameof(XRTracker));
        }

        //
        // Summary:
        //     A set of bit flags describing XR.InputDevice characteristics.
        //"Left Foot", "Right Foot", "Left Shoulder", "Right Shoulder", "Left Elbow", "Right Elbow", "Left Knee", "Right Knee", "Waist", "Chest", "Camera", "Keyboard"
        
        public enum InputDeviceTrackerCharacteristics : uint
        {
            TrackerLeftFoot = 0x1000u,
            TrackerRightFoot = 0x2000u,
            TrackerLeftShoulder = 0x4000u,
            TrackerRightShoulder = 0x8000u,
            TrackerLeftElbow = 0x10000u,
            TrackerRightElbow = 0x20000u,
            TrackerLeftKnee = 0x40000u,
            TrackerRightKnee = 0x80000u,
            TrackerWaist = 0x100000u,
            TrackerChest = 0x200000u,
            TrackerCamera = 0x400000u,
            TrackerKeyboard = 0x800000u
        }

        /// <summary>
        /// OpenXR user path definitions for the tracker.
        /// </summary>
        public static class TrackerUserPaths
        {
            /// <summary>
            /// Path for user left foot
            /// </summary>
            public const string leftFoot = "/user/vive_tracker_htcx/role/left_foot";

            /// <summary>
            /// Path for user right foot
            /// </summary>
            public const string rightFoot = "/user/vive_tracker_htcx/role/right_foot";

            /// <summary>
            /// Path for user left shoulder
            /// </summary>
            public const string leftShoulder = "/user/vive_tracker_htcx/role/left_shoulder";

            /// <summary>
            /// Path for user right shoulder
            /// </summary>
            public const string rightShoulder = "/user/vive_tracker_htcx/role/right_shoulder";

            /// <summary>
            /// Path for user left elbow
            /// </summary>
            public const string leftElbow = "/user/vive_tracker_htcx/role/left_elbow";

            /// <summary>
            /// Path for user right elbow
            /// </summary>
            public const string rightElbow = "/user/vive_tracker_htcx/role/right_elbow";

            /// <summary>
            /// Path for user left knee
            /// </summary>
            public const string leftKnee = "/user/vive_tracker_htcx/role/left_knee";

            /// <summary>
            /// Path for user right knee
            /// </summary>
            public const string rightKnee = "/user/vive_tracker_htcx/role/right_knee";

            /// <summary>
            /// Path for user waist
            /// </summary>
            public const string waist = "/user/vive_tracker_htcx/role/waist";

            /// <summary>
            /// Path for user chest
            /// </summary>
            public const string chest = "/user/vive_tracker_htcx/role/chest";

            /// <summary>
            /// Path for user custom camera
            /// </summary>
            public const string camera = "/user/vive_tracker_htcx/role/camera";

            /// <summary>
            /// Path for user keyboard
            /// </summary>
            public const string keyboard = "/user/vive_tracker_htcx/role/keyboard";
        }

        /// <inheritdoc/>
        protected override void RegisterActionMapsWithRuntime()
        {
            ActionMapConfig actionMap = new ActionMapConfig()
            {
                name = "htcvivetracker",
                localizedName = kDeviceLocalizedNameTracker,
                desiredInteractionProfile = profileTracker,
                manufacturer = "HTC",
                serialNumber = "",
                deviceInfos = new List<DeviceConfig>()
                {
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftFoot,
                        userPath = TrackerUserPaths.leftFoot
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightFoot,
                        userPath = TrackerUserPaths.rightFoot
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftShoulder,
                        userPath = TrackerUserPaths.leftShoulder
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightShoulder,
                        userPath = TrackerUserPaths.rightShoulder
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftElbow,
                        userPath = TrackerUserPaths.leftElbow
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightElbow,
                        userPath = TrackerUserPaths.rightElbow
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerLeftKnee,
                        userPath = TrackerUserPaths.leftKnee
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerRightKnee,
                        userPath = TrackerUserPaths.rightKnee
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerWaist,
                        userPath = TrackerUserPaths.waist
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerChest,
                        userPath = TrackerUserPaths.chest
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerCamera,
                        userPath = TrackerUserPaths.camera
                    },
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics.TrackedDevice) | (InputDeviceCharacteristics)InputDeviceTrackerCharacteristics.TrackerKeyboard,
                        userPath = TrackerUserPaths.keyboard
                    }

                },
                actions = new List<ActionConfig>()
                {
                    new ActionConfig()
                    {
                        name = "devicePose",
                        localizedName = "Device Pose",
                        type = ActionType.Pose,
                        usages = new List<string>()
                        {
                            "Device",
                        },
                        bindings = new List<ActionBinding>()
                        {
                            new ActionBinding()
                            {
                                interactionPath = "/input/grip/pose",
                                interactionProfileName = profileTracker,
                            }
                        }
                    },
                }
            };

            AddActionMap(actionMap);

            List<ActionConfig> controllerActions = new List<ActionConfig>()
            {
                new ActionConfig()
                {
                    name = "grip",
                    localizedName = "Grip",
                    type = ActionType.Axis1D,
                    usages = new List<string>()
                    {
                        "Grip"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = squeeze,
                            interactionProfileName = profile,
                        }
                    }
                },
                new ActionConfig()
                {
                    name = "gripPressed",
                    localizedName = "Grip Pressed",
                    type = ActionType.Binary,
                    usages = new List<string>()
                    {
                        "GripButton"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = squeeze,
                            interactionProfileName = profile,
                        }
                    }
                },
                new ActionConfig()
                {
                    name = "menu",
                    localizedName = "Menu",
                    type = ActionType.Binary,
                    usages = new List<string>()
                    {
                        "MenuButton"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = menu,
                            interactionProfileName = profile,
                        }
                    }
                },
                new ActionConfig()
                {
                    name = "select",
                    localizedName = "Select",
                    type = ActionType.Binary,
                    usages = new List<string>()
                    {
                        "SystemButton"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = system,
                            interactionProfileName = profile,
                        }
                    }
                },
                new ActionConfig()
                {
                    name = "trigger",
                    localizedName = "Trigger",
                    type = ActionType.Axis1D,
                    usages = new List<string>()
                    {
                        "Trigger"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = trigger,
                            interactionProfileName = profile,
                        }
                    }
                },
                new ActionConfig()
                {
                    name = "triggerPressed",
                    localizedName = "Trigger Pressed",
                    type = ActionType.Binary,
                    usages = new List<string>()
                    {
                        "TriggerButton"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = triggerClick,
                            interactionProfileName = profile,
                        }
                    }
                },
                new ActionConfig()
                {
                    name = "trackpad",
                    localizedName = "Trackpad",
                    type = ActionType.Axis2D,
                    usages = new List<string>()
                    {
                        "Primary2DAxis"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = trackpad,
                            interactionProfileName = profile,
                        }
                    }
                },
                new ActionConfig()
                {
                    name = "trackpadTouched",
                    localizedName = "Trackpad Touched",
                    type = ActionType.Binary,
                    usages = new List<string>()
                    {
                        "Primary2DAxisTouch"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = trackpadTouch,
                            interactionProfileName = profile,
                        }
                    }
                },
                new ActionConfig()
                {
                    name = "trackpadClicked",
                    localizedName = "Trackpad Clicked",
                    type = ActionType.Binary,
                    usages = new List<string>()
                    {
                        "Primary2DAxisClick"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = trackpadClick,
                            interactionProfileName = profile,
                        }
                    }
                },
                new ActionConfig()
                {
                    name = "devicePose",
                    localizedName = "Device Pose",
                    type = ActionType.Pose,
                    usages = new List<string>()
                    {
                        "Device"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = grip,
                            interactionProfileName = profile,
                        }
                    }
                },
                // Pointer Pose
                new ActionConfig()
                {
                    name = "pointer",
                    localizedName = "Pointer Pose",
                    type = ActionType.Pose,
                    usages = new List<string>()
                    {
                        "Pointer"
                    },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = aim,
                            interactionProfileName = profile,
                        }
                    }
                },
                // Haptics
                new ActionConfig()
                {
                    name = "haptic",
                    localizedName = "Haptic Output",
                    type = ActionType.Vibrate,
                    usages = new List<string>() { "Haptic" },
                    bindings = new List<ActionBinding>()
                    {
                        new ActionBinding()
                        {
                            interactionPath = haptic,
                            interactionProfileName = profile,
                        }
                    }
                }
            };

            actionMap = new ActionMapConfig()
            {
                name = "htcvivecontrollerright",
                localizedName = kDeviceLocalizedNameRight,
                desiredInteractionProfile = profile,
                manufacturer = "HTC",
                serialNumber = "",
                deviceInfos = new List<DeviceConfig>()
                {
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics)(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right),
                        userPath = UserPaths.rightHand
                    }
                },
                actions = controllerActions
            };

            AddActionMap(actionMap);
            
            actionMap = new ActionMapConfig()
            {
                name = "htcvivecontrollerleft",
                localizedName = kDeviceLocalizedNameLeft,
                desiredInteractionProfile = profile,
                manufacturer = "HTC",
                serialNumber = "",
                deviceInfos = new List<DeviceConfig>()
                {
                    new DeviceConfig()
                    {
                        characteristics = (InputDeviceCharacteristics)(InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.TrackedDevice | InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left),
                        userPath = UserPaths.leftHand
                    }
                },
                actions = controllerActions                
            };
            AddActionMap(actionMap);            
        }
    }
}
