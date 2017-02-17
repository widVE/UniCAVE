using UnityEngine;
using System.Collections;

//This script calculates the off-axis projection matrix for the scene's camera
//written 8/29/16 Brady Boettcher

public class devlabMatrix : MonoBehaviour {

    private Vector3 pa, pb, pc, pe1, pe2, near, far;
    private GameObject camera1;
	private GameObject camera2;
    private Camera cam1;
	private Camera cam2;
    private GameObject plane;

	void Start () {
        camera1 = GameObject.Find("LeftEye");
		camera2 = GameObject.Find("RightEye");

        cam1 = camera1.GetComponent<Camera>();
		cam2 = camera2.GetComponent<Camera>();

        plane = GameObject.FindGameObjectWithTag("plane");
	}
	
	void LateUpdate () {
        //go through each wall and get vertices

            
            Mesh tempMesh = plane.GetComponent<MeshFilter>().mesh;

            pa = plane.transform.TransformPoint(tempMesh.vertices[0]);
            pb = plane.transform.TransformPoint(tempMesh.vertices[2]);
            pc = plane.transform.TransformPoint(tempMesh.vertices[3]);

        //set camera projection matrix
        pe1 = cam1.transform.position;
		pe2 = cam2.transform.position;
        cam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, cam1);
		cam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, cam2);
		camera1.transform.rotation = Quaternion.identity;
		camera2.transform.rotation = Quaternion.identity;
		//camera1.transform.eulerAngles = new Vector3(0, 0, 0);
		//Debug.Log (camera2.transform);
		//Debug.Log(cam2.gameObject.transform);
		//cam1.transform.rotation.eulerAngles = Vector3.zero;
		//cam2.transform.rotation.eulerAngles = Vector3.zero;
	}

    static Matrix4x4 getMatrix(Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pe, Camera theCam)
    {
        Vector3 vu, vr, vn, va, vb, vc;
        float l, r, b, t;

        //compute orthonormal basis for the screen
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
        m[0, 2] = (r + l) / (r - l);
        m[1, 1] = 2.0f * n / (t - b);
        m[1, 2] = (t + b) / (t - b);
        m[2, 2] = (f + n) / (n - f);
        m[2, 3] = 2.0f * f * n / (n - f);
        m[3, 2] = -1.0f; 

        return m;

    }

}
