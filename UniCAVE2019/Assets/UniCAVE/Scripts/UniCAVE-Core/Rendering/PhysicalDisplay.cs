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

    private bool updatedViewports = false;
    
    /// <summary>
    /// Camera for rendering stereo left eye
    /// </summary>
    [NonSerialized]
    public Camera leftCam;

    /// <summary>
    /// Camera for rendering non-stereo eye or passive-stereo left and right
    /// </summary>
    [NonSerialized]
    public Camera centerCam;

    /// <summary>
    /// Camera for rendering stereo right eye
    /// </summary>
    [NonSerialized]
    public Camera rightCam;

    /// <summary>
    /// If post processing is used, the texture rendered to stereo left
    /// </summary>
    [NonSerialized]
    public RenderTexture leftTex;

    /// <summary>
    /// If post processing is used, the texture rendered to non-stereo
    /// </summary>
    [NonSerialized]
    public RenderTexture centerTex;

    /// <summary>
    /// If post processing is used, the texture rendered to stereo right
    /// </summary>
    [NonSerialized]
    public RenderTexture rightTex;

    /// <summary>
    /// Whether all setup operations are complete
    /// </summary>
    [NonSerialized]
    bool initialized = false;

    /// <summary>
    /// Whether all setup operations are complete
    /// </summary>
    /// <returns>initialized</returns>
    public bool Initialized() {
        return initialized;
    }

    /// <summary>
    /// Whether or not this display is enabled for this machine.
    /// If has manager, cedes decision to the manager
    /// When in editor, always true.
    /// </summary>
    /// <returns>Whether to enable this display</returns>
    public bool ShouldBeActive() {
#if UNITY_EDITOR
        return true;
#else
        //machine name doesn't matter if it has a manager
        //in that case we defer to manager's machine name
        return (manager == null ? (Util.GetMachineName() == machineName) : manager.ShouldBeActive());
#endif
    }

    /// <summary>
    /// A list of every camera associated with this display
    /// </summary>
    /// <returns>A list of every camera associated with this display
    /// </summary></returns>
    public List<Camera> GetAllCameras() {
        List<Camera> res = new List<Camera>();
        if (centerCam != null) res.Add(centerCam);
        if (leftCam != null) res.Add(leftCam);
        if (rightCam != null) res.Add(rightCam);
        return res;
    }

    /// <summary>
    /// Convert a coordinate in screenspace of this display to worldspace
    /// </summary>
    /// <param name="ss">Screenspace coordinate in range {[-1,1], [-1,1]} </param>
    /// <returns>The world space position of ss</returns>
    public Vector3 ScreenspaceToWorld(Vector2 ss) {
        return transform.localToWorldMatrix * new Vector4(ss.x * halfWidth(), ss.y * halfHeight(), 0.0f, 1.0f);
    }

    /// <summary>
    /// Upper right (quadrant 1) corner world space coordinate
    /// </summary>
    public Vector3 UpperRight {
        get {
            return transform.localToWorldMatrix * new Vector4(halfWidth(), halfHeight(), 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// Upper left (quadrant 2) corner world space coordinate
    /// </summary>
    public Vector3 UpperLeft {
        get {
            return transform.localToWorldMatrix * new Vector4(-halfWidth(), halfHeight(), 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// Lower left (quadrant 3) corner world space coordinate
    /// </summary>
    public Vector3 LowerLeft {
        get {
            return transform.localToWorldMatrix * new Vector4(-halfWidth(), -halfHeight(), 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// Lower right (quadrant 4) corner world space coordinate
    /// </summary>
    public Vector3 LowerRight {
        get {
            return transform.localToWorldMatrix * new Vector4(halfWidth(), -halfHeight(), 0.0f, 1.0f);
        }
    }

    /// <summary>
    /// Generate a list of potential errors associated with this display, printed to console during runtime and shown in the script editor
    /// May include false positives and false negatives, this is just used to bring attention to common issues
    /// </summary>
    /// <returns>A list of potential script configuration errors</returns>
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
        if (exclusiveFullscreen && dualPipe) {
            errors.Add("Physical display is in dual pipe mode but also uses a certain display");
        }
        if (exclusiveFullscreen && manager != null) {
            errors.Add("Physical display uses exlusive fullscreen, but is also managed");
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
        if (useXRCameras && !is3D) {
            errors.Add("Physical Display uses XR cameras but is not 3D");
        }
        if (exclusiveFullscreen &&
            (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 ||
            SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 ||
            SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore)) {
            errors.Add("Physical Display uses a certain display but the Unity Player isn't using DirectX");
        }
		
        if (is3D && !dualPipe && (XRSettings.supportedDevices.Length < 1 || XRSettings.supportedDevices[0] != "stereo")) {
            errors.Add(gameObject.name + " expecting quad-buffered stereo but VR Supported - \"Stereo display non-head mounted\" not enabled.");
        }

        return errors;
    }

    /// <summary>
    /// Copies the values of a Settings object into this script instance
    /// </summary>
    /// <param name="settings">Settings to load in</param>
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

    /// <summary>
    /// Get a Settings object associated with the settings of this script instance
    /// </summary>
    /// <returns>Script settings</returns>
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

    /// <summary>
    /// Disable this object if it shouldn't be active
    /// Otherwise, create and assign cameras, reposition window, etc
    /// </summary>
    void Start() {

        centerCam = null;
        leftCam = null;
        rightCam = null;

        if (loadSettingsAtRuntime) {
            Debug.Log("Attempting to Load Settings for Display: " + gameObject.name);
            TryToDeSerialize(serializedLocation);
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
            if (display < Display.displays.Length) {
                Display.displays[display].Activate();
            } else {
                Debug.Log("Display Error for Display: " + gameObject.name + ": Physical Display uses display index " + display + " but Unity does not detect that many displays");
            }
        }


        if (!exclusiveFullscreen) {
            Debug.Log("Setting Display: " + gameObject.name + " to Windowed Mode...");
            if (!is3D || useXRCameras) {
                centerCam = head.CreateCenterEye(gameObject.name);
                if (useXRCameras) {
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                    StereoBlit blit = centerCam.gameObject.AddComponent<StereoBlit>();
                    blit.lcam = leftCam;
                    blit.rcam = rightCam;
                    Debug.Log("Created dummy cams");
                }
                Debug.Log("Setting Display: " + gameObject.name + " to Non-3D Windowed");
                #if UNITY_STANDALONE_WIN
                if (manager == null) WindowsUtils.SetMyWindowInfo("Non-3D Windowed", windowBounds.x, windowBounds.y, windowBounds.width, windowBounds.height, 441, 411);
                #endif
            } else {
                if (!dualPipe && !dualInstance) {
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                    Debug.Log("Setting Display: " + gameObject.name + " to Quad Buffer 3D Windowed");
                    #if UNITY_STANDALONE_WIN
                    if (manager == null) WindowsUtils.SetMyWindowInfo("Quad Buffer 3D Windowed", windowBounds.x, windowBounds.y, windowBounds.width, windowBounds.height, 421, 420);
                    #endif
                } else if (dualPipe && !dualInstance) {
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                    Debug.Log("Setting Display: " + gameObject.name + " to Dual-Eye Dual-Pipe-3D Windowed");
                    #if UNITY_STANDALONE_WIN
                    if (manager == null) WindowsUtils.SetMyWindowInfo("Dual-Eye Dual-Pipe-3D Windowed", windowBounds.x, windowBounds.y, windowBounds.width, windowBounds.height, 422, 398);
                    #endif
                } else if (dualPipe && dualInstance) {
                    if (Util.GetArg("eye") == "left") {
                        leftCam = head.CreateLeftEye(gameObject.name);
                        Debug.Log("Setting Display: " + gameObject.name + " to Left-Eye Dual-Pipe-3D Windowed");
                        #if UNITY_STANDALONE_WIN
                        if (manager == null) WindowsUtils.SetMyWindowInfo("Left-Eye Dual-Pipe-3D Windowed", leftViewport.x, leftViewport.y, leftViewport.width, leftViewport.height, 300, 367);
                        #endif
                    } else if (Util.GetArg("eye") == "right") {
                        rightCam = head.CreateRightEye(gameObject.name);
                        Debug.Log("Setting Display: " + gameObject.name + " to Right-Eye Dual-Pipe-3D Windowed");
                        #if UNITY_STANDALONE_WIN
                        if (manager == null) WindowsUtils.SetMyWindowInfo("Right-Eye Dual-Pipe-3D Windowed", rightViewport.x, rightViewport.y, rightViewport.width, rightViewport.height, 342, 498);
                        #endif
                    }
                }
            }
        } else {
            if (!is3D || useXRCameras) {
                centerCam = head.CreateCenterEye(gameObject.name);
                if (useXRCameras) {
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                    StereoBlit blit = centerCam.gameObject.AddComponent<StereoBlit>();
                    blit.lcam = leftCam;
                    blit.rcam = rightCam;
                    Debug.Log("Created dummy cams");
                }
            } else {
                if (!dualPipe && !dualInstance) {
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                } else if (dualPipe && !dualInstance) {
                    leftCam = head.CreateLeftEye(gameObject.name);
                    rightCam = head.CreateRightEye(gameObject.name);
                } else if (dualPipe && dualInstance) {
                    if (Util.GetArg("eye") == "left") {
                        leftCam = head.CreateLeftEye(gameObject.name);
                    } else if (Util.GetArg("eye") == "right") {
                        rightCam = head.CreateRightEye(gameObject.name);
                    }
                }
            }
        }

        if (useRenderTextures) {
            if (leftCam != null) {
                leftTex = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0);
                leftCam.targetTexture = leftTex;
                leftTex.name = Util.ObjectFullName(leftCam.gameObject) + " Target Tex";
            }
            if (centerCam != null) {
                centerTex = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0);
                centerCam.targetTexture = centerTex;
                centerTex.name = Util.ObjectFullName(centerCam.gameObject) + " Target Tex";
            }
            if (rightCam != null) {
                rightTex = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0);
                rightCam.targetTexture = rightTex;
                rightTex.name = Util.ObjectFullName(rightCam.gameObject) + " Target Tex";
            }
        }

        if (exclusiveFullscreen) {
            if (leftCam != null) leftCam.targetDisplay = display;
            if (centerCam != null) centerCam.targetDisplay = display;
            if (rightCam != null) rightCam.targetDisplay = display;
        }
#if UNITY_EDITOR
        if (rightCam == null && leftCam == null && centerCam == null) {
            if (!is3D || useXRCameras) {
                centerCam = head.CreateCenterEye(gameObject.name);
            } else {
                leftCam = head.CreateLeftEye(gameObject.name);
                rightCam = head.CreateRightEye(gameObject.name);
            }
        }
#endif
    }

    /// <summary>
    /// Assign camera matrices for all associated cameras
    /// </summary>
    private void LateUpdate() {
        if (leftCam != null) {
            Matrix4x4 leftMat = Util.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, leftCam.transform.position, head.nearClippingPlane, head.farClippingPlane);
            leftCam.projectionMatrix = leftMat;
            leftCam.transform.rotation = transform.rotation;
        }

        if (rightCam != null) {
            Matrix4x4 rightMat = Util.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, rightCam.transform.position, head.nearClippingPlane, head.farClippingPlane);
            rightCam.projectionMatrix = rightMat;
            rightCam.transform.rotation = transform.rotation;
        }

        if (centerCam != null) {
            if (!useXRCameras) {
                centerCam.projectionMatrix = Util.getAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, centerCam.transform.position, head.nearClippingPlane, head.farClippingPlane);
            }
            centerCam.transform.rotation = transform.rotation;
        }
    }

    /// <summary>
    /// Reposition window (takes multiple frames to complete)
    /// </summary>
    void Update() {
        if(!updatedViewports && WindowsUtils.CompletedOperation()) {
            //we must also defer setting the camera viewports until the screen has the correct resolution
            if (dualPipe && !dualInstance) {
                leftCam.pixelRect = new Rect(leftViewport.x, leftViewport.y, leftViewport.width, leftViewport.height);
                rightCam.pixelRect = new Rect(rightViewport.x, rightViewport.y, rightViewport.width, rightViewport.height);
            }
            updatedViewports = true;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Draw debug view in editor
    /// </summary>
    void EditorDraw() {
        var mat = transform.localToWorldMatrix;
        Gizmos.color = Color.white;
        Gizmos.DrawLine(UpperRight, UpperLeft);
        Gizmos.DrawLine(UpperLeft, LowerLeft);
        Gizmos.DrawLine(LowerLeft, LowerRight);
        Gizmos.DrawLine(LowerRight, UpperRight);
    }

    /// <summary>
    /// Draw debug view in editor
    /// </summary>
    void OnDrawGizmos() {
        EditorDraw();
    }

    /// <summary>
    /// Draw advanced debug view in editor when selected
    /// </summary>
    private void OnDrawGizmosSelected() {
        var mat = transform.localToWorldMatrix;
        Vector3 right = mat * new Vector4(halfWidth() * 0.75f, 0.0f, 0.0f, 1.0f);
        Vector3 up = mat * new Vector4(0.0f, halfHeight() * 0.75f, 0.0f, 1.0f);
        Gizmos.color = new Color(0.75f, 0.25f, 0.25f);
        Gizmos.DrawLine((transform.position * 2.0f + right) / 3.0f, right);
        Gizmos.color = new Color(0.25f, 0.75f, 0.25f);
        Gizmos.DrawLine((transform.position * 2.0f + up) / 3.0f, up);
    }
#endif

    /// <summary>
    /// Where to load and save serialized Settings from
    /// </summary>
    public string serializedLocation = "settings.json";

    /// <summary>
    /// Try to save Settings object0 to path
    /// </summary>
    /// <param name="path">path to save to</param>
    public void TryToSerialize(string path) {
        System.IO.File.WriteAllText(path, JsonUtility.ToJson(GetSettings()));
        Debug.Log("Serialized settings to " + new System.IO.FileInfo(path).FullName);
    }

    /// <summary>
    /// Try to load Settings object from path
    /// </summary>
    /// <param name="path">path to load from</param>
    public void TryToDeSerialize(string path) {
        SetSettings(JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(path)));
        Debug.Log("Deserialized settings from " + new System.IO.FileInfo(path).FullName);
    }
}

#if UNITY_EDITOR
/// <summary>
/// Editor for this display; some options are not always valid so they are disabled in some settings configurations
/// </summary>
[CustomEditor(typeof(PhysicalDisplay))]
public class PhysicalDisplayDitor : Editor {

    /// <summary>
    /// Render script GUI in editor
    /// </summary>
    public override void OnInspectorGUI() {
        PhysicalDisplay display = target as PhysicalDisplay;

        PhysicalDisplayManager newMan = (PhysicalDisplayManager)EditorGUILayout.ObjectField("Manager", display.manager, typeof(PhysicalDisplayManager), true);
        if (newMan != display.manager) {
            if (newMan != null) {
                newMan.displays.Add(display);
            }
            if (display.manager != null) {
                display.manager.displays.Remove(display);
            }
        }
        display.manager = newMan;

        if (newMan == null) display.machineName = EditorGUILayout.TextField("Machine Name", display.machineName);
        display.width = EditorGUILayout.FloatField("Physical Width", display.width);
        display.height = EditorGUILayout.FloatField("Physical Height", display.height);
        display.head = (HeadConfiguration)EditorGUILayout.ObjectField("Head", display.head, typeof(HeadConfiguration), true);

        display.useXRCameras = EditorGUILayout.Toggle(new GUIContent(
            "Use XR Cameras (Quad Buffer)",
            @"Whether the cameras associated with this display should output to an XR device (such as headset or quad-buffered stereo 3D display)
            If you do post processing on the cameras (such as a PhysicalDisplayCalibration) set this to false
            This is probably also unnecessary if using a Dual-Pipe 3D display"
            ), display.useXRCameras
        );
        display.useRenderTextures = EditorGUILayout.Toggle(new GUIContent("Use Render Textures", "Render to render textures instead of screen, for post processing"), display.useRenderTextures);
        if (display.useRenderTextures) {
            display.renderTextureSize = EditorGUILayout.Vector2IntField("Render Texture Size", display.renderTextureSize);
        }

        if (!display.useRenderTextures && display.manager == null) {
            if (display.exclusiveFullscreen = EditorGUILayout.Toggle("Use Specific Display", display.exclusiveFullscreen)) {
                display.display = EditorGUILayout.IntField("Display", display.display);
            }
        }
        if (display.manager != null) display.exclusiveFullscreen = false;

        if (display.is3D = EditorGUILayout.Toggle("Is 3D", display.is3D)) {
            if (display.dualPipe = EditorGUILayout.Toggle(new GUIContent("Dual Pipe", "Does the display use a dual pipe setup?"), display.dualPipe)) {
                if (!display.exclusiveFullscreen && !(display.dualInstance = EditorGUILayout.Toggle(new GUIContent("Dual Instance", "Use one instance of Unity for each eye?"), display.dualInstance))) {
                    //3d, dual pipe, single instance
                    display.windowBounds = EditorGUILayout.RectIntField(new GUIContent("Viewport Rect", "Where the window will be positioned on the screen"), display.windowBounds);
                }
                display.leftViewport = EditorGUILayout.RectIntField("Left Viewport", display.leftViewport);
                display.rightViewport = EditorGUILayout.RectIntField("Right Viewport", display.rightViewport);
            } else {
                display.windowBounds = EditorGUILayout.RectIntField(new GUIContent("Viewport Rect", "Where the window will be positioned on the screen"), display.windowBounds);
                display.dualInstance = false;
            }
        } else {
            display.windowBounds = EditorGUILayout.RectIntField(new GUIContent("Viewport Rect", "Where the window will be positioned on the screen"), display.windowBounds);
            display.dualPipe = false;
            display.dualInstance = false;
        }

        List<string> errors = display.GetSettingsErrors();
        if (errors.Count != 0) {
            GUIStyle style = new GUIStyle();
            style.richText = true;
            EditorGUILayout.LabelField("<color=red>This PhysicalDisplay has some incompatible or invalid settings, behavior may be undefined!</color>", style);
        }
        foreach (string error in errors) {
            GUIStyle style = new GUIStyle();
            style.richText = true;
            EditorGUILayout.LabelField("<color=red>" + error + "</color>", style);
        }

        
        if(display.loadSettingsAtRuntime = EditorGUILayout.Toggle("Load Settings At Runtime", display.loadSettingsAtRuntime)) {
            display.serializedLocation = EditorGUILayout.TextField(display.serializedLocation);
        }
        if (GUILayout.Button("Export Display Settings to JSON")) {
            string path = EditorUtility.SaveFilePanel("Export Settings", "./", "settings.json", "json");
            if(path != null) {
                display.TryToSerialize(path);
            }
        }
        if (GUILayout.Button("Import Display Settings from JSON")) {
            string path = EditorUtility.SaveFilePanel("Import Settings", "./", "settings.json", "json");
            if (path != null) {
                display.TryToDeSerialize(path);
            }
            
        }

        if (GUI.changed) {
            EditorUtility.SetDirty(display);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}
#endif