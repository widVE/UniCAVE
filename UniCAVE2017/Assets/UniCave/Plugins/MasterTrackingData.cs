using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

public class MasterTrackingData : MonoBehaviour
{
    public static string HeadNodeMachineName = "C6_V1_HEAD";
    public static bool MultipleDisplays = true;
    public static Vector3 TrackingSystemOffset = new Vector3(0.002f, -1.4478f, -0.025f);

    public static Vector3 LeftEyeOffset = new Vector3(-0.031f, -0.0266f, 0.041f);
    public static Vector3 RightEyeOffset = new Vector3(0.031f, -0.0266f, 0.041f);
    public static CameraClearFlags clearFlags = CameraClearFlags.SolidColor;
    public static bool leftStereoFirst = true;
    public static Color background = Color.black;
    public static float fieldOfView = 60f;
    public static float nearClipPlane = .02f;
    public static float farClipPlane = 100f;

    //public variables you'll see in the UI
    public string _HeadNodeMachineName = "C6_V1_HEAD";
    public bool _MultipleDisplays = true;
    public Vector3 _TrackingSystemOffset = new Vector3(0.002f, -1.4478f, -0.025f);

    public Vector3 _LeftEyeOffset = new Vector3(-0.031f, -0.0266f, 0.041f);
    public Vector3 _RightEyeOffset = new Vector3(0.031f, -0.0266f, 0.041f);
    public CameraClearFlags _clearFlags = CameraClearFlags.SolidColor;
    public bool _leftStereoFirst = true;
    public Color _background = Color.black;
    public float _fieldOfView = 60f;
    public float _nearClipPlane = .02f;
    public float _farClipPlane = 100f;
    
    void Start() 
    {
        
    }

    void Awake()
    {
        HeadNodeMachineName = _HeadNodeMachineName;
        MultipleDisplays = _MultipleDisplays;
        TrackingSystemOffset = _TrackingSystemOffset;

        LeftEyeOffset = _LeftEyeOffset;
        RightEyeOffset = _RightEyeOffset;
        clearFlags = _clearFlags;
        leftStereoFirst = _leftStereoFirst;
        background = _background;
        fieldOfView = _fieldOfView;
        nearClipPlane = _nearClipPlane;
        farClipPlane = _farClipPlane;

        if (MultipleDisplays)
        {
            for (int i = 0; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
        }
    }

    [ContextMenu("Sync Camera Settings")]
    void syncCameraSettings()
    {   //syncs all child cameras to the settings specified
        Camera[] childCameras = GetComponentsInChildren<Camera>();
        foreach (Camera c in childCameras)
        {
            if (c.name.Contains("Left"))
            {
                c.transform.localPosition = LeftEyeOffset;
                if (leftStereoFirst) c.depth = -1f;
                else c.depth = 0f;
            }
            else if (c.name.Contains("Right"))
            {
                c.transform.localPosition = RightEyeOffset;
                if (leftStereoFirst) c.depth = 0f;
                else c.depth = -1f;
            }
            c.clearFlags = clearFlags;
            c.backgroundColor = background;
            c.fieldOfView = fieldOfView;
            c.nearClipPlane = nearClipPlane;
            c.farClipPlane = farClipPlane;
        }
    }
}
