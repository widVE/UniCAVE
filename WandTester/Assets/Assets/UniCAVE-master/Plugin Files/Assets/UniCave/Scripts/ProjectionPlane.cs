//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
//Brady Boettcher
//Living Environments Laboratory
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ProjectionPlane : MonoBehaviour {

    public GameObject plane;
    public GameObject trackedRotation;
    public bool forceRight = false;

    public string MachineName;

    private Camera leftEye=null;
    private Camera rightEye=null;
    private Vector3 pa, pb, pc;
    
	// Use this for initialization
	void Start () {
        string mn = System.Environment.MachineName;
        
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
#endif
        {
            if (mn != MachineName)
            {
                //remove extra cameras in case when machine shouldn't be using them..
                Transform lc = transform.GetChild(0);
                lc.parent = null;
                Destroy(lc.gameObject);
                if (transform.childCount > 0)
                {
                    Transform rc = transform.GetChild(0);
                    rc.parent = null;
                    Destroy(rc.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
	}
	
	// Update is called once per frame
	void LateUpdate () {

        string mn = System.Environment.MachineName;
#if UNITY_EDITOR
        if (mn == MachineName || UnityEditor.EditorApplication.isPlaying)
#else
        if (mn == MachineName)
#endif
        {
            if (plane != null)
            {
                //incorrect to assume that leftEye is always the first child now...
                if (forceRight)
                {
                    rightEye = transform.GetChild(0).GetComponent<Camera>();
                }
                else
                {
                    leftEye = transform.GetChild(0).GetComponent<Camera>();
                }

                if (transform.childCount > 1)
                {
                    rightEye = transform.GetChild(1).GetComponent<Camera>();
                }
            }

            Quaternion trackerRotation = Quaternion.identity;
            if (trackedRotation != null)
            {
                trackerRotation = trackedRotation.transform.rotation;
            }

            Vector3 trackedHead = transform.parent.position;

            pa = plane.transform.TransformPoint(plane.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = plane.transform.TransformPoint(plane.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = plane.transform.TransformPoint(plane.GetComponent<MeshFilter>().mesh.vertices[3]);

            transform.rotation = plane.transform.rotation;

            if (leftEye != null)
            {
                //set camera projection matrix            
                Vector3 eyePos = trackedHead + (trackerRotation * MasterTrackingData.LeftEyeOffset);
                //Debug.LogError("Left Eye Proj: " + eyePos.ToString("F4"));
                leftEye.transform.position = eyePos;
                leftEye.projectionMatrix = getAsymProjMatrix(pa, pb, pc, eyePos, leftEye);
                leftEye.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, leftEye.projectionMatrix);
            }

            if (rightEye != null)
            {
                //set camera projection matrix        
                Vector3 eyePos = trackedHead + (trackerRotation * MasterTrackingData.RightEyeOffset);
                //Debug.LogError("Right Eye Proj: " + eyePos.ToString("F4"));
                rightEye.transform.position = eyePos;
                rightEye.projectionMatrix = getAsymProjMatrix(pa, pb, pc, eyePos, rightEye);
                rightEye.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, rightEye.projectionMatrix);
            }
        }
    }

    static Matrix4x4 getAsymProjMatrix(Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pe, Camera theCam)
    {
        Vector3 vu, vr, vn, va, vb, vc;
        float l, r, b, t;

        //compute orthonormal basis for the screen - could pre-compute this...
        vr = (pb - pa).normalized;
        vu = (pc - pa).normalized;
        vn = Vector3.Cross(vr, vu).normalized;

        //compute screen corner vectors
        va = pa - pe;
        vb = pb - pe;
        vc = pc - pe;

        //find the distance from the eye to screen plane
        float n = theCam.nearClipPlane;
        float f = theCam.farClipPlane;
        float d = Vector3.Dot(va, vn); // distance from eye to screen
        l = Vector3.Dot(vr, va) * n / d;
        r = Vector3.Dot(vr, vb) * n / d;
        b = Vector3.Dot(vu, va) * n / d;
        t = Vector3.Dot(vu, vc) * n / d;

        //put together the matrix - bout time amirite?
        Matrix4x4 m = new Matrix4x4();

        //from http://forum.unity3d.com/threads/using-projection-matrix-to-create-holographic-effect.291123/
        m[0, 0] = 2.0f * n / (r - l);
        m[0, 1] = 0;
        m[0, 2] = (r + l) / (r - l);
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = 2.0f * n / (t - b);
        m[1, 2] = (t + b) / (t - b);
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = -(f + n) / (f - n);
        m[2, 3] = (-2.0f * f * n) / (f - n);
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = -1.0f;
        m[3, 3] = 0;

        return m;
    } 
}
