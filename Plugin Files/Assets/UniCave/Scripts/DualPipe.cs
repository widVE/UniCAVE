//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher
//Optional class to support dual pipe active stereo configurations

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DualPipe : MonoBehaviour {

    public bool isLeft;
    public int xTranslate = 1920;
    public int yTranslate = 0;
    public int xRes = 1920;
    public int yRes = 1920;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);

    public static void SetPosition(int x, int y, int resX = 1920, int resY = 1920)
    {
        SetWindowPos(FindWindow(null, UnityEngine.Application.productName), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
    }
#endif

    [ContextMenu("Setup Left Build")]
    private void leftBuild()
    {
#if UNITY_EDITOR
        isLeft = true;
        UnityEditor.PlayerSettings.productName = "LeftBuild";
#endif
    }
    [ContextMenu("Setup Right Build")]
    private void rightBuild()
    {
#if UNITY_EDITOR
        isLeft = false;
        UnityEditor.PlayerSettings.productName = "RightBuild";
#endif
    }

    // Use this for initialization
    void Start()
    {
        if (System.Environment.MachineName != MasterTrackingData.HeadNodeMachineName)
        {
            if (isLeft)
            {
                Camera[] camerasToDelete = GameObject.FindObjectsOfType<Camera>();
                foreach (Camera camera in camerasToDelete)
                {
                    if (camera.gameObject.name == "RightEye")
                    {
                        Destroy(camera.gameObject);
                    }
                }
            }
            else
            {
                Camera[] camerasToDelete = GameObject.FindObjectsOfType<Camera>();
                foreach (Camera camera in camerasToDelete)
                {
                    if (camera.gameObject.name == "LeftEye")
                    {
                        Destroy(camera.gameObject);
                    }
                }

                ProjectionPlane[] planeScripts = gameObject.GetComponentsInChildren<ProjectionPlane>();
                foreach (ProjectionPlane p in planeScripts)
                {
                    p.forceRight = true;
                }
#if UNITY_STANDALONE_WIN
                SetPosition(xTranslate, yTranslate, xRes, yRes);
#endif
            }
        }
    }
}
