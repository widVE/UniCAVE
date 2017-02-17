using UnityEngine;
using System.Collections;

public class idahoMatrix : MonoBehaviour
{

    private Vector3 pa, pb, pc, pe1, pe2, near, far;
    private Camera Frontcam1, Leftcam1, Rightcam1, Floorcam1;
    private Camera Frontcam2, Leftcam2, Rightcam2, Floorcam2; 
    private GameObject front, left, right, floor;
    private bool shouldProceed;

    void Start()
    {
        shouldProceed = false;
        Frontcam1 = GameObject.Find("Frontcam").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Leftcam1 = GameObject.Find("Leftcam").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Rightcam1 = GameObject.Find("Rightcam").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Floorcam1 = GameObject.Find("Floorcam").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Frontcam2 = GameObject.Find("Frontcam").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Leftcam2 = GameObject.Find("Leftcam").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Rightcam2 = GameObject.Find("Rightcam").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Floorcam2 = GameObject.Find("Floorcam").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        front = GameObject.Find("front wall");
        left = GameObject.Find("left wall");
        right = GameObject.Find("right wall");
        floor = GameObject.Find("floor");
        shouldProceed = true;
    }

    void LateUpdate()
    {
        if (shouldProceed)
        {
            //go through each wall and get vertices
            //front
            pa = front.transform.TransformPoint(front.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = front.transform.TransformPoint(front.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = front.transform.TransformPoint(front.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Frontcam1.transform.position;
            pe2 = Frontcam2.transform.position;
            Frontcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Frontcam1);
            Frontcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Frontcam2);
            Frontcam1.transform.rotation = Quaternion.identity;
            Frontcam2.transform.rotation = Quaternion.identity;
            //left
            pa = left.transform.TransformPoint(left.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = left.transform.TransformPoint(left.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = left.transform.TransformPoint(left.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Leftcam1.transform.position;
            pe2 = Leftcam2.transform.position;
            Leftcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Leftcam1);
            Leftcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Leftcam2);
            Leftcam1.transform.eulerAngles = new Vector3(0, -90, 0);
            Leftcam2.transform.eulerAngles = new Vector3(0, -90, 0);
            //right
            pa = right.transform.TransformPoint(right.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = right.transform.TransformPoint(right.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = right.transform.TransformPoint(right.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Rightcam1.transform.position;
            pe2 = Rightcam2.transform.position;
            Rightcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Rightcam1);
            Rightcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Rightcam2);
            Rightcam1.transform.eulerAngles = new Vector3(0, 90, 0);
            Rightcam2.transform.eulerAngles = new Vector3(0, 90, 0);
            //floor
            pa = floor.transform.TransformPoint(floor.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = floor.transform.TransformPoint(floor.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = floor.transform.TransformPoint(floor.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Floorcam1.transform.position;
            pe2 = Floorcam2.transform.position;
            Floorcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Floorcam1);
            Floorcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Floorcam2);
            Floorcam1.transform.eulerAngles = new Vector3(90, 0, 0);
            Floorcam2.transform.eulerAngles = new Vector3(90, 0, 0);
        }
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
