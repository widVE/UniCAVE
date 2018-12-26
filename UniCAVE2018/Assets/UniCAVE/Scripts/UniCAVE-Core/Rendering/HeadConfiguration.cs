using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.InteropServices;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

[Serializable]
public class HeadConfiguration : MonoBehaviour {

    public static HeadConfiguration globalHead = null;

    public Vector3 leftEyeOffset;
    public Vector3 centerEyeOffset { get { return (leftEyeOffset + rightEyeOffset) * 0.5f; } }
    public Vector3 rightEyeOffset;

    public float nearClippingPlane = 0.01f, farClippingPlane = 100.0f;
    
    //creates an eye as a child and sets its properties
    public Camera CreateLeftEye(string name) {
        GameObject obj = new GameObject("Left Eye For: " + name);
        Camera res = obj.AddComponent<Camera>();
        res.nearClipPlane = nearClippingPlane;
        res.transform.parent = transform;
        res.transform.localPosition = leftEyeOffset;
        return res;
    }
    public Camera CreateCenterEye(string name) {
        GameObject obj = new GameObject("Center Eye For: " + name);
        Camera res = obj.AddComponent<Camera>();
        res.nearClipPlane = nearClippingPlane;
        res.transform.parent = transform;
        res.transform.localPosition = centerEyeOffset;
        return res;
    }
    public Camera CreateRightEye(string name) {
        GameObject obj = new GameObject("Right Eye For: " + name);
        Camera res = obj.AddComponent<Camera>();
        res.nearClipPlane = nearClippingPlane;
        res.transform.parent = transform;
        res.transform.localPosition = rightEyeOffset;
        return res;
    }

    public void Start() {
        globalHead = this;
    }

#if UNITY_EDITOR

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.localToWorldMatrix * new Vector4(leftEyeOffset.x, leftEyeOffset.y, leftEyeOffset.z, 1.0f), 0.01f);
        Gizmos.DrawWireSphere(transform.localToWorldMatrix * new Vector4(centerEyeOffset.x, centerEyeOffset.y, centerEyeOffset.z, 1.0f), 0.01f);
        Gizmos.DrawWireSphere(transform.localToWorldMatrix * new Vector4(rightEyeOffset.x, rightEyeOffset.y, rightEyeOffset.z, 1.0f), 0.01f);
        Gizmos.DrawWireSphere(transform.position, Mathf.Max(leftEyeOffset.magnitude, rightEyeOffset.magnitude) + 0.02f);
    }
    #endif
}
