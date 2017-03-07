﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

public class MasterTrackingData : MonoBehaviour
{
    public string HeadNodeMachineName = "C6_V1_HEAD";
    public bool MultipleDisplays = true;
    public Vector3 TrackingSystemOffset = new Vector3(0.002f, -1.4478f, -0.025f);

    public Vector3 LeftEyeOffset = new Vector3(-0.031f, -0.0266f, 0.041f);
    public Vector3 RightEyeOffset = new Vector3(0.031f, -0.0266f, 0.041f);
    public CameraClearFlags clearFlags = CameraClearFlags.SolidColor;
    public bool leftStereoFirst = true;
    public Color background = Color.black;
    public float fieldOfView = 60f;
    public float nearClipPlane = .02f;
    public float farClipPlane = 100f;
    
    private static MasterTrackingData _instance;

    void Start() 
    {
        
    }

    void Awake()
    {
        _instance = this;
       
        if (MultipleDisplays)
        {
            for (int i = 0; i < Display.displays.Length; i++)
            {
                Display.displays[i].Activate();
            }
        }
    }

    public static MasterTrackingData Instance() 
	{ 
		return _instance;
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