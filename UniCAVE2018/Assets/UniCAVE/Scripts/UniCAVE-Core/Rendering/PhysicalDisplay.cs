//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
//Benny Wysong-Grass
//University of Wisconsin - Madison Virtual Environments Group
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

struct Settings {
    public PhysicalDisplayManager manager;
    public string machineName;
    public float width;
    public float height;
    public float halfWidth() { return width * 0.5f; }
    public float halfHeight() { return height * 0.5f; }
    public HeadConfiguration head;

    public bool useXRCameras;
    public bool useRenderTextures;
    public Vector2Int renderTextureSize;

    public bool is3D;
    public bool exclusiveFullscreen;
    public int display;
    public bool dualPipe;
    public bool dualInstance;
    public RectInt windowBounds;
    public RectInt leftViewport;
    public RectInt rightViewport;
}

[Serializable]
public class PhysicalDisplay : MonoBehaviour {
    #region Windows Utils
    #if UNITY_STANDALONE_WIN
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

    [DllImport("user32.dll")]
    public static extern bool SetWindowText(System.IntPtr hwnd, System.String lpString);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner
    }

    private int _setWidth = -1, _setHeight = -1;
    private int _desiredWidth = -1, _desiredHeight = -1;
    private int _desiredX = -1, _desiredY = -1;
    private string _desiredName;
    //setWidth and setHeight are used to uniquely identify the window
    //so if there are multiple instances of this application running at once,
    //make setWidth and setHeight unique values so the wrong window is not selected
    //this solution is terrible but this is the only way to uniquely find which window
    //this instance of unity is related to
    private void SetMyWindowInfo(string text, int posX, int posY, int width, int height, int setWidth, int setHeight) {
        _setWidth = setWidth;
        _setHeight = setHeight;
        _desiredWidth = width;
        _desiredHeight = height;
        _desiredName = text;
        _desiredX = posX;
        _desiredY = posY;
        _resolutions = loadAllWindowSizes();
        Screen.SetResolution(setWidth, setHeight, false);
    }

    private Dictionary<long, Vector2Int> _resolutions = null;
    Dictionary<long, Vector2Int> loadAllWindowSizes() {
        Dictionary<long, Vector2Int> res = new Dictionary<long, Vector2Int>();

        EnumWindows(delegate (IntPtr wnd, IntPtr param) {
            RECT r = new RECT();
            GetWindowRect(wnd, out r);

            int wndWidth = r.Right - r.Left;
            int wndHeight = r.Bottom - r.Top;

            res.Add(wnd.ToInt64(), new Vector2Int(wndWidth, wndHeight));

            return true;
        }, IntPtr.Zero);

        return res;
    }
#endif
    #endregion

    public PhysicalDisplayManager manager;
    public string machineName;
    public float width;
    public float height;
    public float halfWidth() { return width * 0.5f; }
    public float halfHeight() { return height * 0.5f; }
    public HeadConfiguration head;

    public bool useXRCameras;
    public bool useRenderTextures;
    public Vector2Int renderTextureSize;

    public bool is3D;
    public bool exclusiveFullscreen;
    public int display;
    public bool dualPipe;
    public bool dualInstance;
    public RectInt windowBounds;
    public RectInt leftViewport;
    public RectInt rightViewport;

    public bool loadSettingsAtRuntime;

    [NonSerialized]
    public Camera leftCam;
    [NonSerialized]
    public Camera centerCam;
    [NonSerialized]
    public Camera rightCam;

    [NonSerialized]
    public RenderTexture leftTex;
    [NonSerialized]
    public RenderTexture centerTex;
    [NonSerialized]
    public RenderTexture rightTex;


    [NonSerialized]
    bool initialized = false;
    public bool Initialized() {
        return initialized;
    }

    public bool ShouldBeActive() {
        //machine name doesn't matter if it has a manager
        //in that case we defer to manager's machine name
        return (manager == null ? (Util.GetMachineName() == machineName) : manager.ShouldBeActive());
    }

    public List<Camera> GetAllCameras() {
        List<Camera> res = new List<Camera>();
        if (centerCam != null) res.Add(centerCam);
        if (leftCam != null) res.Add(leftCam);
        if (rightCam != null) res.Add(rightCam);
        return res;
    }

    public Vector3 ScreenspaceToWorld(Vector2 ss) {
        return transform.localToWorldMatrix * new Vector4(ss.x * halfWidth(), ss.y * halfHeight(), 0.0f, 1.0f);
    }
    public Vector3 UpperRight {
        get {
            return transform.localToWorldMatrix * new Vector4(halfWidth(), halfHeight(), 0.0f, 1.0f);
        }
    }
    public Vector3 UpperLeft {
        get {
            return transform.localToWorldMatrix * new Vector4(-halfWidth(), halfHeight(), 0.0f, 1.0f);
        }
    }
    public Vector3 LowerLeft {
        get {
            return transform.localToWorldMatrix * new Vector4(-halfWidth(), -halfHeight(), 0.0f, 1.0f);
        }
    }
    public Vector3 LowerRight {
        get {
            return transform.localToWorldMatrix * new Vector4(halfWidth(), -halfHeight(), 0.0f, 1.0f);
        }
    }

    public List<string> GetSettingsErrors() {
        List<string> errors = new List<string>();
        if (head == null) {
            errors.Add("Physical Display must have a reference to a GameObject with Head Configuration script!");
        }
        if ((machineName == null || machineName.Length == 0) && manager == null) {
            errors.Add("Physical Display should probably have a non-empty machine name");
        }
        if (width <= 0 || height <= 0) {
            errors.Add("Physical Display has impossible dimensions");
        }
        if (exclusiveFullscreen && useRenderTextures) {
            errors.Add("Physical Display uses renderTextures but also uses a certain display");
        }
        if(exclusiveFullscreen && dualPipe) {
            errors.Add("Physical display is in dual pipe mode but also uses a certain display");
        }
        if (dualInstance && manager != null) {
            errors.Add("Physical Display has dual instance enabled, but the display is also managed");
        }
        if (useRenderTextures && manager != null && GetComponent<PhysicalDisplayCalibration>() == null) {
            errors.Add("Physical Display uses render textures but is also managed");
        }
        if (useXRCameras && dualPipe) {
            errors.Add("Physical Display uses XR cameras but is also dual pipe");
        }
        if(useXRCameras && !is3D) {
            errors.Add("Physical Display uses XR cameras but is not 3D");
        }
        if(exclusiveFullscreen &&
            (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 ||
            SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 ||
            SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore)) {
            errors.Add("Physical Display uses a certain display but the Unity Player isn't using DirectX");
        }
        if (is3D && !dualPipe && !XRSettings.enabled) {
            errors.Add("Physical Display is passive stereo but VR is not enabled (Enable with Edit->Project Settings->Player->XR Settings)");
        }
        if (is3D && !dualPipe && (XRSettings.supportedDevices.Length < 1 || XRSettings.supportedDevices[0] != "stereo")) {
            errors.Add("Physical Display is passive stereo but the primary XR device is not passive stereo");
        }

        return errors;
    }

    //public List<string> CleanSettings() {
    //    List<string> changedSettings = new List<string>();
    //    //clear all viewport stuff if using a certain display
    //    //or if using render textures for post processing
    //    if (exclusiveFullscreen) {

    //    }
    //    return changedSettings;
    //}

    void SetSettings(Settings settings) {
        manager = settings.manager;
        head = settings.head;
        machineName = settings.machineName;
        width = settings.width;
        height = settings.height;

        useXRCameras = settings.useXRCameras;
        useRenderTextures = settings.useRenderTextures;
        renderTextureSize = settings.renderTextureSize;

        is3D = settings.is3D;
        exclusiveFullscreen = settings.exclusiveFullscreen;
        display = settings.display;
        dualPipe = settings.dualPipe;
        dualInstance = settings.dualInstance;
        windowBounds = settings.windowBounds;
        leftViewport = settings.leftViewport;
        rightViewport = settings.rightViewport;
    }
    Settings GetSettings() {
        Settings settings = new Settings();
        settings.manager = manager;
        settings.head = head;
        settings.machineName = machineName;
        settings.width = width;
        settings.height = height;

        settings.useXRCameras = useXRCameras;
        settings.useRenderTextures = useRenderTextures;
        settings.renderTextureSize = renderTextureSize;

        settings.is3D = is3D;
        settings.exclusiveFullscreen = exclusiveFullscreen;
        settings.display = display;
        settings.dualPipe = dualPipe;
        settings.dualInstance = dualInstance;
        settings.windowBounds = windowBounds;
        settings.leftViewport = leftViewport;
        settings.rightViewport = rightViewport;
        return settings;
    }

    void Start() {
        
        centerCam = null;
        leftCam = null;
        rightCam = null;

        if (loadSettingsAtRuntime) {
            Debug.Log("Attempting to Load Settings for Display: " + gameObject.name);
            TryToDeSerialize();
        }

        if (!ShouldBeActive()) {
            Debug.Log("Deactivating Display: " + gameObject.name);
            gameObject.SetActive(false);
            return;
        }

        Debug.Log("Display Active: " + gameObject.name);

        foreach (string er in GetSettingsErrors()) {
            Debug.Log("Display Warning for Display: " + gameObject.name + ": " + er);
        }

        if (head == null) {
            Debug.Log("Display Error for Display: " + gameObject.name + ": Physical Display has no head object");
        }

        initialized = true;

        if (exclusiveFullscreen) {
            if(display < Display.displays.Length) {
                Display.displays[display].Activate();
            } else {
                Debug.Log("Display Error for Display: " + gameObject.name + ": Physical Display uses display index " + display + " but Unity does not detect that many displays");
            }
        }
            

        if(!exclusiveFullscreen) {
            Debug.Log("Setting Display: " + gameObject.name + " to Windowed Mode...");
            if(!is3D) {
                centerCam = head.CreateCenterEye(gameObject.name);
                Debug.Log("Setting Display: " + gameObject.name + " to Non-3D Windowed");
                #if UNITY_STANDALONE_WIN
                if(manager == null) SetMyWindowInfo("Non-3D Windowed", windowBounds.x, windowBounds.y, windowBounds.width, windowBounds.height, 441, 411);
                #endif
            } else {
                if (!dualPipe && !dualInstance) {
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                    Debug.Log("Setting Display: " + gameObject.name + " to Passive-3D Windowed");
                    #if UNITY_STANDALONE_WIN
                    if(manager == null) SetMyWindowInfo("Passive-3D Windowed", windowBounds.x, windowBounds.y, windowBounds.width, windowBounds.height, 421, 420);
                    #endif
                } else if(dualPipe && !dualInstance) {
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                    Debug.Log("Setting Display: " + gameObject.name + " to Dual-Eye Dual-Pipe-3D Windowed");
                    #if UNITY_STANDALONE_WIN
                    if(manager == null) SetMyWindowInfo("Dual-Eye Dual-Pipe-3D Windowed", windowBounds.x, windowBounds.y, windowBounds.width, windowBounds.height, 422, 398);
                    #endif
                } else if(dualPipe && dualInstance) {
                    if(Util.GetArg("eye") == "left") {
                        leftCam = head.CreateLeftEye(gameObject.name);
                        Debug.Log("Setting Display: " + gameObject.name + " to Left-Eye Dual-Pipe-3D Windowed");
                        #if UNITY_STANDALONE_WIN
                        if(manager == null) SetMyWindowInfo("Left-Eye Dual-Pipe-3D Windowed", leftViewport.x, leftViewport.y, leftViewport.width, leftViewport.height, 300, 367);
                        #endif
                    } else if(Util.GetArg("eye") == "right") {
                        rightCam = head.CreateRightEye(gameObject.name);
                        Debug.Log("Setting Display: " + gameObject.name + " to Right-Eye Dual-Pipe-3D Windowed");
                        #if UNITY_STANDALONE_WIN
                        if(manager == null) SetMyWindowInfo("Right-Eye Dual-Pipe-3D Windowed", rightViewport.x, rightViewport.y, rightViewport.width, rightViewport.height, 342, 498);
                        #endif
                    }
                }
            }
        } else {
            if(!is3D) {
                centerCam = head.CreateCenterEye(gameObject.name);
            } else {
                if (!dualPipe && !dualInstance) {   
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                } else if(dualPipe && !dualInstance) {
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                } else if(dualPipe && dualInstance) {
                    if(Util.GetArg("eye") == "left") {
                        leftCam = head.CreateLeftEye(gameObject.name);
                    } else if(Util.GetArg("eye") == "right") {
                        rightCam = head.CreateRightEye(gameObject.name);
                    }
                }
            }
        }
        if(useRenderTextures) {
            if(leftCam != null) {
                leftTex = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0);
                leftCam.targetTexture = leftTex;
            }
            if (centerCam != null) {
                centerTex = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0);
                centerCam.targetTexture = centerTex;
            }
            if (rightCam != null) {
                rightTex = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0);
                rightCam.targetTexture = rightTex;
            }
        }
        if(exclusiveFullscreen) {
            if (leftCam != null) leftCam.targetDisplay = display;
            if (centerCam != null) centerCam.targetDisplay = display;
            if (rightCam != null) rightCam.targetDisplay = display;
        }
    }

    private void LateUpdate() {
        if (centerCam != null) {
            centerCam.projectionMatrix = Util.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, centerCam.transform.position, head.nearClippingPlane, head.farClippingPlane);
            centerCam.transform.rotation = transform.rotation;
        }

        if(leftCam != null) {
            Matrix4x4 leftMat = Util.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, leftCam.transform.position, head.nearClippingPlane, head.farClippingPlane);

            if (useXRCameras) {
                leftCam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, leftMat);
            } else {
                leftCam.projectionMatrix = leftMat;
            }

            leftCam.transform.rotation = transform.rotation;
        }

        if (rightCam != null) {
            Matrix4x4 rightMat = Util.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, rightCam.transform.position, head.nearClippingPlane, head.farClippingPlane);

            if (useXRCameras) {
                rightCam.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, rightMat);
            } else {
                rightCam.projectionMatrix = rightMat;
            }

            rightCam.transform.rotation = transform.rotation;
        }
    }

    void Update() {
        //it seems that Windows doe not immediately set the window properties so
        //we try over and over until it does
        #if UNITY_STANDALONE_WIN
        if (_resolutions != null) {
            Dictionary<long, Vector2Int> newResolutions = loadAllWindowSizes();
            foreach (var kvp in newResolutions) {
                if (kvp.Value.x == _setWidth && kvp.Value.y == _setHeight) {
                    SetWindowText(new IntPtr(kvp.Key), _desiredName);
                    SetWindowPos(new IntPtr(kvp.Key), 0, _desiredX, _desiredY, _desiredWidth, _desiredHeight, _desiredWidth * _desiredHeight == 0 ? 1 : 0);
                    _resolutions = null;

                    //we must also defer setting the camera viewports until the screen has the correct resolution
                    if (dualPipe && !dualInstance) {
                        leftCam.pixelRect  = new Rect(leftViewport.x,  leftViewport.y,  leftViewport.width,  leftViewport.height);
                        rightCam.pixelRect = new Rect(rightViewport.x, rightViewport.y, rightViewport.width, rightViewport.height);
                    }
                }
            }
        }
        #endif
    }

    void EditorDraw() {
        var mat = transform.localToWorldMatrix;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(UpperRight, UpperLeft);
        Gizmos.DrawLine(UpperLeft, LowerLeft);
        Gizmos.DrawLine(LowerLeft, LowerRight);
        Gizmos.DrawLine(LowerRight, UpperRight);
    }
    void OnDrawGizmos() {
        EditorDraw();
    }
    private void OnDrawGizmosSelected() {
        var mat = transform.localToWorldMatrix;
        Vector3 right = mat * new Vector4(halfWidth() * 0.75f, 0.0f, 0.0f, 1.0f);
        Vector3 up = mat * new Vector4(0.0f, halfHeight() * 0.75f, 0.0f, 1.0f);
        Gizmos.color = new Color(0.75f, 0.25f, 0.25f);
        Gizmos.DrawLine((transform.position * 2.0f + right) / 3.0f, right);
        Gizmos.color = new Color(0.25f, 0.75f, 0.25f);
        Gizmos.DrawLine((transform.position * 2.0f + up) / 3.0f, up);
    }

    public string serializedLocation = "serialization path";
    public void TryToSerialize(string path = null) {
        if (path == null) path = serializedLocation;
        System.IO.File.WriteAllText(path, JsonUtility.ToJson(GetSettings()));
        Debug.Log("Serialized settings to " + new System.IO.FileInfo(path).FullName);
    }
    public void TryToDeSerialize(string path = null) {
        if (path == null) path = serializedLocation;
        SetSettings(JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(path)));
        Debug.Log("Deserialized settings from " + new System.IO.FileInfo(path).FullName);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PhysicalDisplay))]
public class PhysicalDisplayDitor : Editor {
    public override void OnInspectorGUI() {
        PhysicalDisplay display = target as PhysicalDisplay;

        PhysicalDisplayManager newMan = (PhysicalDisplayManager)EditorGUILayout.ObjectField("Manager", display.manager, typeof(PhysicalDisplayManager), true);
        if(newMan != display.manager) {
            if(newMan != null) {
                newMan.displays.Add(display);
            }
            if(display.manager != null) {
                display.manager.displays.Remove(display);
            }
        }
        display.manager = newMan;

        if(newMan == null) display.machineName = EditorGUILayout.TextField("Machine Name", display.machineName);
        display.width = EditorGUILayout.FloatField("Physical Width", display.width);
        display.height = EditorGUILayout.FloatField("Physical Height", display.height);
        display.head = (HeadConfiguration)EditorGUILayout.ObjectField("Head", display.head, typeof(HeadConfiguration), true);

        display.useXRCameras = EditorGUILayout.Toggle(new GUIContent(
            "Use XR Cameras",
            @"Whether the cameras associated with this display should output to an XR device (such as headset or passive 3D display)
            If you do post processing on the cameras (such as a PhysicalDisplayCalibration) set this to false
            This is probably also unnecessary if using a Dual-Pipe 3D display"
            ), display.useXRCameras
        );
        display.useRenderTextures = EditorGUILayout.Toggle(new GUIContent("Use Render Textures", "Render to render textures instead of screen, for post processing"), display.useRenderTextures);
        if(display.useRenderTextures) {
            display.renderTextureSize = EditorGUILayout.Vector2IntField("Render Texture Size", display.renderTextureSize);
        }

        if (!display.useRenderTextures) {
            if (display.exclusiveFullscreen = EditorGUILayout.Toggle("Use Specific Display", display.exclusiveFullscreen))
            {
                display.display = EditorGUILayout.IntField("Display", display.display);
            }
        }

        if (display.is3D = EditorGUILayout.Toggle("Is 3D", display.is3D)) {
            if(display.dualPipe = EditorGUILayout.Toggle(new GUIContent("Dual Pipe", "Does the display use a dual pipe setup?"), display.dualPipe)) {
                if(!display.exclusiveFullscreen && !(display.dualInstance = EditorGUILayout.Toggle(new GUIContent("Dual Instance", "Use one instance of Unity for each eye?"), display.dualInstance))) {
                    //3d, dual pipe, single instance
                    display.windowBounds = EditorGUILayout.RectIntField(new GUIContent("Window Rect", "Where the window will be positioned on the screen"), display.windowBounds);
                }
                display.leftViewport = EditorGUILayout.RectIntField("Left Viewport", display.leftViewport);
                display.rightViewport = EditorGUILayout.RectIntField("Right Viewport", display.rightViewport);
            } else {
                display.windowBounds = EditorGUILayout.RectIntField(new GUIContent("Window Rect", "Where the window will be positioned on the screen"), display.windowBounds);
            }
        } else {
            display.windowBounds = EditorGUILayout.RectIntField(new GUIContent("Window Rect", "Where the window will be positioned on the screen"), display.windowBounds);
        }

        List<string> errors = display.GetSettingsErrors();
        if(errors.Count != 0) {
            GUIStyle style = new GUIStyle();
            style.richText = true;
            EditorGUILayout.LabelField("<color=red>Warning: This PhysicalDisplay has some incompatible or invalid settings, behavior may be undefined!</color>", style);
        }
        foreach(string error in errors) {
            GUIStyle style = new GUIStyle();
            style.richText = true;
            EditorGUILayout.LabelField("<color=red>" + error + "</color>", style);
        }

        display.serializedLocation = EditorGUILayout.TextField(display.serializedLocation);
        display.loadSettingsAtRuntime = EditorGUILayout.Toggle("Load Settings At Runtime", display.loadSettingsAtRuntime);
        if (GUILayout.Button("Export Display Settings to JSON")) {
            display.TryToSerialize();
        }
        if (GUILayout.Button("Import Display Settings from JSON")) {
            display.TryToDeSerialize();
        }

        if (GUI.changed) {
            EditorUtility.SetDirty(display);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
#endif