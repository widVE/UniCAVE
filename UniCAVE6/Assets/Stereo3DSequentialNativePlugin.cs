/*
 Stereoscopic 3D system by Vital Volkov — Sequential + NativeRenderingPlugin build

 Removed from original:
   - GUI settings panel (canvas, toggles, sliders, tooltips, dropdowns)
   - Save / Load settings
   - Cinemachine support
   - All non-Sequential output methods (Interlace, SideBySide, OverUnder,
     Anaglyph, Two_Displays, Direct3D11 blit path)
   - Win32 cursor-to-texture code

 Retained:
   - Sequential stereo render path (oddFrame, optimize, shader pass 5)
   - Native rendering plugin path (SetDataFromUnity / GetRenderEventFunc)
   - All three render pipelines: Default, URP, HDRP
   - Camera cloning, projection / FOV / IPD setup
   - Tracking
   - AdditionalS3DCameras
   - VSync helper
*/

//#define Debug

using UnityEngine;
using UnityEngine.Rendering;
#if URP
using UnityEngine.Rendering.Universal;
#elif HDRP
using UnityEngine.Rendering.HighDefinition;
#endif
using System.Reflection;
using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;

#if INPUT_SYSTEM && ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

#if POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

public class Stereo3DSequentialNativePlugin : MonoBehaviour
{
    public enum Method
    {
        Sequential  // only valid value in this build; enum kept for DataForPlugin compatibility
    };

    public enum EyePriority { Left, Center, Right };

    // -------------------------------------------------------------------------
    // Inspector settings
    // -------------------------------------------------------------------------
    [Header("Settings")]
    public bool S3DEnabled = true;
    public EyePriority eyePriority = EyePriority.Center;
    public bool swapLR;
    public bool optimize;
    public bool vSync = true;               // REQUIRED for Sequential
    public float PPI = 96;
    public float userIPD = 66;
    public float virtualIPD = 66;
    public float virtualIPDMax = 1000;
    public bool matchUserIPD = true;
    public float FOV = 90;
    public Vector2 FOVMinMax = new Vector2(10, 170);
    public bool tracking;
    public bool timeBasedSequential;
    public bool cloneCamera = true;
    public GameObject cameraPrefab;
    public RenderTextureFormat RTFormat;
    public bool debugLog;

    [Header("FPS Boost")]
    public bool blitToScreen = true;
    public bool disableCullingMask;
    public bool nearClipHack;
    public bool matrixKillHack;
    public bool disableMainCam;

    [Header("Info")]
    public Material S3DMaterial;

    // -------------------------------------------------------------------------
    // Additional cameras
    // -------------------------------------------------------------------------
    public List<Camera> additionalS3DCameras;
    public AdditionalS3DCamera[] additionalS3DCamerasStruct;

    // -------------------------------------------------------------------------
    // Private — cameras
    // -------------------------------------------------------------------------
    Camera cam;
    Camera camera_left;
    Camera camera_right;
    int sceneCullingMask;
    float nearClip;
    float sceneNearClip;
    float farClip;
    float sceneFarClip;
    float cameraBackupNearClip;
    int additionalS3DTopmostCameraIndex;
    Camera lastAdditionalS3DTopmostCamera;
    float bottommostCameraDepth;
    Camera topmostCamera;
    Camera topmostCamera_left;
    Camera topmostCamera_right;
    Vector3 leftCamLocalPos;
    Vector3 rightCamLocalPos;
    float aspect;
    float vFOV;
    float scaleX;
    float scaleY;
    float screenDistance;
    float imageWidth;
    Matrix4x4 camBackupMatrix;
    Matrix4x4 camDefaultMatrix;
    Vector2 camBackupLensShift;
    bool camFOVChangedExternal;

    // -------------------------------------------------------------------------
    // Private — render
    // -------------------------------------------------------------------------
    public RenderTexture renderTexture_left;
    public RenderTexture renderTexture_right;
    int rtWidth;
    int rtHeight;
    int pass = 5;               // Sequential shader pass
    bool oddFrame;
    bool nativeRenderingPlugin;
    bool blitToScreenClearRequired;
    Color clearScreenColor = Color.clear;
    Rect clientSizePixelRect;
    Rect camRectClamped;
    Rect camRectClampedPixels;
    bool borders;
    Vector2Int clientSize = new Vector2Int(Screen.width, Screen.height);
    Vector2Int viewportSize;
    Vector2Int screenSize;

    // Full-screen quad mesh
    Mesh screenBlitMesh;
    Mesh clientSizeMesh;
    Mesh clientSizeMeshFlippedY;

    static int[] quadIndicesClockwise = new int[] { 0, 1, 2, 3 };
    static Vector2[] quadVerticesUV = new Vector2[]
    {
        new Vector2(0,0), new Vector2(0,1), new Vector2(1,1), new Vector2(1,0)
    };
    static Vector3[] quadVertices = new Vector3[]
    {
        quadVerticesUV[0], quadVerticesUV[1], quadVerticesUV[2], quadVerticesUV[3]
    };

    CommandBuffer cb_main;
    CommandBuffer cb_left;
    CommandBuffer cb_right;

    delegate void NoInputDelegate();
    NoInputDelegate endOfFrameDelegate = delegate { };

    // -------------------------------------------------------------------------
    // Pipeline-specific
    // -------------------------------------------------------------------------
#if URP
    UniversalAdditionalCameraData camData;
    UniversalRenderPipelineAsset URPAsset;
    ScriptableRenderer lastScriptableRenderer;
    bool URPAssetIsReady;
    List<Camera> cameraStack;
    List<Camera> leftCameraStack;
    List<Camera> rightCameraStack;
    Camera[] lastCameraStack = new Camera[0];
#elif HDRP
    HDAdditionalCameraData camData;
    HDRenderPipelineAsset HDRPAsset;
    RenderPipelineSettings HDRPSettings;
    RenderPipelineSettings backupHDRPSettings;
    LayerMask volumeLayerMask;
    LayerMask probeLayerMask;
#else
    OnRenderImageDelegate onRenderImageDelegate_cameraMain;
    OnRenderImageDelegate onRenderImageDelegate_cameraLeft;
    OnRenderImageDelegate onRenderImageDelegate_cameraRight;
    NoInputDelegate camerasTargetRemoveDelegateLeft  = delegate { };
    NoInputDelegate camerasTargetRemoveDelegateRight = delegate { };
    Camera blitCamera_left;
    Camera blitCamera_right;
#endif

#if POST_PROCESSING_STACK_V2
    PostProcessLayer PPLayer;
    bool PPLayerDefaultStatus;
    bool PPLayerLastStatus;
#endif

    // -------------------------------------------------------------------------
    // Native plugin
    // -------------------------------------------------------------------------
    struct DataForPlugin
    {
        public static IntPtr textureHandleLeft;
        public static IntPtr textureHandleRight;
        public static bool S3DEnabled;
        public static UnityEngine.Experimental.Rendering.GraphicsFormat graphicsFormat;
        public static bool linear;
        public static Rect cameraPixelRect;
        public static bool secondWindow;
        public static bool mainCameraSecondWindow;
        public static bool leftCameraSecondWindow;
    }

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("RenderingPlugin")]
#endif
    private static extern void SetDataFromUnity(
        IntPtr textureLeft,
        IntPtr textureRight,
        bool S3DEnabled,
        // Method enum omitted — always Sequential; plugin receives int 0 equivalent
        int method,
        UnityEngine.Experimental.Rendering.GraphicsFormat graphicsFormat,
        bool linear,
        Rect camRectClampedPixels,
        bool secondWindow,
        bool mainCameraSecondWindow,
        bool leftCameraSecondWindow);

#if (UNITY_IPHONE || UNITY_WEBGL) && !UNITY_EDITOR
    [DllImport("__Internal")]
#else
    [DllImport("RenderingPlugin")]
#endif
    private static extern IntPtr GetRenderEventFunc();

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void RegisterPlugin();
#endif

    // -------------------------------------------------------------------------
    // Tracking
    // -------------------------------------------------------------------------
    public enum AxisSelect { X, inverted_X, Y, inverted_Y, Z, inverted_Z }

    [Serializable]
    public struct TrackingViewport
    {
        public Transform center;
        public AxisSelect upDirection;
        public AxisSelect forwardDirection;
        [HideInInspector] public Vector3 upAxis;
        [HideInInspector] public Vector3 forwardAxis;
        [HideInInspector] public Quaternion rotation;
    }
    public TrackingViewport trackingViewport = new TrackingViewport()
        { upDirection = AxisSelect.Y, forwardDirection = AxisSelect.Z };

    TrackingViewport lastTrackingViewport;
    GameObject trackingRotationHelper;
    Vector3 camBackupLocalPosition;
    Quaternion camBackupLocalRotation;
    float screenDistanceBackup;
    float camBackupFOV;
    Vector3 trackingPosition;
    Vector3 trackingViewportCenterPosition;
    const float halfDegreesToRadians = Mathf.PI / 360;
    const float halfRadiansToDegrees = 360 / Mathf.PI;
    float shiftX_left, shiftY_left;
    float shiftX_right, shiftY_right;
    float scaleX_left, scaleY_left;
    float scaleX_right, scaleY_right;
    bool lastTracking;

    // -------------------------------------------------------------------------
    // Change-detection snapshots
    // -------------------------------------------------------------------------
    bool lastS3DEnabled;
    EyePriority lastEyePriority;
    bool lastSwapLR;
    bool lastOptimize;
    bool lastVSync;
    float lastUserIPD;
    float lastVirtualIPD;
    float lastVirtualIPDMax;
    bool lastMatchUserIPD;
    float lastPPI;
    float lastFOV;
    Vector2 lastMinMaxFOV;
    bool lastGUIAsOverlay;
    GameObject lastCameraPrefab;
    RenderTextureFormat lastRTFormat;
    Rect lastCamRect;
    bool lastDisableCullingMask;
    bool lastNearClipHack;
    bool lastMatrixKillHack;
    bool lastDisableMainCam;
    bool lastCloneCamera;
    CameraDataStruct lastCameraDataStruct;
    bool lastLoadSettingsFromFile;
    Camera[] lastAdditionalS3DCameras = new Camera[0];
    bool onOffToggle;
#if !UNITY_2022_1_OR_NEWER
    bool lastFullscreen;
    Vector2 lastWindowedClientSize;
#endif
#if URP
    bool detectCameraSettingChange = true;
#elif HDRP
    bool detectCameraSettingChange = true;
#else
    bool detectCameraSettingChange = true;
#endif
    CameraDataStruct cameraDataStruct;

    // =========================================================================
    // Awake
    // =========================================================================
    void Awake()
    {
        if (debugLog) Debug.Log("Awake");

        cb_main = new CommandBuffer { name = "S3D Blit To Screen" };
        cb_left  = new CommandBuffer { name = "S3D Blit To Screen" };
        cb_right = new CommandBuffer { name = "S3D Blit To Screen" };

        clientSizeMesh = new Mesh();
        clientSizeMesh.vertices = quadVertices;
        clientSizeMesh.uv       = quadVerticesUV;
        clientSizeMesh.SetIndices(quadIndicesClockwise, MeshTopology.Quads, 0);

        clientSizeMeshFlippedY = new Mesh();
        clientSizeMeshFlippedY.vertices = quadVertices;
        clientSizeMeshFlippedY.uv = new Vector2[]
        {
            new Vector2(0,1), new Vector2(0,0), new Vector2(1,0), new Vector2(1,1)
        };
        clientSizeMeshFlippedY.SetIndices(quadIndicesClockwise, MeshTopology.Quads, 0);

        screenBlitMesh = new Mesh();

#if !UNITY_2022_1_OR_NEWER
        if (!Screen.fullScreen)
            lastWindowedClientSize = new Vector2(Screen.width, Screen.height);
#endif
    }

    // =========================================================================
    // OnEnable
    // =========================================================================
    void OnEnable()
    {
        if (debugLog) Debug.Log("OnEnable");

        clientSizePixelRect = new Rect(0, 0, clientSize.x, clientSize.y);

        S3DMaterial = new Material(Shader.Find("Stereo3D Screen Quad"));

        cam = GetComponent<Camera>();

#if POST_PROCESSING_STACK_V2
        if (GetComponent<PostProcessLayer>())
        {
            PPLayer = GetComponent<PostProcessLayer>();
            PPLayer.finalBlitToCameraTarget = false;
            PPLayerLastStatus = PPLayerDefaultStatus = PPLayer.enabled;
        }
#endif

        cam.orthographic = false;
#if !(URP || HDRP)
        cam.stereoTargetEye = StereoTargetEyeMask.None;
#endif
        sceneCullingMask     = cam.cullingMask;
        camBackupLensShift   = cam.lensShift;
        cameraBackupNearClip = cam.nearClipPlane;

        // --- Create left / right cameras ---
        if (cameraPrefab)
        {
            camera_left  = Instantiate(cameraPrefab, transform.position, transform.rotation).GetComponent<Camera>();
            camera_left.name  = "prefabCamera_left";
            camera_right = Instantiate(cameraPrefab, transform.position, transform.rotation).GetComponent<Camera>();
            camera_right.name = "prefabCamera_right";
        }
        else if (cloneCamera)
        {
            camera_left  = Instantiate(cam, transform.position, transform.rotation);
            camera_left.tag  = "Untagged";
            camera_left.name += "_left";
            camera_right = Instantiate(cam, transform.position, transform.rotation);
            camera_right.tag  = "Untagged";
            camera_right.name += "_right";

            foreach (var component in cam.GetComponents(typeof(Component)))
                if (!(component is Transform) && !(component is Camera)
#if URP
                    && !(component is UniversalAdditionalCameraData)
#elif HDRP
                    && !(component is HDAdditionalCameraData)
#elif POST_PROCESSING_STACK_V2
                    && !(component is PostProcessLayer)
#endif
                    )
                {
                    Destroy(camera_left.GetComponent(component.GetType()));
                    Destroy(camera_right.GetComponent(component.GetType()));
                }

            for (int i = 0; i < camera_left.transform.childCount; i++)
            {
                Destroy(camera_left.transform.GetChild(i).gameObject);
                Destroy(camera_right.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            //instead grab already existing camera...

            camera_left  = new GameObject("camera_left").AddComponent<Camera>();
            camera_right = new GameObject("camera_right").AddComponent<Camera>();
            camera_left.CopyFrom(cam);
            camera_right.CopyFrom(cam);
        }

        bottommostCameraDepth = camera_left.depth = camera_right.depth =
            cam.depth - 1 - additionalS3DCameras.Count;
        camera_left.transform.parent  = camera_right.transform.parent = transform;
        camera_left.usePhysicalProperties = camera_right.usePhysicalProperties = cam.usePhysicalProperties;
#if !(URP || HDRP)
        camera_left.stereoTargetEye  = StereoTargetEyeMask.Left;
        camera_right.stereoTargetEye = StereoTargetEyeMask.Right;
#endif

        if (Screen.dpi != 0) PPI = Screen.dpi;

        additionalS3DCamerasStruct      = new AdditionalS3DCamera[additionalS3DCameras.Count];
        additionalS3DTopmostCameraIndex = -1;
        lastAdditionalS3DTopmostCamera  = null;

#if URP
        camData          = cam.GetUniversalAdditionalCameraData();
        cameraStack      = camData.cameraStack;
        leftCameraStack  = camera_left.GetUniversalAdditionalCameraData().cameraStack;
        rightCameraStack = camera_right.GetUniversalAdditionalCameraData().cameraStack;
        leftCameraStack.RemoveAll(t => t);
        rightCameraStack.RemoveAll(t => t);
        URPAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        Invoke("URPAssetSettings_Get", Time.deltaTime * 2);
#elif HDRP
        camData = cam.GetComponent<HDAdditionalCameraData>();
        volumeLayerMask = camData.volumeLayerMask;
        probeLayerMask  = camData.probeLayerMask;
        HDRPAsset = GraphicsSettings.currentRenderPipeline as HDRenderPipelineAsset;
        backupHDRPSettings = HDRPSettings = HDRPAsset.currentPlatformRenderPipelineSettings;
        HDRPSettings.colorBufferFormat = RenderPipelineSettings.ColorBufferFormat.R16G16B16A16;
        typeof(HDRenderPipelineAsset)
            .GetField("m_RenderPipelineSettings", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(GraphicsSettings.currentRenderPipeline, HDRPSettings);
#endif

        // --- Additional S3D cameras ---
        for (int i = 0; i < additionalS3DCameras.Count; i++)
        {
            Camera c = additionalS3DCameras[i];
            if (!c) continue;

#if !(URP || HDRP)
            c.stereoTargetEye = StereoTargetEyeMask.None;
#endif
            additionalS3DTopmostCameraIndex = i;
            lastAdditionalS3DTopmostCamera  = c;

            if (c.transform.Find(c.name + "(Clone)_left"))
            {
                additionalS3DCamerasStruct[i].camera       = c;
                additionalS3DCamerasStruct[i].camera_left  = c.transform.Find(c.name + "(Clone)_left").GetComponent<Camera>();
                additionalS3DCamerasStruct[i].camera_right = c.transform.Find(c.name + "(Clone)_right").GetComponent<Camera>();
            }
            else
            {
                c.depth = cam.depth + 1 + i;
                additionalS3DCamerasStruct[i].camera              = c;
                additionalS3DCamerasStruct[i].cameraBackupMatrix  = c.projectionMatrix;
                additionalS3DCamerasStruct[i].cameraDefaultMatrix = c.projectionMatrix;
                c.usePhysicalProperties = cam.usePhysicalProperties;

#if HDRP
                var acd = c.GetComponent<HDAdditionalCameraData>();
                if (acd.clearColorMode != HDAdditionalCameraData.ClearColorMode.Color)
                {
                    acd.clearColorMode    = HDAdditionalCameraData.ClearColorMode.Color;
                    acd.backgroundColorHDR = Color.clear;
                }
#elif !URP
                c.clearFlags = CameraClearFlags.Depth;
#endif

#if POST_PROCESSING_STACK_V2
                if (c.GetComponent<PostProcessLayer>())
                {
                    additionalS3DCamerasStruct[i].PPLayer = c.GetComponent<PostProcessLayer>();
                    additionalS3DCamerasStruct[i].PPLayer.finalBlitToCameraTarget = false;
                    additionalS3DCamerasStruct[i].PPLayerLastStatus = additionalS3DCamerasStruct[i].PPLayer.enabled;
                }
#endif
                Camera cl = Instantiate(c, c.transform.position, c.transform.rotation);
                cl.tag = "Untagged"; cl.name += "_left";
                Camera cr = Instantiate(c, c.transform.position, c.transform.rotation);
                cr.tag = "Untagged"; cr.name += "_right";

                cl.depth = cr.depth = bottommostCameraDepth + 1 + i;
#if !(URP || HDRP)
                cl.stereoTargetEye = StereoTargetEyeMask.Left;
                cr.stereoTargetEye = StereoTargetEyeMask.Right;
#endif
                cl.transform.parent = cr.transform.parent = c.transform;
                additionalS3DCamerasStruct[i].camera_left  = cl;
                additionalS3DCamerasStruct[i].camera_right = cr;
            }
        }

        TopMostCamera_Set();
        SceneNearClip_Set();
        SceneFarClip_Set();
        nearClip = sceneNearClip;
        farClip  = sceneFarClip;

#if URP
        CameraStackSet();
#endif

        // --- Check native plugin ---
        // Sequential + Direct3D11 = native plugin path
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11 && blitToScreen)
            nativeRenderingPlugin = true;

        if (cam.targetDisplay != 0)
        {
            DataForPlugin.secondWindow           = true;
            DataForPlugin.mainCameraSecondWindow = true;
        }

        // Snapshot change-detection state
        lastS3DEnabled    = S3DEnabled;
        lastEyePriority   = eyePriority;
        lastSwapLR        = swapLR;
        lastOptimize      = optimize;
        lastVSync         = vSync;
        lastPPI           = PPI;
        lastUserIPD       = userIPD;
        lastVirtualIPD    = virtualIPD;
        lastVirtualIPDMax = virtualIPDMax;
        lastMatchUserIPD  = matchUserIPD;
        lastFOV           = FOV;
        lastMinMaxFOV     = FOVMinMax;
        lastCamRect       = cam.rect;
        lastDisableCullingMask = disableCullingMask;
        lastNearClipHack  = nearClipHack;
        lastMatrixKillHack = matrixKillHack;
        lastDisableMainCam = disableMainCam;
        lastCloneCamera   = cloneCamera;
        lastTracking      = tracking;
        lastAdditionalS3DCameras = additionalS3DCameras.ToArray();
        lastTrackingViewport     = trackingViewport;

#if !UNITY_EDITOR
        if (cam.targetDisplay < Display.displays.Length)
        {
            if (!Display.displays[cam.targetDisplay].active)
                Display.displays[cam.targetDisplay].Activate();
        }
        else cam.targetDisplay = 0;

        screenSize.x = Display.displays[cam.targetDisplay].systemWidth;
        screenSize.y = Display.displays[cam.targetDisplay].systemHeight;
#else
        screenSize.x = Screen.currentResolution.width;
        screenSize.y = Screen.currentResolution.height;
#endif

        if (tracking && trackingViewport.center) TrackingViewportAxis_Set();

        VSync_Set();
        Aspect_Set();
        S3DCamerasPosition_Set();
        Clip_Set();

        CameraDataStruct_Set();
        LastCameraDataStruct_Set();

        //trackingRotationHelper = new GameObject("trackingRotationHelper");
        //trackingRotationHelper.transform.SetParent(transform, false);
        BackupCamPosition();
    }

    // =========================================================================
    // Start
    // =========================================================================
    void Start()
    {
        if (debugLog) Debug.Log("Start");

#if UNITY_2020_1_OR_NEWER && !UNITY_2023_1_OR_NEWER
        for (int i = 1; i < Display.displays.Length; i++)
            Display.displays[i].Activate();
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        RegisterPlugin();
#endif

        StartCoroutine(EndOfFrame());
    }

    private IEnumerator EndOfFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            endOfFrameDelegate();

            if (nativeRenderingPlugin)
                GL.IssuePluginEvent(GetRenderEventFunc(), 1);
        }
    }

    // =========================================================================
    // Update
    // =========================================================================
    void Update()
    {
#if UNITY_EDITOR
        screenSize.x = Screen.currentResolution.width;
        screenSize.y = Screen.currentResolution.height;
#endif

        // --- Sequential frame parity ---
        if (timeBasedSequential)
        {
#if UNITY_2020_2_OR_NEWER
            double t = Time.timeAsDouble;
#else
            double t = Time.time;
#endif
#if UNITY_2022_2_OR_NEWER
            double rr = Screen.currentResolution.refreshRateRatio.value;
#else
            int rr = Screen.currentResolution.refreshRate;
#endif
            uint rn = (uint)(t * rr);
            oddFrame = (rn - rn / 2 * 2) == 1;
        }
        else
            oddFrame = !oddFrame;

        // --- Drive sequential shader ---
        if (S3DEnabled && !nativeRenderingPlugin)
        {
            S3DMaterial.SetInt("_OddFrame", oddFrame ? 1 : 0);

#if !URP
            if (optimize && oddFrame)
            {
                camera_left.Render();
                camera_right.Render();
                foreach (var c in additionalS3DCamerasStruct)
                    if (c.camera) { c.camera_left.Render(); c.camera_right.Render(); }
            }
#endif
        }

        // --- Resize ---
        if (Screen.width != clientSize.x || Screen.height != clientSize.y)
        {
#if !UNITY_2022_1_OR_NEWER
            if (!Screen.fullScreen) lastWindowedClientSize = new Vector2(Screen.width, Screen.height);
#endif
            Resize();
        }
#if !UNITY_2022_1_OR_NEWER
        else if (lastFullscreen != Screen.fullScreen)
        {
            lastFullscreen = Screen.fullScreen;
            if (!Screen.fullScreen && lastWindowedClientSize != Vector2.zero)
                Screen.SetResolution((int)lastWindowedClientSize.x, (int)lastWindowedClientSize.y, false);
            Aspect_Set();
        }
#endif

        // --- Change detection ---
        if (lastS3DEnabled != S3DEnabled)
        {
            lastS3DEnabled = S3DEnabled;
            Aspect_Set(); Clip_Set();
#if URP
            CameraStackSet();
#endif
        }

        if (lastEyePriority != eyePriority) onOffToggle = true;

        if (lastSwapLR != swapLR) { lastSwapLR = swapLR; S3DCamerasPosition_Set(); }

        if (lastOptimize != optimize) { lastOptimize = optimize; Aspect_Set(); }

        if (lastVSync != vSync) { lastVSync = vSync; VSync_Set(); }

        if (cam.fieldOfView != vFOV) { camFOVChangedExternal = true; FOV_Set(); }
        else if (lastFOV != FOV)     FOV_Set();

        if (lastMinMaxFOV != FOVMinMax) lastMinMaxFOV = FOVMinMax;

        if (lastPPI != PPI)             { PPI = Mathf.Clamp(PPI, 1, 1000); lastPPI = PPI; S3DProjection_Set(); }
        if (lastUserIPD != userIPD)     { UserIPD_Clamp(); S3DProjection_Set(); lastUserIPD = userIPD; }
        if (lastVirtualIPD != virtualIPD) { VirtualIPD_Clamp(); S3DCamerasPosition_Set(); lastVirtualIPD = virtualIPD; }
        if (lastVirtualIPDMax != virtualIPDMax) { lastVirtualIPDMax = virtualIPDMax; }
        if (lastMatchUserIPD != matchUserIPD) { lastMatchUserIPD = matchUserIPD; VirtualIPD_Clamp(); S3DCamerasPosition_Set(); }
        if (lastCamRect != cam.rect)    { lastCamRect = cam.rect; Aspect_Set(); }
        if (lastDisableCullingMask != disableCullingMask) onOffToggle = true;
        if (lastNearClipHack != nearClipHack)   onOffToggle = true;
        if (lastMatrixKillHack != matrixKillHack) onOffToggle = true;
        if (lastDisableMainCam != disableMainCam) onOffToggle = true;
        if (lastCloneCamera != cloneCamera) { lastCloneCamera = cloneCamera; onOffToggle = true; }

        if (lastAdditionalS3DCameras.Length != additionalS3DCameras.Count)
            onOffToggle = true;
        else
            for (int i = 0; i < lastAdditionalS3DCameras.Length; i++)
                if (lastAdditionalS3DCameras[i] != additionalS3DCameras[i]) { onOffToggle = true; break; }

#if HDRP
        if (detectCameraSettingChange && !HDRPSettings.Equals(HDRPAsset.currentPlatformRenderPipelineSettings))
        {
            backupHDRPSettings = HDRPAsset.currentPlatformRenderPipelineSettings;
            onOffToggle = true;
        }
#elif URP
        if (detectCameraSettingChange)
        {
            if (lastCameraStack.Length != cameraStack.Count)
                onOffToggle = true;
            else
                for (int i = 0; i < lastCameraStack.Length; i++)
                    if (lastCameraStack[i] != cameraStack[i]) { onOffToggle = true; break; }

            if (URPAsset != GraphicsSettings.currentRenderPipeline ||
                URPAssetIsReady && !lastScriptableRenderer.Equals(URPAsset.scriptableRenderer))
                onOffToggle = true;
        }
#endif

#if POST_PROCESSING_STACK_V2
        if (detectCameraSettingChange && PPLayer && PPLayer.enabled != PPLayerLastStatus)
        {
            PPLayerDefaultStatus = PPLayer.enabled;
            onOffToggle = true;
        }
#endif

        if (onOffToggle)
        {
            if (debugLog) Debug.Log("OnOffToggle");
            onOffToggle = false;
            enabled = false;
            Invoke("Enable", Time.deltaTime * 4);
        }
    }

    // =========================================================================
    // LateUpdate
    // =========================================================================
    void LateUpdate()
    {
        Tracking_Set();
        CameraDataStruct_Set();

        if (detectCameraSettingChange && !lastCameraDataStruct.Equals(cameraDataStruct))
        {
            onOffToggle = true;
            if (lastCameraDataStruct.cullingMask != cameraDataStruct.cullingMask)
                sceneCullingMask = cameraDataStruct.cullingMask;
            if (lastCameraDataStruct.farClipPlane != cameraDataStruct.farClipPlane)
                sceneFarClip = cam.farClipPlane;
            else if (lastCameraDataStruct.nearClipPlane != cameraDataStruct.nearClipPlane)
                cameraBackupNearClip = cam.nearClipPlane;
            lastCameraDataStruct = cameraDataStruct;
        }

        for (int i = 0; i < additionalS3DCamerasStruct.Length; i++)
            if (!additionalS3DCamerasStruct[i].lastCameraDataStruct.Equals(additionalS3DCamerasStruct[i].cameraDataStruct))
            {
                onOffToggle = true;
                additionalS3DCamerasStruct[i].lastCameraDataStruct = additionalS3DCamerasStruct[i].cameraDataStruct;
            }
    }

    // =========================================================================
    // Render_Set — Sequential only
    // =========================================================================
    void Render_Set()
    {
        if (debugLog) Debug.Log("Render_Set (Sequential)");

        Render_Release();

        rtWidth  = viewportSize.x;
        rtHeight = viewportSize.y;
        pass     = 5;   // Sequential shader pass

        if (S3DEnabled)
        {
#if POST_PROCESSING_STACK_V2
            if (PPLayer) PPLayerLastStatus = PPLayer.enabled = false;
#endif
            if (disableCullingMask) cam.cullingMask = 0;

#if HDRP
            camData.volumeLayerMask = 0;
            camData.probeLayerMask  = 0;
#endif
            camera_left.enabled  = true;
            camera_right.enabled = true;

            for (int i = 0; i < additionalS3DCamerasStruct.Length; i++)
                if (additionalS3DCamerasStruct[i].camera)
                {
                    additionalS3DCamerasStruct[i].camera.enabled       = false;
                    additionalS3DCamerasStruct[i].camera_left.enabled  = true;
                    additionalS3DCamerasStruct[i].camera_right.enabled = true;
                }

#if !URP
            if (optimize)
            {
                camera_left.enabled  = false;
                camera_right.enabled = false;
                foreach (var c in additionalS3DCamerasStruct)
                    if (c.camera) { c.camera_left.enabled = false; c.camera_right.enabled = false; }
            }
#endif

            renderTexture_left  = RT_Make(renderTexture_left);
            renderTexture_right = RT_Make(renderTexture_right);
            renderTexture_left.Create();
            renderTexture_right.Create();

            S3DMaterial.SetTexture("_LeftTex",  renderTexture_left);
            S3DMaterial.SetTexture("_RightTex", renderTexture_right);

            if (nativeRenderingPlugin)
                SetTexturesForPlugin(renderTexture_left, renderTexture_right);

            TargetDisplays_Set(cam.targetDisplay, cam.targetDisplay);
            SetClientSizeRect();
            AssignRenderTextures_S3DOn();
            RenderTextureContextSet();

            if (disableMainCam || (blitTime_IsEndOfFrame() && disableMainCam))
            {
                cam.enabled = false;
                if (eyePriority == EyePriority.Left) camera_left.tag  = "MainCamera";
                else                                 camera_right.tag = "MainCamera";
            }
        }
        else
        {
            cameraRestore();
            S3DMaterial.SetFloat("_ShiftX", 0);

            if (nativeRenderingPlugin || !blitToScreen)
            {
                AssignRenderTextures_S3DOff();
                if (nativeRenderingPlugin) SetTexturesForPlugin(renderTexture_left, null);
                RenderTextureContextSet();
            }
        }

        if (detectCameraSettingChange)
        {
            lastCameraDataStruct.cullingMask   = cam.cullingMask;
            lastCameraDataStruct.targetDisplay = cam.targetDisplay;
#if HDRP
            lastCameraDataStruct.volumeLayerMask = camData.volumeLayerMask;
            lastCameraDataStruct.probeLayerMask  = camData.probeLayerMask;
#endif
        }
    }

    bool blitTime_IsEndOfFrame()
    {
        // In this stripped build we always use OnEndOfFrame for the sequential path
        return true;
    }

    // =========================================================================
    // RenderTextureContextSet — Sequential
    // =========================================================================
    void RenderTextureContextSet()
    {
        if (debugLog) Debug.Log("RenderTextureContextSet (Sequential)");

        // Remove all previously wired delegates first
#if URP || HDRP
#if UNITY_2021_1_OR_NEWER
        RenderPipelineManager.endContextRendering   -= SingleDisplayBlitToScreen_EndContextRendering;
        RenderPipelineManager.beginContextRendering -= RenderTexture_Reset_BeginContextRendering;
        RenderPipelineManager.endContextRendering   -= BlitToRenderTexture_EndContextRendering;
#else
        RenderPipelineManager.endFrameRendering   -= SingleDisplayBlitToScreen_EndFrameRendering;
        RenderPipelineManager.beginFrameRendering -= RenderTexture_Reset_BeginFrameRendering;
        RenderPipelineManager.endFrameRendering   -= BlitToRenderTexture_EndFrameRendering;
#endif
#else
        if (onRenderImageDelegate_cameraMain)
        {
            onRenderImageDelegate_cameraMain.RenderImageEvent -= SingleDisplayBlitToScreen_OnRenderImageDelegate;
            onRenderImageDelegate_cameraMain.RenderImageEvent -= SetTexture_OnRenderImageDelegate;
        }
        if (onRenderImageDelegate_cameraLeft)
        {
            onRenderImageDelegate_cameraLeft.RenderImageEvent  -= BlitSourceToRenderTextureLeft_OnRenderImageDelegate;
            onRenderImageDelegate_cameraRight.RenderImageEvent -= BlitSourceToRenderTextureRight_OnRenderImageDelegate;
            Destroy(onRenderImageDelegate_cameraLeft);
            Destroy(onRenderImageDelegate_cameraRight);
            onRenderImageDelegate_cameraLeft  = null;
            onRenderImageDelegate_cameraRight = null;
        }
        Camera.onPreRender  -= RenderTexture_Reset_OnPreRender;
        Camera.onPostRender -= SingleDisplayBlitToScreen_OnPostRender;
        camerasTargetRemoveDelegateLeft  = delegate { };
        camerasTargetRemoveDelegateRight = delegate { };
#endif
        endOfFrameDelegate = delegate { };

        camera_left.targetTexture  = null;
        camera_right.targetTexture = null;
        camera_left.rect = camera_right.rect = cam.rect;
        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera) { c.camera_left.targetTexture = null; c.camera_right.targetTexture = null; c.camera_left.rect = c.camera_right.rect = cam.rect; }

        S3DMaterial.SetInt("_Flipped", 0);

        if (!S3DEnabled || renderTexture_left == null) return;

        AssignRenderTextures_S3DOn();
        SetClientSizeRect();

        // Native plugin: plugin handles the blit; we only need to push data and
        // issue the plugin event (done in EndOfFrame coroutine).
        if (nativeRenderingPlugin)
        {
            endOfFrameDelegate += NativePlugin_EndOfFrame;
            return;
        }

        // Software sequential blit
#if URP || HDRP
#if UNITY_2021_1_OR_NEWER
        RenderPipelineManager.beginContextRendering += RenderTexture_Reset_BeginContextRendering;
        RenderPipelineManager.endContextRendering   += SingleDisplayBlitToScreen_EndContextRendering;
#else
        RenderPipelineManager.beginFrameRendering += RenderTexture_Reset_BeginFrameRendering;
        RenderPipelineManager.endFrameRendering   += SingleDisplayBlitToScreen_EndFrameRendering;
#endif
#else
        onRenderImageDelegate_cameraMain = cam.gameObject.AddComponent<OnRenderImageDelegate>();

        onRenderImageDelegate_cameraLeft  = topmostCamera_left.gameObject.AddComponent<OnRenderImageDelegate>();
        onRenderImageDelegate_cameraRight = topmostCamera_right.gameObject.AddComponent<OnRenderImageDelegate>();
        Camera.onPreRender += RenderTexture_Reset_OnPreRender;
        onRenderImageDelegate_cameraLeft.RenderImageEvent  += BlitSourceToRenderTextureLeft_OnRenderImageDelegate;
        onRenderImageDelegate_cameraRight.RenderImageEvent += BlitSourceToRenderTextureRight_OnRenderImageDelegate;
        onRenderImageDelegate_cameraMain.RenderImageEvent  += SingleDisplayBlitToScreen_OnRenderImageDelegate;
#endif

        endOfFrameDelegate += SingleDisplayBlitToScreen_OnEndOfFrame;
    }

    // =========================================================================
    // Native plugin helpers
    // =========================================================================
    void SetTexturesForPlugin(RenderTexture rtLeft, RenderTexture rtRight)
    {
        if (debugLog) Debug.Log("SetTexturesForPlugin");

        DataForPlugin.textureHandleLeft  = IntPtr.Zero;
        DataForPlugin.textureHandleRight = IntPtr.Zero;

        if (rtLeft)
        {
            DataForPlugin.graphicsFormat    = rtLeft.graphicsFormat;
            DataForPlugin.textureHandleLeft = rtLeft.GetNativeTexturePtr();
        }
        if (rtRight)
            DataForPlugin.textureHandleRight = rtRight.GetNativeTexturePtr();

        SendDataToPlugin();
    }

    void SendDataToPlugin()
    {
        DataForPlugin.S3DEnabled      = S3DEnabled;
        DataForPlugin.linear          = QualitySettings.activeColorSpace == ColorSpace.Linear;
        DataForPlugin.cameraPixelRect = camRectClampedPixels;

        SetDataFromUnity(
            DataForPlugin.textureHandleLeft,
            DataForPlugin.textureHandleRight,
            DataForPlugin.S3DEnabled,
            12, // Sequential enum value as int (matches original Method.Sequential index)
            DataForPlugin.graphicsFormat,
            DataForPlugin.linear,
            DataForPlugin.cameraPixelRect,
            DataForPlugin.secondWindow,
            DataForPlugin.mainCameraSecondWindow,
            DataForPlugin.leftCameraSecondWindow);
    }

    void NativePlugin_EndOfFrame()
    {
        // Keep render textures assigned so the plugin always has valid handles
        AssignRenderTextures_S3DOn();
        SendDataToPlugin();
    }

    // =========================================================================
    // Render callbacks
    // =========================================================================
    void SingleDisplayBlitToScreen()
    {
        CustomBlitScreen(null, null, screenBlitMesh, S3DMaterial, pass, blitToScreenClearRequired, clientSizePixelRect);
    }

    void SingleDisplayBlitToScreen_OnEndOfFrame()        => SingleDisplayBlitToScreen();
    void SingleDisplayBlitToScreen_OnPostRender(Camera c){ if (c == cam) SingleDisplayBlitToScreen(); }
    void SingleDisplayBlitToScreen_OnRenderImageDelegate(RenderTexture s, RenderTexture d, Camera c)
        => SingleDisplayBlitToScreen();

#if URP || HDRP
#if UNITY_2021_1_OR_NEWER
    void SingleDisplayBlitToScreen_EndContextRendering(ScriptableRenderContext ctx, List<Camera> list)
    { foreach (var c in list) if (c == cam) SingleDisplayBlitToScreen(); }

    void RenderTexture_Reset_BeginContextRendering(ScriptableRenderContext ctx, List<Camera> list)
    {
        foreach (var c in list)
        {
            if (c == camera_left)  c.targetTexture = renderTexture_left;
            else if (c == camera_right) c.targetTexture = renderTexture_right;
        }
    }

    void BlitToRenderTexture_EndContextRendering(ScriptableRenderContext ctx, List<Camera> list) { }
#else
    void SingleDisplayBlitToScreen_EndFrameRendering(ScriptableRenderContext ctx, Camera[] arr)
    { foreach (var c in arr) if (c == cam) SingleDisplayBlitToScreen(); }

    void RenderTexture_Reset_BeginFrameRendering(ScriptableRenderContext ctx, Camera[] arr)
    {
        foreach (var c in arr)
        {
            if (c == camera_left)  c.targetTexture = renderTexture_left;
            else if (c == camera_right) c.targetTexture = renderTexture_right;
        }
    }

    void BlitToRenderTexture_EndFrameRendering(ScriptableRenderContext ctx, Camera[] arr) { }
#endif
#else
    void RenderTexture_Reset_OnPreRender(Camera c)
    {
        if (c.name.Contains("_left"))  c.targetTexture = renderTexture_left;
        else if (c.name.Contains("_right")) c.targetTexture = renderTexture_right;
    }

    void BlitSourceToRenderTextureLeft_OnRenderImageDelegate(RenderTexture s, RenderTexture d, Camera c)
        => Graphics.Blit(s, renderTexture_left);

    void BlitSourceToRenderTextureRight_OnRenderImageDelegate(RenderTexture s, RenderTexture d, Camera c)
        => Graphics.Blit(s, renderTexture_right);

    void SetTexture_OnRenderImageDelegate(RenderTexture s, RenderTexture d, Camera c)
    {
        renderTexture_left = RenderTexture.active as RenderTexture ?? renderTexture_left;
        if (nativeRenderingPlugin) SetTexturesForPlugin(renderTexture_left, null);
        onRenderImageDelegate_cameraMain.RenderImageEvent -= SetTexture_OnRenderImageDelegate;
    }
#endif

    // =========================================================================
    // Blit utilities
    // =========================================================================
    void CustomBlitScreen(RenderTexture source, RenderTexture destination, Mesh blitMesh,
        Material material, int pass, bool clear,
        [System.Runtime.InteropServices.Optional] Rect rect)
    {
        GL.PushMatrix();
        if (clear) { RenderTexture.active = null; GL.Viewport(clientSizePixelRect); GL.Clear(true, true, clearScreenColor); }
        RenderTexture.active = destination;
        if (material) { material.SetTexture("_MainTex", source); material.SetPass(pass); }
        if (rect != Rect.zero) GL.Viewport(rect);
        GL.LoadOrtho();
        Graphics.DrawMeshNow(blitMesh, Matrix4x4.identity);
        GL.PopMatrix();
    }

    // =========================================================================
    // Camera / render helpers
    // =========================================================================
    void AssignRenderTextures_S3DOff()
    {
        var rt = RT_Make(null);
        cam.targetTexture = rt;
        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera) c.camera.targetTexture = rt;
    }

    void AssignRenderTextures_S3DOn()
    {
        camera_left.targetTexture  = renderTexture_left;
        camera_right.targetTexture = renderTexture_right;
        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera)
            {
                c.camera_left.targetTexture  = renderTexture_left;
                c.camera_right.targetTexture = renderTexture_right;
            }
    }

    void SetClientSizeRect()
    {
        camera_left.rect = camera_right.rect = Rect.MinMaxRect(0, 0, 1, 1);
        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera) c.camera_left.rect = c.camera_right.rect = Rect.MinMaxRect(0, 0, 1, 1);
    }

    void TargetDisplays_Set(int left, int right)
    {
        camera_left.targetDisplay  = left;
        camera_right.targetDisplay = right;
        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera)
            {
                c.camera.targetDisplay       = cam.targetDisplay;
                c.camera_left.targetDisplay  = left;
                c.camera_right.targetDisplay = right;
            }
    }

    void TopMostCamera_Set()
    {
        if (additionalS3DTopmostCameraIndex != -1)
        {
            topmostCamera       = additionalS3DCamerasStruct[additionalS3DTopmostCameraIndex].camera;
            topmostCamera_left  = additionalS3DCamerasStruct[additionalS3DTopmostCameraIndex].camera_left;
            topmostCamera_right = additionalS3DCamerasStruct[additionalS3DTopmostCameraIndex].camera_right;
        }
        else
        {
            topmostCamera       = cam;
            topmostCamera_left  = camera_left;
            topmostCamera_right = camera_right;
        }
#if !(URP || HDRP)
        blitCamera_left  = topmostCamera_left;
        blitCamera_right = topmostCamera_right;
#endif
    }

    void cameraRestore()
    {
        camera_left.enabled = camera_right.enabled = false;
        cam.enabled = true;
        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera) { c.camera_left.enabled = c.camera_right.enabled = false; c.camera.enabled = true; }
        cam.cullingMask = sceneCullingMask;
#if HDRP
        camData.volumeLayerMask = volumeLayerMask;
        camData.probeLayerMask  = probeLayerMask;
#endif
    }

    void Render_Release()
    {
        if (debugLog) Debug.Log("Render_Release");

#if URP || HDRP
#if UNITY_2021_1_OR_NEWER
        RenderPipelineManager.endContextRendering   -= SingleDisplayBlitToScreen_EndContextRendering;
        RenderPipelineManager.beginContextRendering -= RenderTexture_Reset_BeginContextRendering;
        RenderPipelineManager.endContextRendering   -= BlitToRenderTexture_EndContextRendering;
#else
        RenderPipelineManager.endFrameRendering   -= SingleDisplayBlitToScreen_EndFrameRendering;
        RenderPipelineManager.beginFrameRendering -= RenderTexture_Reset_BeginFrameRendering;
        RenderPipelineManager.endFrameRendering   -= BlitToRenderTexture_EndFrameRendering;
#endif
#else
        Destroy(onRenderImageDelegate_cameraMain);
        onRenderImageDelegate_cameraMain = null;
        Camera.onPreRender  -= RenderTexture_Reset_OnPreRender;
        Camera.onPostRender -= SingleDisplayBlitToScreen_OnPostRender;
#endif
        endOfFrameDelegate = delegate { };

        camera_left.targetTexture = camera_right.targetTexture = cam.targetTexture = null;
        if (renderTexture_left)  { renderTexture_left.Release();  renderTexture_left  = null; }
        if (renderTexture_right) { renderTexture_right.Release(); renderTexture_right = null; }

        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera)
                c.camera_left.targetTexture = c.camera_right.targetTexture = c.camera.targetTexture = null;

#if POST_PROCESSING_STACK_V2
        if (PPLayer) PPLayerLastStatus = PPLayer.enabled = PPLayerDefaultStatus;
#endif
    }

    RenderTexture RT_Make(RenderTexture old,
        [System.Runtime.InteropServices.Optional] RenderTextureFormat fmtOverride)
    {
        if (old) old.Release();
        RenderTextureFormat fmt = (fmtOverride != 0) ? fmtOverride : RTFormat;
        var rt = new RenderTexture(rtWidth, rtHeight, 24, fmt);
        rt.filterMode = FilterMode.Point;
#if URP
        if (URPAsset != null && URPAsset.msaaSampleCount != 0) rt.antiAliasing = URPAsset.msaaSampleCount;
#elif HDRP
        if ((int)HDRPSettings.msaaSampleCount != 0) rt.antiAliasing = (int)HDRPSettings.msaaSampleCount;
#else
        if (QualitySettings.antiAliasing != 0) rt.antiAliasing = QualitySettings.antiAliasing;
#endif
        return rt;
    }

    // =========================================================================
    // Clip, Projection, Aspect, FOV, IPD
    // =========================================================================
    void SceneNearClip_Set()
    {
        sceneNearClip = (additionalS3DTopmostCameraIndex != -1)
            ? additionalS3DCameras[additionalS3DTopmostCameraIndex].nearClipPlane
            : cameraBackupNearClip;
    }

    void SceneFarClip_Set() { sceneFarClip = cam.farClipPlane; }

    void Clip_Set()
    {
        nearClip = sceneNearClip;
        farClip  = sceneFarClip;

        if (additionalS3DTopmostCameraIndex != -1)
        {
            additionalS3DCamerasStruct[additionalS3DTopmostCameraIndex].camera_left.nearClipPlane  =
            additionalS3DCamerasStruct[additionalS3DTopmostCameraIndex].camera_right.nearClipPlane =
            additionalS3DCamerasStruct[additionalS3DTopmostCameraIndex].camera.nearClipPlane       = nearClip;
            cam.nearClipPlane = cameraBackupNearClip;
        }
        else
            camera_left.nearClipPlane = camera_right.nearClipPlane = cam.nearClipPlane = nearClip;

        camera_left.farClipPlane = camera_right.farClipPlane = cam.farClipPlane = farClip;

        if (S3DEnabled && nearClipHack) cam.nearClipPlane = -1;

        S3DProjection_Set();
    }

    void Resize()
    {
        clientSize          = new Vector2Int(Screen.width, Screen.height);
        clientSizePixelRect = new Rect(0, 0, clientSize.x, clientSize.y);
        Aspect_Set();
    }

    void Aspect_Set()
    {
        Render_Release();

        borders = (cam.rect != Rect.MinMaxRect(0, 0, 1, 1));

        Vector2 lb = new Vector2(Mathf.Clamp01(cam.rect.x), Mathf.Clamp01(cam.rect.y));
        Vector2 rt = new Vector2(Mathf.Clamp01(cam.rect.xMax), Mathf.Clamp01(cam.rect.yMax));
        camRectClamped = new Rect(lb.x, lb.y, rt.x - lb.x, rt.y - lb.y);

        camRectClampedPixels = new Rect(
            Mathf.Round(camRectClamped.x * clientSize.x),
            Mathf.Round(camRectClamped.y * clientSize.y),
            Mathf.Round(camRectClamped.width  * clientSize.x),
            Mathf.Round(camRectClamped.height * clientSize.y));

        viewportSize = new Vector2Int(
            (int)camRectClampedPixels.width,
            (int)camRectClampedPixels.height);

        // Rebuild full-screen quad for sequential
        screenBlitMesh.Clear();
        screenBlitMesh.vertices = quadVertices;
        screenBlitMesh.uv       = quadVerticesUV;
        screenBlitMesh.SetIndices(quadIndicesClockwise, MeshTopology.Quads, 0);

        aspect = viewportSize.x / (float)viewportSize.y;
        camera_left.aspect = camera_right.aspect = cam.aspect = aspect;
        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera) c.camera.aspect = c.camera_left.aspect = c.camera_right.aspect = aspect;

        FOV_Set();
        Render_Set();
    }

    void FOV_Set()
    {
        if (FOVControl_Active() && !camFOVChangedExternal)
        {
            FOV    = Mathf.Clamp(FOV, FOVMinMax.x, FOVMinMax.y);
            scaleX = 1 / Mathf.Tan(FOV * Mathf.PI / 360);
            scaleY = scaleX * aspect;
            vFOV   = 360 * Mathf.Atan(1 / scaleY) / Mathf.PI;
        }
        else
        {
            camFOVChangedExternal = false;
            vFOV   = cam.fieldOfView;
            FOV    = Mathf.Atan(aspect * Mathf.Tan(vFOV * Mathf.PI / 360)) * 360 / Mathf.PI;
            scaleX = 1 / Mathf.Tan(FOV * Mathf.PI / 360);
            scaleY = scaleX * aspect;
        }

        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera) c.camera_left.fieldOfView = c.camera_right.fieldOfView = c.camera.fieldOfView = vFOV;

        lastFOV = FOV;
        S3DProjection_Set();
    }

    bool FOVControl_Active() => true; // FOV control always on in this stripped build

    void UserIPD_Clamp()   { userIPD   = Mathf.Clamp(userIPD,   0.01f, 100); if (matchUserIPD) VirtualIPD_Clamp(); }
    void VirtualIPD_Clamp(){ virtualIPD = matchUserIPD ? userIPD : Mathf.Clamp(virtualIPD, 0.01f, virtualIPDMax); }

    void VSync_Set()
    {
        QualitySettings.vSyncCount  = vSync ? 1 : 0;
        Application.targetFrameRate = vSync ?
#if UNITY_2022_2_OR_NEWER
            (int)Screen.currentResolution.refreshRateRatio.value
#else
            Screen.currentResolution.refreshRate
#endif
            : -1;
    }

    void S3DCamerasPosition_Set()
    {
        if (eyePriority == EyePriority.Left)
            { leftCamLocalPos = Vector3.zero; rightCamLocalPos = Vector3.right * virtualIPD * .001f; }
        else if (eyePriority == EyePriority.Right)
            { leftCamLocalPos = Vector3.left * virtualIPD * .001f; rightCamLocalPos = Vector3.zero; }
        else
            { leftCamLocalPos = Vector3.left * virtualIPD * .0005f; rightCamLocalPos = Vector3.right * virtualIPD * .0005f; }

        if (swapLR) { var t = leftCamLocalPos; leftCamLocalPos = rightCamLocalPos; rightCamLocalPos = t; }

        camera_left.transform.localPosition  = leftCamLocalPos;
        camera_right.transform.localPosition = rightCamLocalPos;
        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera)
            {
                c.camera_left.transform.localPosition  = leftCamLocalPos;
                c.camera_right.transform.localPosition = rightCamLocalPos;
            }
    }

    void S3DProjection_Set()
    {
        imageWidth     = viewportSize.x * 25.4f / PPI;
        scaleX         = 1 / Mathf.Tan(FOV * Mathf.PI / 360);
        scaleY         = scaleX * aspect;
        screenDistance = scaleX * imageWidth * .5f;

        float shift = userIPD / imageWidth;

        S3DMaterial.SetFloat("_ShiftY", 0);   // No interlace shift for Sequential

        if (!cam.usePhysicalProperties)
        {
            if (nearClipHack || true) { /* matrix clip handled in Clip_Set */ }
        }
        else
        {
            camera_left.sensorSize  = camera_right.sensorSize  = cam.sensorSize  = new Vector2(imageWidth, imageWidth / aspect);
            camera_left.gateFit     = camera_right.gateFit     = cam.gateFit     = Camera.GateFitMode.None;
            camera_left.focalLength = camera_right.focalLength = cam.focalLength = screenDistance;

            Vector2 ls = new Vector2(-shift * .5f, 0);
            if (swapLR) ls.x *= -1;
            camera_left.lensShift  = -ls;
            camera_right.lensShift = new Vector2(ls.x, 0);

            foreach (var c in additionalS3DCamerasStruct)
                if (c.camera)
                {
                    c.camera.sensorSize    = cam.sensorSize;
                    c.camera.gateFit       = cam.gateFit;
                    c.camera.focalLength   = cam.focalLength;
                    c.camera.lensShift     = Vector2.zero;
                    c.camera_left.sensorSize  = c.camera_right.sensorSize  = cam.sensorSize;
                    c.camera_left.gateFit     = c.camera_right.gateFit     = cam.gateFit;
                    c.camera_left.focalLength = c.camera_right.focalLength = cam.focalLength;
                    c.camera_left.lensShift   = camera_left.lensShift;
                    c.camera_right.lensShift  = camera_right.lensShift;
                }

            vFOV = cam.fieldOfView;
        }

        if (detectCameraSettingChange)
        {
            lastCameraDataStruct.lensShift                     = cam.lensShift;
            lastCameraDataStruct.focalLength                   = cam.focalLength;
            lastCameraDataStruct.nonJitteredProjectionMatrix   = cam.nonJitteredProjectionMatrix;
            lastCameraDataStruct.projectionMatrix              = cam.projectionMatrix;
            lastCameraDataStruct.nearClipPlane                 = cam.nearClipPlane;
            lastCameraDataStruct.farClipPlane                  = cam.farClipPlane;
        }
    }

    // =========================================================================
    // Tracking
    // =========================================================================
    void BackupCamPosition()
    {
        camBackupLocalPosition = transform.localPosition;
        camBackupLocalRotation = transform.localRotation;
        screenDistanceBackup   = screenDistance;
        camBackupFOV           = cam.fieldOfView;
    }

    void ResetCamPosition()
    {
        transform.localPosition = camBackupLocalPosition;
        transform.localRotation = camBackupLocalRotation;
        cam.fieldOfView = camera_left.fieldOfView = camera_right.fieldOfView = camBackupFOV;
        vFOV = cam.fieldOfView;
        FOV  = Mathf.Atan(aspect * Mathf.Tan(vFOV * Mathf.PI / 360)) * 360 / Mathf.PI;
        lastFOV = FOV;
        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera)
                c.camera.fieldOfView = c.camera_left.fieldOfView = c.camera_right.fieldOfView = camBackupFOV;
    }

    void TrackingViewportAxis_Set()
    {
        Vector3 Axis(AxisSelect sel, Transform t) => sel switch
        {
            AxisSelect.X          =>  t.right,
            AxisSelect.inverted_X => -t.right,
            AxisSelect.Y          =>  t.up,
            AxisSelect.inverted_Y => -t.up,
            AxisSelect.Z          =>  t.forward,
            _                     => -t.forward
        };
        trackingViewport.upAxis      = Axis(trackingViewport.upDirection,     trackingViewport.center);
        trackingViewport.forwardAxis = Axis(trackingViewport.forwardDirection, trackingViewport.center);
    }

    public void TrackingRotationHelper_SetRotation(Quaternion rotation)
        => trackingRotationHelper.transform.localRotation = rotation;

    void Tracking_Set()
    {
        if (lastTracking != tracking)
        {
            lastTracking = tracking;
            if (!tracking) { ResetCamPosition(); S3DCamerasPosition_Set(); S3DProjection_Set(); }
        }

        if (!lastTrackingViewport.Equals(trackingViewport)) onOffToggle = true;

        if (!tracking) { BackupCamPosition(); return; }

        float imageHeight = imageWidth / aspect;

        if (trackingViewport.center)
        {
            if (trackingViewport.rotation != trackingViewport.center.rotation)
            {
                trackingViewport.rotation = trackingViewport.center.rotation;
                TrackingViewportAxis_Set();
                lastTrackingViewport = trackingViewport;
            }
            trackingViewportCenterPosition = trackingViewport.center.position;
            Vector3 perp = trackingViewport.center.position +
                Vector3.ProjectOnPlane(transform.position - trackingViewportCenterPosition, trackingViewport.forwardAxis);
            transform.LookAt(perp, trackingViewport.upAxis);
        }
        else
            trackingViewportCenterPosition = transform.parent.TransformPoint(camBackupLocalPosition)
                + transform.forward * screenDistanceBackup * .001f;

        Vector3 stc = transform.position - trackingViewportCenterPosition;
        trackingPosition = new Vector3(
            Vector3.Dot(stc, transform.right),
            Vector3.Dot(stc, transform.up),
            Vector3.Dot(stc, transform.forward));

        Vector3 axisX = transform.InverseTransformDirection(trackingRotationHelper.transform.right);
        camera_left.transform.localPosition  = axisX * leftCamLocalPos.x;
        camera_right.transform.localPosition = axisX * rightCamLocalPos.x;

        screenDistance = -Vector3.Dot(stc, transform.forward) * 1000;

        cam.fieldOfView          = Mathf.Atan(imageHeight / screenDistance * .5f) * halfRadiansToDegrees;
        camera_left.fieldOfView  = Mathf.Atan(imageHeight / (screenDistance - camera_left.transform.localPosition.z * 1000) * .5f) * halfRadiansToDegrees;
        camera_right.fieldOfView = Mathf.Atan(imageHeight / (screenDistance - camera_right.transform.localPosition.z * 1000) * .5f) * halfRadiansToDegrees;

        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera)
            {
                c.camera_left.transform.localPosition  = camera_left.transform.localPosition;
                c.camera_right.transform.localPosition = camera_right.transform.localPosition;
                c.camera.fieldOfView       = cam.fieldOfView;
                c.camera_left.fieldOfView  = camera_left.fieldOfView;
                c.camera_right.fieldOfView = camera_right.fieldOfView;
            }

        vFOV    = cam.fieldOfView;
        FOV     = Mathf.Atan(aspect * Mathf.Tan(vFOV * Mathf.PI / 360)) * 360 / Mathf.PI;
        lastFOV = FOV;
    }

    // =========================================================================
    // URP Camera Stack
    // =========================================================================
#if URP
    void URPAssetSettings_Get()
    {
        lastScriptableRenderer = URPAsset.scriptableRenderer;
        URPAssetIsReady = true;
    }

    void CameraStackSet()
    {
        if (S3DEnabled)
        {
            foreach (var c in cameraStack)
                if (additionalS3DCameras.Contains(c))
                {
                    int idx = additionalS3DCameras.IndexOf(c);
                    leftCameraStack.Add(additionalS3DCamerasStruct[idx].camera_left);
                    rightCameraStack.Add(additionalS3DCamerasStruct[idx].camera_right);
                }
                else { leftCameraStack.Add(c); rightCameraStack.Add(c); }
            cameraStack.RemoveAll(t => t);
        }
        else CameraStackRestore();

        lastCameraStack = cameraStack.ToArray();
    }

    void CameraStackRestore()
    {
        foreach (var c in leftCameraStack)
            cameraStack.Add(c.name.Contains("_left") ? c.transform.parent.GetComponent<Camera>() : c);
        leftCameraStack.RemoveAll(t => t);
        rightCameraStack.RemoveAll(t => t);
    }
#endif

    // =========================================================================
    // OnDisable / OnApplicationQuit
    // =========================================================================
    void OnDisable()
    {
        if (debugLog) Debug.Log("OnDisable");

        Render_Release();
        cameraRestore();

#if URP
        CameraStackRestore();
#endif

        if (lastAdditionalS3DTopmostCamera)
            lastAdditionalS3DTopmostCamera.nearClipPlane = sceneNearClip;
        cam.nearClipPlane = cameraBackupNearClip;
        cam.farClipPlane  = sceneFarClip;

        HDRPSettings_Restore();

        if(trackingRotationHelper)
        {
            Destroy(trackingRotationHelper.gameObject);
        }

        if (cam.usePhysicalProperties) { cam.projectionMatrix = camBackupMatrix; cam.usePhysicalProperties = true; }
        cam.lensShift = camBackupLensShift;

        Destroy(camera_left.gameObject);
        Destroy(camera_right.gameObject);

        foreach (var c in additionalS3DCamerasStruct)
            if (c.camera) { c.camera.lensShift = camBackupLensShift; Destroy(c.camera_left.gameObject); Destroy(c.camera_right.gameObject); }

        Destroy(S3DMaterial);
        ResetCamPosition();
        CancelInvoke();
        Resources.UnloadUnusedAssets();
    }

    void HDRPSettings_Restore()
    {
#if HDRP
        typeof(HDRenderPipelineAsset)
            .GetField("m_RenderPipelineSettings", BindingFlags.Instance | BindingFlags.NonPublic)
            .SetValue(GraphicsSettings.currentRenderPipeline, backupHDRPSettings);
#endif
    }

    void OnApplicationQuit()
    {
        Destroy(clientSizeMesh);
        Destroy(clientSizeMeshFlippedY);
        Destroy(screenBlitMesh);
        cb_main.Release();
        cb_left.Release();
        cb_right.Release();
    }

    void Enable() { enabled = true; }

    // =========================================================================
    // CameraDataStruct
    // =========================================================================
    void CameraDataStruct_Set()
    {
        cameraDataStruct = CameraDataStruct_Make(cam);
        for (int i = 0; i < additionalS3DCamerasStruct.Length; i++)
            if (additionalS3DCamerasStruct[i].camera)
                additionalS3DCamerasStruct[i].cameraDataStruct =
                    CameraDataStruct_Make(additionalS3DCamerasStruct[i].camera);
    }

    void LastCameraDataStruct_Set()
    {
        lastCameraDataStruct = cameraDataStruct;
        for (int i = 0; i < additionalS3DCamerasStruct.Length; i++)
            additionalS3DCamerasStruct[i].lastCameraDataStruct =
                additionalS3DCamerasStruct[i].cameraDataStruct;
    }

    void CameraDataStruct_Change() { CameraDataStruct_Set(); LastCameraDataStruct_Set(); }

    CameraDataStruct CameraDataStruct_Make(Camera c)
    {
#if URP
        var cd = c.GetUniversalAdditionalCameraData();
#elif HDRP
        var cd = c.GetComponent<HDAdditionalCameraData>();
#endif
        return new CameraDataStruct(
#if URP
            cd.requiresDepthTexture, cd.stopNaN, cd.antialiasingQuality, cd.antialiasing,
            cd.renderPostProcessing,
#if UNITY_2021_1_OR_NEWER
            cd.volumeStack,
#endif
            cd.volumeTrigger, cd.volumeLayerMask, cd.requiresColorTexture,
#if UNITY_2020_2_OR_NEWER
            cd.allowXRRendering,
#endif
            cd.renderType, cd.requiresColorOption, cd.requiresDepthOption,
            cd.renderShadows, cd.dithering,
#elif HDRP
            cd.clearColorMode, cd.backgroundColorHDR, cd.clearDepth, cd.customRenderingSettings,
            cd.volumeLayerMask, cd.volumeAnchorOverride, cd.antialiasing, cd.dithering,
#if UNITY_2020_2_OR_NEWER
            cd.xrRendering,
#endif
            cd.SMAAQuality, cd.stopNaNs, cd.taaSharpenStrength,
#if UNITY_2020_2_OR_NEWER
            cd.TAAQuality, cd.taaHistorySharpening, cd.taaAntiFlicker,
            cd.taaMotionVectorRejection, cd.taaAntiHistoryRinging,
#endif
#if UNITY_2021_2_OR_NEWER
            cd.taaBaseBlendFactor,
#endif
#if UNITY_2021_3_OR_NEWER
            cd.taaJitterScale,
#endif
            cd.flipYMode, cd.fullscreenPassthrough, cd.invertFaceCulling,
            cd.probeLayerMask, cd.hasPersistentHistory,
#if UNITY_2020_2_OR_NEWER
            cd.exposureTarget,
#endif
#if !UNITY_2022_1_OR_NEWER
            cd.physicalParameters,
#endif
            cd.renderingPathCustomFrameSettings,
            cd.renderingPathCustomFrameSettingsOverrideMask,
            cd.defaultFrameSettings,
#if UNITY_2021_2_OR_NEWER
            cd.allowDeepLearningSuperSampling,
            cd.deepLearningSuperSamplingUseCustomQualitySettings,
            cd.deepLearningSuperSamplingQuality,
            cd.deepLearningSuperSamplingUseCustomAttributes,
            cd.deepLearningSuperSamplingUseOptimalSettings,
            cd.deepLearningSuperSamplingSharpening,
            cd.materialMipBias,
#endif
#endif
            c.clearStencilAfterLightingPass, c.useOcclusionCulling, c.usePhysicalProperties,
            c.sensorSize, c.overrideSceneCullingMask, c.cameraType, c.layerCullSpherical,
            c.eventMask, c.clearFlags, c.backgroundColor, c.aspect, c.lensShift, c.depth,
            c.transparencySortAxis, c.transparencySortMode, c.opaqueSortMode,
            c.orthographic, c.orthographicSize, c.forceIntoRenderTexture,
            c.allowDynamicResolution, c.allowMSAA, c.allowHDR,
            c.actualRenderingPath, c.renderingPath, c.cullingMask, c.focalLength, c.rect,
#if UNITY_2021_2_OR_NEWER
            c.sceneViewFilterMode,
#endif
            c.scene, c.useJitteredProjectionMatrixForTransparentRendering,
            c.nonJitteredProjectionMatrix, c.projectionMatrix,
            c.targetDisplay, c.farClipPlane, c.gateFit, c.nearClipPlane);
    }

    // =========================================================================
    // Structs
    // =========================================================================
    [Serializable]
    public struct AdditionalS3DCamera
    {
        public Camera camera;
        public Camera camera_left;
        public Camera camera_right;
        public Matrix4x4 cameraBackupMatrix;
        public Matrix4x4 cameraDefaultMatrix;
#if POST_PROCESSING_STACK_V2
        public PostProcessLayer PPLayer;
        public bool PPLayerLastStatus;
#endif
        public CameraDataStruct cameraDataStruct;
        public CameraDataStruct lastCameraDataStruct;
    }

    public struct CameraDataStruct
    {
#if URP
        public bool requiresDepthTexture, stopNaN;
        public AntialiasingQuality antialiasingQuality;
        public AntialiasingMode antialiasing;
        public bool renderPostProcessing;
#if UNITY_2021_1_OR_NEWER
        public VolumeStack volumeStack;
#endif
        public Transform volumeTrigger;
        public LayerMask volumeLayerMask;
        public bool requiresColorTexture;
#if UNITY_2020_2_OR_NEWER
        public bool allowXRRendering;
#endif
        public CameraRenderType renderType;
        public CameraOverrideOption requiresColorOption, requiresDepthOption;
        public bool renderShadows, dithering;
#elif HDRP
        public HDAdditionalCameraData.ClearColorMode clearColorMode;
        public Color backgroundColorHDR;
        public bool clearDepth, customRenderingSettings;
        public LayerMask volumeLayerMask;
        public Transform volumeAnchorOverride;
        public HDAdditionalCameraData.AntialiasingMode antialiasing;
        public bool dithering;
#if UNITY_2020_2_OR_NEWER
        public bool xrRendering;
#endif
        public HDAdditionalCameraData.SMAAQualityLevel SMAAQuality;
        public bool stopNaNs;
        public float taaSharpenStrength;
#if UNITY_2020_2_OR_NEWER
        public HDAdditionalCameraData.TAAQualityLevel TAAQuality;
        public float taaHistorySharpening, taaAntiFlicker, taaMotionVectorRejection;
        public bool taaAntiHistoryRinging;
#endif
#if UNITY_2021_2_OR_NEWER
        public float taaBaseBlendFactor;
#endif
#if UNITY_2021_3_OR_NEWER
        public float taaJitterScale;
#endif
        public HDAdditionalCameraData.FlipYMode flipYMode;
        public bool fullscreenPassthrough, invertFaceCulling;
        public LayerMask probeLayerMask;
        public bool hasPersistentHistory;
#if UNITY_2020_2_OR_NEWER
        public GameObject exposureTarget;
#endif
#if !UNITY_2022_1_OR_NEWER
        public HDPhysicalCamera physicalParameters;
#endif
        public FrameSettings renderingPathCustomFrameSettings;
        public FrameSettingsOverrideMask renderingPathCustomFrameSettingsOverrideMask;
        public FrameSettingsRenderType defaultFrameSettings;
#if UNITY_2021_2_OR_NEWER
        bool allowDeepLearningSuperSampling;
        public bool deepLearningSuperSamplingUseCustomQualitySettings, deepLearningSuperSamplingUseCustomAttributes, deepLearningSuperSamplingUseOptimalSettings;
        public uint deepLearningSuperSamplingQuality;
        public float deepLearningSuperSamplingSharpening, materialMipBias;
#endif
#endif
        public bool clearStencilAfterLightingPass, useOcclusionCulling, usePhysicalProperties;
        public Vector2 sensorSize;
        public ulong overrideSceneCullingMask;
        public CameraType cameraType;
        public bool layerCullSpherical;
        public int eventMask;
        public CameraClearFlags clearFlags;
        public Color backgroundColor;
        public float aspect;
        public Vector2 lensShift;
        public float depth;
        public Vector3 transparencySortAxis;
        public TransparencySortMode transparencySortMode;
        public OpaqueSortMode opaqueSortMode;
        public bool orthographic;
        public float orthographicSize;
        public bool forceIntoRenderTexture, allowDynamicResolution, allowMSAA, allowHDR;
        public RenderingPath actualRenderingPath, renderingPath;
        public int cullingMask;
        public float focalLength;
        public Rect rect;
#if UNITY_2021_2_OR_NEWER
        public Camera.SceneViewFilterMode sceneViewFilterMode;
#endif
        public UnityEngine.SceneManagement.Scene scene;
        public bool useJitteredProjectionMatrixForTransparentRendering;
        public Matrix4x4 nonJitteredProjectionMatrix, projectionMatrix;
        public int targetDisplay;
        public float farClipPlane;
        public Camera.GateFitMode gateFit;
        public float nearClipPlane;

        public CameraDataStruct(
#if URP
            bool requiresDepthTexture, bool stopNaN, AntialiasingQuality antialiasingQuality,
            AntialiasingMode antialiasing, bool renderPostProcessing,
#if UNITY_2021_1_OR_NEWER
            VolumeStack volumeStack,
#endif
            Transform volumeTrigger, LayerMask volumeLayerMask, bool requiresColorTexture,
#if UNITY_2020_2_OR_NEWER
            bool allowXRRendering,
#endif
            CameraRenderType renderType, CameraOverrideOption requiresColorOption,
            CameraOverrideOption requiresDepthOption, bool renderShadows, bool dithering,
#elif HDRP
            HDAdditionalCameraData.ClearColorMode clearColorMode, Color backgroundColorHDR,
            bool clearDepth, bool customRenderingSettings, LayerMask volumeLayerMask,
            Transform volumeAnchorOverride, HDAdditionalCameraData.AntialiasingMode antialiasing,
            bool dithering,
#if UNITY_2020_2_OR_NEWER
            bool xrRendering,
#endif
            HDAdditionalCameraData.SMAAQualityLevel SMAAQuality, bool stopNaNs, float taaSharpenStrength,
#if UNITY_2020_2_OR_NEWER
            HDAdditionalCameraData.TAAQualityLevel TAAQuality, float taaHistorySharpening,
            float taaAntiFlicker, float taaMotionVectorRejection, bool taaAntiHistoryRinging,
#endif
#if UNITY_2021_2_OR_NEWER
            float taaBaseBlendFactor,
#endif
#if UNITY_2021_3_OR_NEWER
            float taaJitterScale,
#endif
            HDAdditionalCameraData.FlipYMode flipYMode, bool fullscreenPassthrough,
            bool invertFaceCulling, LayerMask probeLayerMask, bool hasPersistentHistory,
#if UNITY_2020_2_OR_NEWER
            GameObject exposureTarget,
#endif
#if !UNITY_2022_1_OR_NEWER
            HDPhysicalCamera physicalParameters,
#endif
            FrameSettings renderingPathCustomFrameSettings,
            FrameSettingsOverrideMask renderingPathCustomFrameSettingsOverrideMask,
            FrameSettingsRenderType defaultFrameSettings,
#if UNITY_2021_2_OR_NEWER
            bool allowDeepLearningSuperSampling,
            bool deepLearningSuperSamplingUseCustomQualitySettings,
            uint deepLearningSuperSamplingQuality,
            bool deepLearningSuperSamplingUseCustomAttributes,
            bool deepLearningSuperSamplingUseOptimalSettings,
            float deepLearningSuperSamplingSharpening, float materialMipBias,
#endif
#endif
            bool clearStencilAfterLightingPass, bool useOcclusionCulling, bool usePhysicalProperties,
            Vector2 sensorSize, ulong overrideSceneCullingMask, CameraType cameraType,
            bool layerCullSpherical, int eventMask, CameraClearFlags clearFlags,
            Color backgroundColor, float aspect, Vector2 lensShift, float depth,
            Vector3 transparencySortAxis, TransparencySortMode transparencySortMode,
            OpaqueSortMode opaqueSortMode, bool orthographic, float orthographicSize,
            bool forceIntoRenderTexture, bool allowDynamicResolution, bool allowMSAA, bool allowHDR,
            RenderingPath actualRenderingPath, RenderingPath renderingPath,
            int cullingMask, float focalLength, Rect rect,
#if UNITY_2021_2_OR_NEWER
            Camera.SceneViewFilterMode sceneViewFilterMode,
#endif
            UnityEngine.SceneManagement.Scene scene,
            bool useJitteredProjectionMatrixForTransparentRendering,
            Matrix4x4 nonJitteredProjectionMatrix, Matrix4x4 projectionMatrix,
            int targetDisplay, float farClipPlane,
            Camera.GateFitMode gateFit, float nearClipPlane)
        {
#if URP
            this.requiresDepthTexture = requiresDepthTexture; this.stopNaN = stopNaN;
            this.antialiasingQuality  = antialiasingQuality;  this.antialiasing = antialiasing;
            this.renderPostProcessing = renderPostProcessing;
#if UNITY_2021_1_OR_NEWER
            this.volumeStack = volumeStack;
#endif
            this.volumeTrigger = volumeTrigger; this.volumeLayerMask = volumeLayerMask;
            this.requiresColorTexture = requiresColorTexture;
#if UNITY_2020_2_OR_NEWER
            this.allowXRRendering = allowXRRendering;
#endif
            this.renderType = renderType; this.requiresColorOption = requiresColorOption;
            this.requiresDepthOption = requiresDepthOption; this.renderShadows = renderShadows;
            this.dithering = dithering;
#elif HDRP
            this.clearColorMode = clearColorMode; this.backgroundColorHDR = backgroundColorHDR;
            this.clearDepth = clearDepth; this.customRenderingSettings = customRenderingSettings;
            this.volumeLayerMask = volumeLayerMask; this.volumeAnchorOverride = volumeAnchorOverride;
            this.antialiasing = antialiasing; this.dithering = dithering;
#if UNITY_2020_2_OR_NEWER
            this.xrRendering = xrRendering;
#endif
            this.SMAAQuality = SMAAQuality; this.stopNaNs = stopNaNs;
            this.taaSharpenStrength = taaSharpenStrength;
#if UNITY_2020_2_OR_NEWER
            this.TAAQuality = TAAQuality; this.taaHistorySharpening = taaHistorySharpening;
            this.taaAntiFlicker = taaAntiFlicker; this.taaMotionVectorRejection = taaMotionVectorRejection;
            this.taaAntiHistoryRinging = taaAntiHistoryRinging;
#endif
#if UNITY_2021_2_OR_NEWER
            this.taaBaseBlendFactor = taaBaseBlendFactor;
#endif
#if UNITY_2021_3_OR_NEWER
            this.taaJitterScale = taaJitterScale;
#endif
            this.flipYMode = flipYMode; this.fullscreenPassthrough = fullscreenPassthrough;
            this.invertFaceCulling = invertFaceCulling; this.probeLayerMask = probeLayerMask;
            this.hasPersistentHistory = hasPersistentHistory;
#if UNITY_2020_2_OR_NEWER
            this.exposureTarget = exposureTarget;
#endif
#if !UNITY_2022_1_OR_NEWER
            this.physicalParameters = physicalParameters;
#endif
            this.renderingPathCustomFrameSettings = renderingPathCustomFrameSettings;
            this.renderingPathCustomFrameSettingsOverrideMask = renderingPathCustomFrameSettingsOverrideMask;
            this.defaultFrameSettings = defaultFrameSettings;
#if UNITY_2021_2_OR_NEWER
            this.allowDeepLearningSuperSampling = allowDeepLearningSuperSampling;
            this.deepLearningSuperSamplingUseCustomQualitySettings = deepLearningSuperSamplingUseCustomQualitySettings;
            this.deepLearningSuperSamplingQuality = deepLearningSuperSamplingQuality;
            this.deepLearningSuperSamplingUseCustomAttributes = deepLearningSuperSamplingUseCustomAttributes;
            this.deepLearningSuperSamplingUseOptimalSettings = deepLearningSuperSamplingUseOptimalSettings;
            this.deepLearningSuperSamplingSharpening = deepLearningSuperSamplingSharpening;
            this.materialMipBias = materialMipBias;
#endif
#endif
            this.clearStencilAfterLightingPass = clearStencilAfterLightingPass;
            this.useOcclusionCulling = useOcclusionCulling; this.usePhysicalProperties = usePhysicalProperties;
            this.sensorSize = sensorSize; this.overrideSceneCullingMask = overrideSceneCullingMask;
            this.cameraType = cameraType; this.layerCullSpherical = layerCullSpherical;
            this.eventMask = eventMask; this.clearFlags = clearFlags;
            this.backgroundColor = backgroundColor; this.aspect = aspect;
            this.lensShift = lensShift; this.depth = depth;
            this.transparencySortAxis = transparencySortAxis;
            this.transparencySortMode = transparencySortMode; this.opaqueSortMode = opaqueSortMode;
            this.orthographic = orthographic; this.orthographicSize = orthographicSize;
            this.forceIntoRenderTexture = forceIntoRenderTexture;
            this.allowDynamicResolution = allowDynamicResolution;
            this.allowMSAA = allowMSAA; this.allowHDR = allowHDR;
            this.actualRenderingPath = actualRenderingPath; this.renderingPath = renderingPath;
            this.cullingMask = cullingMask; this.focalLength = focalLength; this.rect = rect;
#if UNITY_2021_2_OR_NEWER
            this.sceneViewFilterMode = sceneViewFilterMode;
#endif
            this.scene = scene;
            this.useJitteredProjectionMatrixForTransparentRendering = useJitteredProjectionMatrixForTransparentRendering;
            this.nonJitteredProjectionMatrix = nonJitteredProjectionMatrix;
            this.projectionMatrix = projectionMatrix;
            this.targetDisplay = targetDisplay; this.farClipPlane = farClipPlane;
            this.gateFit = gateFit; this.nearClipPlane = nearClipPlane;
        }
    }
}