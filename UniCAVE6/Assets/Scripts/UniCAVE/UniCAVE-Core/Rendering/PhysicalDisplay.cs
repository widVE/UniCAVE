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
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniCAVE
{

    /// <summary>
    /// Used to transfer display configuration settings in a single block. Fields have the same meanings as they do
    /// in <c>PhysicalDisplay</c>.
    /// </summary>
    struct Settings
    {
        public PhysicalDisplayManager Manager;

        public string OldMachineName;
        public MachineName MachineNameAsset;
        public string MachineName => UniCAVE.MachineName.GetMachineName(OldMachineName, MachineNameAsset);

        public float Width;
        public float Height;
        public float HalfWidth => Width * 0.5f;
        public float HalfHeight => Height * 0.5f;
        public HeadConfiguration Head;

        public bool UseXRCameras;
        public bool UseRenderTextures;
        public Vector2Int RenderTextureSize;

        public bool Is3D;
        public bool ExclusiveFullscreen;
        public int Display;
        public bool DualPipe;
        public bool DualInstance;
        public RectInt WindowBounds;
        public RectInt LeftViewport;
        public RectInt RightViewport;
    }

    [Serializable]
    public class PhysicalDisplay : MonoBehaviour
    {
        /// <summary>
        /// Manager for this display. Manager is only used if several displays are being combined into a single
        /// output. This will usually be NULL in an actaul CAVE.
        /// </summary>
        public PhysicalDisplayManager manager;

        [UnityEngine.Serialization.FormerlySerializedAs("machineName")]
        public string oldMachineName;
        public MachineName machineNameAsset;
        /// <summary>
        /// Machine name that should host this display. In dual instance mode, several machines may have the same
        /// name if they render different eyes of the same display
        /// </summary>
        public string MachineName => UniCAVE.MachineName.GetMachineName(oldMachineName, machineNameAsset);

        /// <summary>
        /// Width of the physical display.
        /// </summary>
        public float width;

        /// <summary>
        /// Height of the physical display.
        /// </summary>
        public float height;
        public float HalfWidth => width * 0.5f;
        public float HalfHeight => height * 0.5f;
        public HeadConfiguration head;

        /// <summary>
        /// If set, 3D cameras are created but a software <c>StereoBlit</c> is used to alternate them to create the
        /// center eye output.
        /// </summary>
        public bool useXRCameras;

        /// <summary>
        /// If set, the cameras are rendered to textures <c>LeftTex</c>, <c>CenterTex</c> and <c>RightTex</c> for
        /// post processing.
        /// </summary>
        public bool useRenderTextures;
        public Vector2Int renderTextureSize;

        /// <summary>
        /// Is this display 3D capable? If so, the left and right cameras will be used. If not, only the center camera will be used.
        /// This does not imply that the display hardware actually supports 3D output.
        /// </summary>
        public bool is3D;

        /// <summary>
        /// Should we run full screen or in a window? Note that the output may appear full screen even if this is false if
        /// the window is maximised.
        /// </summary>
        public bool exclusiveFullscreen;

        /// <summary>
        /// Display number to draw on. This is the ordinal number of the display as detected by Unity.
        /// This is ignored if <c>exclusiveFullscreen</c> is not set (even if the window is maximised).
        /// </summary>
        public int display;

        /// <summary>
        /// Is this a dual pipe display, with both eyes displayed side by side in a single image? If not, we assume it's
        /// frame alternating. Also, if <c>dualInstance</c> is true then this is ignored as we assume the outputs from
        /// the two instances are being combined somehow.
        /// </summary>
        [Tooltip("Does the display use a dual pipe setup?")]
        public bool dualPipe;

        /// <summary>
        /// Is this a dual instance display, with a separate instance of Unity running for each eye? If so, we expect
        /// there to be an <c>eye</c> argument saying which eye this instance should be.
        /// </summary>
        [Tooltip("Use one instance of Unity for each eye?")]
        public bool dualInstance;


        [Tooltip("Where the window will be positioned on the screen")]
        public RectInt windowBounds;
        public RectInt leftViewport;
        public RectInt rightViewport;

        public bool loadSettingsAtRuntime;

        /// <summary>
        /// Used to defer calculating viewports until after resizing of the window in windowed mode.
        /// </summary>
        private bool _updatedViewports = false;

        /// <summary>
        /// Camera for rendering stereo left eye.
        /// </summary>
        [NonSerialized]
        public Camera LeftCam;

        /// <summary>
        /// Camera for rendering non-stereo eye or passive-stereo left and right.
        /// </summary>
        [NonSerialized]
        public Camera CenterCam;

        /// <summary>
        /// Camera for rendering stereo right eye.
        /// </summary>
        [NonSerialized]
        public Camera RightCam;

        /// <summary>
        /// If post processing is used, the texture rendered to stereo left.
        /// </summary>
        [NonSerialized]
        public RenderTexture LeftTex;

        /// <summary>
        /// If post processing is used, the texture rendered to non-stereo.
        /// </summary>
        [NonSerialized]
        public RenderTexture CenterTex;

        /// <summary>
        /// If post processing is used, the texture rendered to stereo right.
        /// </summary>
        [NonSerialized]
        public RenderTexture RightTex;

        /// <summary>
        /// Whether all setup operations are complete.
        /// </summary>
        [NonSerialized]
        bool _initialized = false;

        /// <summary>
        /// Whether all setup operations are complete.
        /// </summary>
        /// <returns>initialized</returns>
        /// TODO this could be changed to a private setter property.
        public bool Initialized() => _initialized;

        /// <summary>
        /// Whether or not this display is enabled for this machine.
        /// If has manager, cedes decision to the manager.
        /// When in editor, always true.
        /// Otherwise, true if actual machine name matches machine name for this display.
        /// </summary>
        /// <returns>Whether to enable this display.</returns>
        public bool ShouldBeActive()
        {
#if UNITY_EDITOR
            return true;
#else
            //machine name doesn't matter if it has a manager
            //in that case we defer to manager's machine name
            return (manager == null ? (Util.GetMachineName() == MachineName) : manager.ShouldBeActive);
#endif
        }

        /// <summary>
        /// A list of every camera associated with this display.
        /// </summary>
        /// <returns>A list of every camera associated with this display.
        /// </returns>
        public List<Camera> GetAllCameras()
        {
            List<Camera> res = new();
            if (CenterCam != null) res.Add(CenterCam);
            if (LeftCam != null) res.Add(LeftCam);
            if (RightCam != null) res.Add(RightCam);
            return res;
        }

        /// <summary>
        /// Convert a coordinate in screenspace of this display to worldspace
        /// </summary>
        /// <param name="ss">Screenspace coordinate in range {[-1,1], [-1,1]} </param>
        /// <returns>The world space position of ss (on the plane of the screen)</returns>
        public Vector3 ScreenspaceToWorld(Vector2 ss)
        {
            return transform.localToWorldMatrix * new Vector4(ss.x * HalfWidth, ss.y * HalfHeight, 0.0f, 1.0f);
        }

        /// <summary>
        /// Upper right (quadrant 1) corner world space coordinate
        /// </summary>
        public Vector3 UpperRight => transform.localToWorldMatrix * new Vector4(HalfWidth, HalfHeight, 0.0f, 1.0f);

        /// <summary>
        /// Upper left (quadrant 2) corner world space coordinate
        /// </summary>
        public Vector3 UpperLeft => transform.localToWorldMatrix * new Vector4(-HalfWidth, HalfHeight, 0.0f, 1.0f);

        /// <summary>
        /// Lower left (quadrant 3) corner world space coordinate
        /// </summary>
        public Vector3 LowerLeft => transform.localToWorldMatrix * new Vector4(-HalfWidth, -HalfHeight, 0.0f, 1.0f);

        /// <summary>
        /// Lower right (quadrant 4) corner world space coordinate
        /// </summary>
        public Vector3 LowerRight => transform.localToWorldMatrix * new Vector4(HalfWidth, -HalfHeight, 0.0f, 1.0f);

        /// <summary>
        /// Generate a list of potential errors associated with this display, printed to console during runtime and shown in the script editor
        /// May include false positives and false negatives, this is just used to bring attention to common issues
        /// </summary>
        /// <returns>A list of potential script configuration errors</returns>
        public List<string> GetSettingsErrors()
        {
            List<string> errors = new();
            if (head == null) errors.Add("Physical Display must have a reference to a GameObject with Head Configuration script!");
            if (string.IsNullOrEmpty(MachineName) && manager == null) errors.Add("Physical Display should probably have a non-empty machine name");
            if (width <= 0 || height <= 0) errors.Add("Physical Display has impossible dimensions");
            if (exclusiveFullscreen && useRenderTextures) errors.Add("Physical Display uses renderTextures but also uses a certain display");
            if (exclusiveFullscreen && dualPipe) errors.Add("Physical display is in dual pipe mode but also uses a certain display");
            if (exclusiveFullscreen && manager != null) errors.Add("Physical display uses exclusive fullscreen, but is also managed");
            if (dualInstance && manager != null) errors.Add("Physical Display has dual instance enabled, but the display is also managed");
            if (useRenderTextures && manager != null && GetComponent<PhysicalDisplayCalibration>() == null) errors.Add("Physical Display uses render textures but is also managed");
            if (useXRCameras && dualPipe) errors.Add("Physical Display uses XR cameras but is also dual pipe");
            if (useXRCameras && !is3D) errors.Add("Physical Display uses XR cameras but is not 3D");

            if (exclusiveFullscreen &&
                SystemInfo.graphicsDeviceType is UnityEngine.Rendering.GraphicsDeviceType.OpenGLES2 or UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3 or UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore)
            {
                errors.Add("Physical Display uses a certain display but the Unity Player isn't using DirectX");
            }

            if (is3D && !dualPipe && (XRSettings.supportedDevices.Length < 1 || XRSettings.supportedDevices[0] != "stereo"))
            {
                errors.Add(gameObject.name + " expecting quad-buffered stereo but VR Supported - \"Stereo display non-head mounted\" not enabled.");
            }
            return errors;
        }

        /// <summary>
        /// Copies the values of a Settings object into this script instance
        /// </summary>
        /// <param name="settings">Settings to load in</param>
        void SetSettings(Settings settings)
        {
            manager = settings.Manager;
            head = settings.Head;
            machineNameAsset = settings.MachineNameAsset;
            width = settings.Width;
            height = settings.Height;

            useXRCameras = settings.UseXRCameras;
            useRenderTextures = settings.UseRenderTextures;
            renderTextureSize = settings.RenderTextureSize;

            is3D = settings.Is3D;
            exclusiveFullscreen = settings.ExclusiveFullscreen;
            display = settings.Display;
            dualPipe = settings.DualPipe;
            dualInstance = settings.DualInstance;
            windowBounds = settings.WindowBounds;
            leftViewport = settings.LeftViewport;
            rightViewport = settings.RightViewport;
        }

        /// <summary>
        /// Get a Settings object associated with the settings of this script instance
        /// </summary>
        /// <returns>Script settings</returns>
        Settings GetSettings()
        {
            return new Settings
            {
                Manager = manager,
                Head = head,
                MachineNameAsset = machineNameAsset,
                Width = width,
                Height = height,
                UseXRCameras = useXRCameras,
                UseRenderTextures = useRenderTextures,
                RenderTextureSize = renderTextureSize,
                Is3D = is3D,
                ExclusiveFullscreen = exclusiveFullscreen,
                Display = display,
                DualPipe = dualPipe,
                DualInstance = dualInstance,
                WindowBounds = windowBounds,
                LeftViewport = leftViewport,
                RightViewport = rightViewport
            };
        }

        /// <summary>
        /// Disable this object if it shouldn't be active
        /// Otherwise, create and assign cameras, reposition window, etc
        /// </summary>
        void Start()
        {
            Cursor.visible = false;

            CenterCam = null;
            LeftCam = null;
            RightCam = null;

            if (loadSettingsAtRuntime)
            {
                Debug.Log("Attempting to Load Settings for Display: " + gameObject.name);
                TryToDeSerialize(serializedLocation);
            }

            //Debug.Log("EXCLUSIVE: " + (exclusiveFullscreen ? "YES" : "NO"));

            if (!ShouldBeActive())
            {
                Debug.Log("Deactivating Display: " + gameObject.name);
                gameObject.SetActive(false);
                return;
            }

            Debug.Log("Display Active: " + gameObject.name);

            //foreach (string er in GetSettingsErrors())
            //{
            //    Debug.Log("Display Warning for Display: " + gameObject.name + ": " + er);
            //}

            //if (head == null)
            //{
            //    Debug.Log("Display Error for Display: " + gameObject.name + ": Physical Display has no head object");
            //}

            _initialized = true;

            if (exclusiveFullscreen)
            {
                if (display < Display.displays.Length)
                {
                    Display.displays[display].Activate();
                }
                else
                {
                    Debug.Log("Display Error for Display: " + gameObject.name + ": Physical Display uses display index " + display + " but Unity does not detect that many displays");
                }
            }


            // TODO Seems to be a lot of repeated code here
            if (!exclusiveFullscreen)
            {
                Debug.Log("Setting Display: " + gameObject.name + " to Windowed Mode...");
                if (!is3D || useXRCameras)
                {
                    CenterCam = head.CreateEye(HeadConfiguration.Eyes.Center, gameObject.name);
                    if (useXRCameras)
                    {
                        LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name);
                        RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name);
                        StereoBlit blit = CenterCam.gameObject.AddComponent<StereoBlit>();
                        blit.lcam = LeftCam;
                        blit.rcam = RightCam;
                        Debug.Log("Created dummy cams");
                    }
                    Debug.Log("Setting Display: " + gameObject.name + " to Non-3D Windowed");
#if UNITY_STANDALONE_WIN
                    if (manager == null)
                    {
                        // Debug.Log("BOOOOOOOOO");
                        WindowsUtils.SetMyWindowInfo("Non-3D Windowed", windowBounds.x, windowBounds.y, windowBounds.width, windowBounds.height, 441, 411);
                    }
#endif
                }
                else
                {
                    if (!dualPipe && !dualInstance)
                    {
                        if(head.camPrefab == null)
                        {
                            LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name);
                            RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name);
                            Debug.Log("Setting Display: " + gameObject.name + " to Quad Buffer 3D Windowed");
#if UNITY_STANDALONE_WIN
                            if (manager == null) WindowsUtils.SetMyWindowInfo("Quad Buffer 3D Windowed", windowBounds.x, windowBounds.y, windowBounds.width, windowBounds.height, 421, 420);
#endif
                        }
                    }
                    else if (dualPipe && !dualInstance)
                    {
                        LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name);
                        RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name);
                        Debug.Log("Setting Display: " + gameObject.name + " to Dual-Eye Dual-Pipe-3D Windowed");
#if UNITY_STANDALONE_WIN
                        if (manager == null) WindowsUtils.SetMyWindowInfo("Dual-Eye Dual-Pipe-3D Windowed", windowBounds.x, windowBounds.y, windowBounds.width, windowBounds.height, 422, 398);
#endif
                    }
                    else if (dualPipe && dualInstance)
                    {
                        if (Util.GetArg("eye") == "left")
                        {
                            LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name);
                            Debug.Log("Setting Display: " + gameObject.name + " to Left-Eye Dual-Pipe-3D Windowed");
#if UNITY_STANDALONE_WIN
                            if (manager == null)
                            {
                                WindowsUtils.SetMyWindowInfo("Left-Eye Dual-Pipe-3D Windowed", leftViewport.x, leftViewport.y, leftViewport.width, leftViewport.height, 300, 367);
                            }
#endif
                        }
                        else if (Util.GetArg("eye") == "right")
                        {
                            RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name);
                            Debug.Log("Setting Display: " + gameObject.name + " to Right-Eye Dual-Pipe-3D Windowed");
#if UNITY_STANDALONE_WIN
                            if (manager == null)
                            {
                                WindowsUtils.SetMyWindowInfo("Right-Eye Dual-Pipe-3D Windowed", rightViewport.x, rightViewport.y, rightViewport.width, rightViewport.height, 342, 498);
                            }
#endif
                        }
                    }
                }
            }
            else
            {
                if (!is3D || useXRCameras)
                {
                    CenterCam = head.CreateEye(HeadConfiguration.Eyes.Center, gameObject.name);
                    if (useXRCameras)
                    {
                        LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name);
                        RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name);
                        StereoBlit blit = CenterCam.gameObject.AddComponent<StereoBlit>();
                        blit.lcam = LeftCam;
                        blit.rcam = RightCam;
                        Debug.Log("Created dummy cams");
                    }
                }
                else
                {
                    if (!dualPipe && !dualInstance)
                    {
                        LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name);
                        RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name);
                    }
                    else if (dualPipe && !dualInstance)
                    {
                        LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name);
                        RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name);
                    }
                    else if (dualPipe && dualInstance)
                    {
                        if (Util.GetArg("eye") == "left")
                        {
                            LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name);
                        }
                        else if (Util.GetArg("eye") == "right")
                        {
                            RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name);
                        }
                    }
                }
            }

            if (useRenderTextures)
            {
                if (LeftCam != null)
                {
                    LeftTex = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0);
                    LeftCam.targetTexture = LeftTex;
                    LeftTex.name = Util.ObjectFullName(LeftCam.gameObject) + " Target Tex";
                }
                if (CenterCam != null)
                {
                    CenterTex = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0);
                    CenterCam.targetTexture = CenterTex;
                    CenterTex.name = Util.ObjectFullName(CenterCam.gameObject) + " Target Tex";
                }
                if (RightCam != null)
                {
                    RightTex = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 0);
                    RightCam.targetTexture = RightTex;
                    RightTex.name = Util.ObjectFullName(RightCam.gameObject) + " Target Tex";
                }
            }

            if (exclusiveFullscreen)
            {
                if (LeftCam != null) LeftCam.targetDisplay = display;
                if (CenterCam != null) CenterCam.targetDisplay = display;
                if (RightCam != null) RightCam.targetDisplay = display;
            }
//#if UNITY_EDITOR
            if (RightCam == null && LeftCam == null && CenterCam == null)
            {
                if (!is3D || useXRCameras)
                {
                    CenterCam = head.CreateEye(HeadConfiguration.Eyes.Center, gameObject.name);
                }
                else
                {
                     if(head.camPrefab == null)
                     {
                        LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name);
                        RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name);
                     }
                     else
                     {
                        GameObject g = Instantiate(head.camPrefab, head.transform);
                        
                        //LeftCam = head.CreateEye(HeadConfiguration.Eyes.Left, gameObject.name, true);
                        //RightCam = head.CreateEye(HeadConfiguration.Eyes.Right, gameObject.name, true);
                        
                        //g.transform.GetChild(0).parent = RightCam.transform;
                        //g.transform.GetChild(0).parent = LeftCam.transform;

                        //LeftCam.transform.parent = g.transform;
                        //RightCam.transform.parent = g.transform;

                        //g.GetComponent<Camera>().enabled = false;
                        
                        //LeftCam.transform.GetChild(0).localPosition = Vector3.zero;
                        g.transform.GetChild(0).localPosition = head.GetEyeOffset(HeadConfiguration.Eyes.Right);
                        RightCam = g.transform.GetChild(0).GetComponent<Camera>();
                        //RightCam.transform.GetChild(0).localPosition = Vector3.zero;
                        g.transform.GetChild(1).localPosition = head.GetEyeOffset(HeadConfiguration.Eyes.Left);
                         LeftCam = g.transform.GetChild(1).GetComponent<Camera>();//g.transform.GetComponent<Camera>();
                     }
                }
            }
//#endif
        }

        /// <summary>
        /// Assign camera matrices for all associated cameras.
        /// </summary>
        private void LateUpdate()
        {
            if (LeftCam != null)
            {
                Matrix4x4 leftMat = Util.GetAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, LeftCam.transform.position, head.nearClippingPlane, head.farClippingPlane);
                LeftCam.projectionMatrix = leftMat;
                LeftCam.transform.rotation = transform.rotation;
            }

            if (RightCam != null)
            {
                Matrix4x4 rightMat = Util.GetAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, RightCam.transform.position, head.nearClippingPlane, head.farClippingPlane);
                RightCam.projectionMatrix = rightMat;
                RightCam.transform.rotation = transform.rotation;
            }

            if (CenterCam != null)
            {
                if (!useXRCameras)
                {
                    CenterCam.projectionMatrix = Util.GetAsymProjMatrix(LowerLeft, LowerRight, UpperLeft, CenterCam.transform.position, head.nearClippingPlane, head.farClippingPlane);
                }
                CenterCam.transform.rotation = transform.rotation;
            }
        }

        /// <summary>
        /// Used during initialization to set view rectangles after initial window resize has completed
        /// </summary>
        void Update()
        {
            if (_updatedViewports || !WindowsUtils.CompletedOperation()) return;
            //we must also defer setting the camera viewports until the screen has the correct resolution
            if (dualPipe && !dualInstance)
            {
                LeftCam.pixelRect = new Rect(leftViewport.x, leftViewport.y, leftViewport.width, leftViewport.height);
                RightCam.pixelRect = new Rect(rightViewport.x, rightViewport.y, rightViewport.width, rightViewport.height);
            }
            _updatedViewports = true;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw debug view in editor
        /// </summary>
        void EditorDraw()
        {
            Matrix4x4 mat = transform.localToWorldMatrix;
            Gizmos.color = Color.white;
            Gizmos.DrawLine(UpperRight, UpperLeft);
            Gizmos.DrawLine(UpperLeft, LowerLeft);
            Gizmos.DrawLine(LowerLeft, LowerRight);
            Gizmos.DrawLine(LowerRight, UpperRight);
        }

        /// <summary>
        /// Draw debug view in editor
        /// </summary>
        void OnDrawGizmos()
        {
            EditorDraw();
        }

        /// <summary>
        /// Draw advanced debug view in editor when selected
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Matrix4x4 mat = transform.localToWorldMatrix;
            Vector3 right = mat * new Vector4(HalfWidth * 0.75f, 0.0f, 0.0f, 1.0f);
            Vector3 up = mat * new Vector4(0.0f, HalfHeight * 0.75f, 0.0f, 1.0f);
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
        /// Try to save Settings object to path
        /// </summary>
        /// <param name="path">path to save to</param>
        public void TryToSerialize(string path)
        {
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(GetSettings()));
            Debug.Log("Serialized settings to " + new System.IO.FileInfo(path).FullName);
        }

        /// <summary>
        /// Try to load Settings object from path
        /// </summary>
        /// <param name="path">path to load from</param>
        public void TryToDeSerialize(string path)
        {
            SetSettings(JsonUtility.FromJson<Settings>(System.IO.File.ReadAllText(path)));
            Debug.Log("Deserialized settings from " + new System.IO.FileInfo(path).FullName);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor for this display; some options are not always valid so they are disabled in some settings configurations
        /// </summary>
        [CustomEditor(typeof(PhysicalDisplay))]
        public class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                //draw manager               
                SerializedProperty manager = serializedObject.FindProperty(nameof(PhysicalDisplay.manager));

                //cache original manager in case it changes
                PhysicalDisplayManager oldManager = manager.objectReferenceValue as PhysicalDisplayManager;

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(manager);
                bool managerChanged = EditorGUI.EndChangeCheck();

                PhysicalDisplayManager newManager = manager.objectReferenceValue as PhysicalDisplayManager;


                //draw machine name (only if there is no manager)
                SerializedProperty oldMachineName = serializedObject.FindProperty(nameof(PhysicalDisplay.oldMachineName));
                SerializedProperty machineNameAsset = serializedObject.FindProperty(nameof(PhysicalDisplay.machineNameAsset));
                if (!newManager)
                {
                    UniCAVE.MachineName.DrawDeprecatedMachineName(oldMachineName, machineNameAsset, "Machine Name");
                }


                //draw display width, height
                SerializedProperty width = serializedObject.FindProperty(nameof(PhysicalDisplay.width));
                SerializedProperty height = serializedObject.FindProperty(nameof(PhysicalDisplay.height));
                EditorGUILayout.PropertyField(width);
                EditorGUILayout.PropertyField(height);


                //draw head configuration
                SerializedProperty head = serializedObject.FindProperty(nameof(PhysicalDisplay.head));
                EditorGUILayout.PropertyField(head);


                //draw XR camera settings toggle
                SerializedProperty useXRCameras = serializedObject.FindProperty(nameof(PhysicalDisplay.useXRCameras));
                GUIContent useXRCamerasContent = new(text: "Use XR Cameras (Quad Buffer)",
                                                                 tooltip: "Whether the cameras associated with this display should output to an XR device (such as headset or quad-buffered stereo 3D display).\n" +
                                                                          "If you do post processing on the cameras (such as a PhysicalDisplayCalibration) set this to false.\n" +
                                                                          "This is probably also unnecessary if using a Dual-Pipe 3D display.");
                EditorGUILayout.PropertyField(useXRCameras, useXRCamerasContent);


                //draw render texture settings toggle
                SerializedProperty useRenderTextures = serializedObject.FindProperty(nameof(PhysicalDisplay.useRenderTextures));
                GUIContent useRenderTexturesContent = new(text: "Use Render Textures",
                                                                      tooltip: "Render to render textures instead of screen, for post processing");
                EditorGUILayout.PropertyField(useRenderTextures, useRenderTexturesContent);


                //draw render texture settings if render textures are enabled
                SerializedProperty renderTextureSize = serializedObject.FindProperty(nameof(PhysicalDisplay.renderTextureSize));
                if (useRenderTextures.boolValue)
                {
                    EditorGUILayout.PropertyField(renderTextureSize);
                }


                //draw exclusive fullscreen toggle if render textures are disabled and there is no manager
                SerializedProperty exclusiveFullscreen = serializedObject.FindProperty(nameof(PhysicalDisplay.exclusiveFullscreen));
                SerializedProperty display = serializedObject.FindProperty(nameof(PhysicalDisplay.display));
                if (!useRenderTextures.boolValue && !newManager)
                {
                    GUIContent exclusiveFullscreenContent = new(text: "Use Specific Display");
                    EditorGUILayout.PropertyField(exclusiveFullscreen, exclusiveFullscreenContent);

                    //draw display if exclusive fullscreen is enabled
                    if (exclusiveFullscreen.boolValue)
                    {
                        EditorGUILayout.PropertyField(display);
                    }
                }

                //force exclusive fullscreen mode off if there is a manager
                if (newManager && exclusiveFullscreen.boolValue)
                {
                    exclusiveFullscreen.boolValue = false;
                }


                //draw 3D settings
                SerializedProperty is3D = serializedObject.FindProperty(nameof(PhysicalDisplay.is3D));
                SerializedProperty dualPipe = serializedObject.FindProperty(nameof(PhysicalDisplay.dualPipe));
                SerializedProperty dualInstance = serializedObject.FindProperty(nameof(PhysicalDisplay.dualInstance));
                SerializedProperty windowBounds = serializedObject.FindProperty(nameof(PhysicalDisplay.windowBounds));

                EditorGUILayout.PropertyField(is3D);

                if (is3D.boolValue)
                {
                    //draw dual pipe toggle
                    EditorGUILayout.PropertyField(dualPipe);

                    if (dualPipe.boolValue)
                    {
                        //draw dual pipe settings
                        SerializedProperty leftViewport = serializedObject.FindProperty(nameof(PhysicalDisplay.leftViewport));
                        SerializedProperty rightViewport = serializedObject.FindProperty(nameof(PhysicalDisplay.rightViewport));

                        EditorGUILayout.PropertyField(dualInstance);

                        if (!exclusiveFullscreen.boolValue && !dualInstance.boolValue)
                        {
                            //3D, dual pipe, and single instance
                            EditorGUILayout.PropertyField(windowBounds);
                        }

                        EditorGUILayout.PropertyField(leftViewport);
                        EditorGUILayout.PropertyField(rightViewport);
                    }
                    else
                    {
                        //draw window settings
                        EditorGUILayout.PropertyField(windowBounds);

                        if (dualInstance.boolValue) dualInstance.boolValue = false;
                    }
                }
                else
                {
                    //draw window settings
                    EditorGUILayout.PropertyField(windowBounds);

                    if (dualPipe.boolValue) dualPipe.boolValue = false;
                    if (dualInstance.boolValue) dualInstance.boolValue = false;
                }


                //draw settings toggle
                SerializedProperty loadSettingsAtRuntime = serializedObject.FindProperty(nameof(PhysicalDisplay.loadSettingsAtRuntime));
                SerializedProperty serializedLocation = serializedObject.FindProperty(nameof(PhysicalDisplay.serializedLocation));

                EditorGUILayout.PropertyField(loadSettingsAtRuntime);
                if (loadSettingsAtRuntime.boolValue)
                {
                    EditorGUILayout.PropertyField(serializedLocation);
                }

                //done with serialized properties
                serializedObject.ApplyModifiedProperties();

                //update manager if it changed
                PhysicalDisplay physicalDisplay = target as PhysicalDisplay;

                if (managerChanged)
                {
                    PhysicalDisplayManager.SetManager(physicalDisplay, oldManager, newManager);
                }

                //JSON buttons
                if (GUILayout.Button("Export Display Settings to JSON"))
                {
                    string path = EditorUtility.SaveFilePanel("Export Settings", "./", "settings.json", "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        physicalDisplay.TryToSerialize(path);
                    }
                }

                if (GUILayout.Button("Import Display Settings from JSON"))
                {
                    string path = EditorUtility.SaveFilePanel("Import Settings", "./", "settings.json", "json");
                    if (!string.IsNullOrEmpty(path))
                    {
                        physicalDisplay.TryToDeSerialize(path);
                    }
                }
            }
        }
#endif
    }
}