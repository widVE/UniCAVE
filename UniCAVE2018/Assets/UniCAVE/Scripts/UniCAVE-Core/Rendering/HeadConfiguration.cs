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
using System;
using System.Runtime.InteropServices;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

/// <summary>
/// Class responsible for creating the cameras necessary to render for this machine's displays
/// Cameras are created as children of this object, so movement and rotation correctly positions cameras
/// Head and eyes should correspond to the viewer's real head and eyes
/// </summary>
[Serializable]
public class HeadConfiguration : MonoBehaviour {


    /// <summary>
    /// Prefab to create, must contain a Camera component
    /// </summary>
    [Tooltip("Prefab object with camera component")]
    public GameObject camPrefab = null;

    public Vector3 leftEyeOffset;
    public Vector3 centerEyeOffset { get { return (leftEyeOffset + rightEyeOffset) * 0.5f; } }
    public Vector3 rightEyeOffset;

    public float nearClippingPlane = 0.01f, farClippingPlane = 100.0f;
    
    /// <summary>
    /// Create left eye if it doesn't already exist
    /// </summary>
    /// <param name="name">name of display that the camera is for</param>
    /// <returns>The left eye camera</returns>
    public Camera CreateLeftEye(string name) {
        GameObject obj;
        Camera res;
        if (camPrefab != null) {
            obj = Instantiate(camPrefab);
            res = obj.GetComponent<Camera>();
        } else {
            obj = new GameObject();
            res = obj.AddComponent<Camera>();
        }
        obj.name = "Left Eye For: " + name;
        res.nearClipPlane = nearClippingPlane;
        res.farClipPlane = farClippingPlane;
        res.transform.parent = transform;
        res.transform.localPosition = leftEyeOffset;
        return res;
    }

    /// <summary>
    /// Create center eye if it doesn't already exist
    /// </summary>
    /// <param name="name">name of display that the camera is for</param>
    /// <returns>The center eye camera</returns>
    public Camera CreateCenterEye(string name) {
        GameObject obj;
        Camera res;
        if (camPrefab != null) {
            obj = Instantiate(camPrefab);
            res = obj.GetComponent<Camera>();
        } else {
            obj = new GameObject();
            res = obj.AddComponent<Camera>();
        }
        obj.name = "Center Eye For: " + name;
        res.nearClipPlane = nearClippingPlane;
        res.farClipPlane = farClippingPlane;
        res.transform.parent = transform;
        res.transform.localPosition = centerEyeOffset;
        return res;
    }

    /// <summary>
    /// Create right eye if it doesn't already exist
    /// </summary>
    /// <param name="name">name of display that the camera is for</param>
    /// <returns>The right eye camera</returns>
    public Camera CreateRightEye(string name) {
        GameObject obj;
        Camera res;
        if (camPrefab != null) {
            obj = Instantiate(camPrefab);
            res = obj.GetComponent<Camera>();
        } else {
            obj = new GameObject();
            res = obj.AddComponent<Camera>();
        }
        obj.name = "Right Eye For: " + name;
        res.nearClipPlane = nearClippingPlane;
        res.farClipPlane = farClippingPlane;
        res.transform.parent = transform;
        res.transform.localPosition = rightEyeOffset;
        return res;
    }

    #if UNITY_EDITOR
    
    /// <summary>
    /// Draw a sphere at the position of the head, and smaller spheres for each eye (including center eye)
    /// </summary>
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.localToWorldMatrix * new Vector4(leftEyeOffset.x, leftEyeOffset.y, leftEyeOffset.z, 1.0f), 0.01f);
        Gizmos.DrawWireSphere(transform.localToWorldMatrix * new Vector4(centerEyeOffset.x, centerEyeOffset.y, centerEyeOffset.z, 1.0f), 0.01f);
        Gizmos.DrawWireSphere(transform.localToWorldMatrix * new Vector4(rightEyeOffset.x, rightEyeOffset.y, rightEyeOffset.z, 1.0f), 0.01f);
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(leftEyeOffset.magnitude, rightEyeOffset.magnitude) + 0.02f);
    }
    #endif
}
