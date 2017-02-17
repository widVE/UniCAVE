using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class dualView : MonoBehaviour {

    public bool isLeft;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR
    [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
    public static extern bool SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

    [DllImport("user32.dll", EntryPoint = "FindWindow")]
    public static extern IntPtr FindWindow(System.String className, System.String windowName);

    public static void SetPosition(int x, int y, int resX = 1920, int resY = 1920)
    {
        SetWindowPos(FindWindow(null, "Nightmares"), 0, x, y, resX, resY, resX * resY == 0 ? 1 : 0);
    }
#endif

    void Update()
    {
        if (System.Environment.MachineName == MasterTrackingData.Instance().HeadNode)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GetComponent<NetworkView>().RPC("quitApplication", RPCMode.Others);
                Application.Quit();
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        if(System.Environment.MachineName != MasterTrackingData.Instance().HeadNode) {
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
				
				 SetPosition(1920, 0);
            }
        }
    }

    [RPC]
    void quitApplication()
    {
        Application.Quit();
    }
}
