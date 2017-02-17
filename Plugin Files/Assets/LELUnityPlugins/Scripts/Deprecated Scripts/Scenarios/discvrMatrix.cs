using UnityEngine;
using System.Collections;

//This script calculates the off-axis projection matrix for the scene's camera
//written 8/29/16 Brady Boettcher
//Also, I know I hardcoded pretty much all of this for discvr and unrolled almost all of the code
//so if you can shorten it with loops then do that

public class discvrMatrix : MonoBehaviour
{

    private Vector3 pa, pb, pc, pe1, pe2, near, far;
    private Camera Camera1t1, Camera1t2, Camera1b1, Camera1b2;
    private Camera Camera2t1, Camera2t2, Camera2b1, Camera2b2;
    private Camera Camera3t1, Camera3t2, Camera3b1, Camera3b2;
    private Camera Camera4t1, Camera4t2, Camera4b1, Camera4b2;
    private Camera Camera5t1, Camera5t2, Camera5b1, Camera5b2;
    private Camera Camera6t1, Camera6t2, Camera6b1, Camera6b2;
    private Camera Camera7t1, Camera7t2, Camera7b1, Camera7b2;
    private Camera Camera8t1, Camera8t2, Camera8b1, Camera8b2;
    private Camera Camera9t1, Camera9t2, Camera9b1, Camera9b2;
    private Camera Camera10t1, Camera10t2, Camera10b1, Camera10b2;
    private Camera[] cameras;
    private string[] suffixes;
    private GameObject wall1t, wall1b, wall2t, wall2b, wall3t, wall3b, wall4t, wall4b, wall5t, wall5b;
    private GameObject wall6t, wall6b, wall7t, wall7b, wall8t, wall8b, wall9t, wall9b, wall10t, wall10b;
    private bool shouldProceed;

    void Start()
    {
        suffixes = new string[] { "t1", "t2", "b1", "b2", "t", "b"};
        shouldProceed = false;
        initializeCameras();
        initializeWalls();

      /*  //disable all cameras until machine name is checked
        Frontcam1.gameObject.SetActive(false);
        Backcam1.gameObject.SetActive(false);
        Leftcam1.gameObject.SetActive(false);
        Rightcam1.gameObject.SetActive(false);
        Floorcam1.gameObject.SetActive(false);
        Ceilingcam1.gameObject.SetActive(false);
        Frontcam2.gameObject.SetActive(false);
        Backcam2.gameObject.SetActive(false);
        Leftcam2.gameObject.SetActive(false);
        Rightcam2.gameObject.SetActive(false);
        Floorcam2.gameObject.SetActive(false);
        Ceilingcam2.gameObject.SetActive(false); */
        //disableCams();
        shouldProceed = true;
        cameras = new Camera[40];
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i].targetDisplay != 1)
            {
                for (int j = 0; j < Display.displays.Length; ++j)
                {
                    Display.displays[j].Activate();
                }
            }
        }
    }

    void LateUpdate()
    {
        if (shouldProceed)
        {
            //go through each wall and get vertices
       //wall 1
            pa = wall1t.transform.TransformPoint(wall1t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall1t.transform.TransformPoint(wall1t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall1t.transform.TransformPoint(wall1t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera1t1.transform.position;
            pe2 = Camera1t2.transform.position;
            Camera1t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera1t1);
            Camera1t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera1t2);
            Camera1t1.transform.eulerAngles = new Vector3(0, -90, 0);
            Camera1t2.transform.eulerAngles = new Vector3(0, -90, 0);
            pa = wall1b.transform.TransformPoint(wall1b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall1b.transform.TransformPoint(wall1b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall1b.transform.TransformPoint(wall1b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera1b1.transform.position;
            pe2 = Camera1b2.transform.position;
            Camera1b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera1b1);
            Camera1b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera1b2);
            Camera1b1.transform.eulerAngles = new Vector3(0, -90, 0);
            Camera1b2.transform.eulerAngles = new Vector3(0, -90, 0);
       //wall 2
            pa = wall2t.transform.TransformPoint(wall2t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall2t.transform.TransformPoint(wall2t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall2t.transform.TransformPoint(wall2t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera2t1.transform.position;
            pe2 = Camera2t2.transform.position;
            Camera2t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera2t1);
            Camera2t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera2t2);
            Camera2t1.transform.eulerAngles = new Vector3(0, -72, 0);
            Camera2t2.transform.eulerAngles = new Vector3(0, -72, 0);
            pa = wall2b.transform.TransformPoint(wall2b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall2b.transform.TransformPoint(wall2b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall2b.transform.TransformPoint(wall2b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera2b1.transform.position;
            pe2 = Camera2b2.transform.position;
            Camera2b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera2b1);
            Camera2b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera2b2);
            Camera2b1.transform.eulerAngles = new Vector3(0, -72, 0);
            Camera2b2.transform.eulerAngles = new Vector3(0, -72, 0);
       //wall 3
            pa = wall3t.transform.TransformPoint(wall3t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall3t.transform.TransformPoint(wall3t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall3t.transform.TransformPoint(wall3t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera3t1.transform.position;
            pe2 = Camera3t2.transform.position;
            Camera3t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera3t1);
            Camera3t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera3t2);
            Camera3t1.transform.eulerAngles = new Vector3(0, -54, 0);
            Camera3t2.transform.eulerAngles = new Vector3(0, -54, 0);
            pa = wall3b.transform.TransformPoint(wall3b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall3b.transform.TransformPoint(wall3b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall3b.transform.TransformPoint(wall3b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera3b1.transform.position;
            pe2 = Camera3b2.transform.position;
            Camera3b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera3b1);
            Camera3b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera3b2);
            Camera3b1.transform.eulerAngles = new Vector3(0, -54, 0);
            Camera3b2.transform.eulerAngles = new Vector3(0, -54, 0);
       //wall 4
            pa = wall4t.transform.TransformPoint(wall4t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall4t.transform.TransformPoint(wall4t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall4t.transform.TransformPoint(wall4t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera4t1.transform.position;
            pe2 = Camera4t2.transform.position;
            Camera4t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera4t1);
            Camera4t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera4t2);
            Camera4t1.transform.eulerAngles = new Vector3(0, -36, 0);
            Camera4t2.transform.eulerAngles = new Vector3(0, -36, 0);
            pa = wall4b.transform.TransformPoint(wall4b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall4b.transform.TransformPoint(wall4b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall4b.transform.TransformPoint(wall4b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera4b1.transform.position;
            pe2 = Camera4b2.transform.position;
            Camera4b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera4b1);
            Camera4b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera4b2);
            Camera4b1.transform.eulerAngles = new Vector3(0, -36, 0);
            Camera4b2.transform.eulerAngles = new Vector3(0, -36, 0);
       //wall 5
            pa = wall5t.transform.TransformPoint(wall5t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall5t.transform.TransformPoint(wall5t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall5t.transform.TransformPoint(wall5t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera5t1.transform.position;
            pe2 = Camera5t2.transform.position;
            Camera5t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera5t1);
            Camera5t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera5t2);
            Camera5t1.transform.eulerAngles = new Vector3(0, -18, 0);
            Camera5t2.transform.eulerAngles = new Vector3(0, -18, 0);
            pa = wall5b.transform.TransformPoint(wall5b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall5b.transform.TransformPoint(wall5b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall5b.transform.TransformPoint(wall5b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera5b1.transform.position;
            pe2 = Camera5b2.transform.position;
            Camera5b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera5b1);
            Camera5b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera5b2);
            Camera5b1.transform.eulerAngles = new Vector3(0, -18, 0);
            Camera5b2.transform.eulerAngles = new Vector3(0, -18, 0);
       //wall 6
            pa = wall6t.transform.TransformPoint(wall6t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall6t.transform.TransformPoint(wall6t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall6t.transform.TransformPoint(wall6t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera6t1.transform.position;
            pe2 = Camera6t2.transform.position;
            Camera6t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera6t1);
            Camera6t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera6t2);
            Camera6t1.transform.eulerAngles = new Vector3(0, 18, 0);
            Camera6t2.transform.eulerAngles = new Vector3(0, 18, 0);
            pa = wall6b.transform.TransformPoint(wall6b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall6b.transform.TransformPoint(wall6b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall6b.transform.TransformPoint(wall6b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera6b1.transform.position;
            pe2 = Camera6b2.transform.position;
            Camera6b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera6b1);
            Camera6b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera6b2);
            Camera6b1.transform.eulerAngles = new Vector3(0, 18, 0);
            Camera6b2.transform.eulerAngles = new Vector3(0, 18, 0);
       //wall 7
            pa = wall7t.transform.TransformPoint(wall7t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall7t.transform.TransformPoint(wall7t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall7t.transform.TransformPoint(wall7t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera7t1.transform.position;
            pe2 = Camera7t2.transform.position;
            Camera7t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera7t1);
            Camera7t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera7t2);
            Camera7t1.transform.eulerAngles = new Vector3(0, 36, 0);
            Camera7t2.transform.eulerAngles = new Vector3(0, 36, 0);
            pa = wall7b.transform.TransformPoint(wall7b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall7b.transform.TransformPoint(wall7b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall7b.transform.TransformPoint(wall7b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera7b1.transform.position;
            pe2 = Camera7b2.transform.position;
            Camera7b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera7b1);
            Camera7b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera7b2);
            Camera7b1.transform.eulerAngles = new Vector3(0, 36, 0);
            Camera7b2.transform.eulerAngles = new Vector3(0, 36, 0);
       //wall 8
            pa = wall8t.transform.TransformPoint(wall8t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall8t.transform.TransformPoint(wall8t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall8t.transform.TransformPoint(wall8t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera8t1.transform.position;
            pe2 = Camera8t2.transform.position;
            Camera8t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera8t1);
            Camera8t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera8t2);
            Camera8t1.transform.eulerAngles = new Vector3(0, 54, 0);
            Camera8t2.transform.eulerAngles = new Vector3(0, 54, 0);
            pa = wall8b.transform.TransformPoint(wall8b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall8b.transform.TransformPoint(wall8b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall8b.transform.TransformPoint(wall8b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera8b1.transform.position;
            pe2 = Camera8b2.transform.position;
            Camera8b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera8b1);
            Camera8b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera8b2);
            Camera8b1.transform.eulerAngles = new Vector3(0, 54, 0);
            Camera8b2.transform.eulerAngles = new Vector3(0, 54, 0);
       //wall 9
            pa = wall9t.transform.TransformPoint(wall9t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall9t.transform.TransformPoint(wall9t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall9t.transform.TransformPoint(wall9t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera9t1.transform.position;
            pe2 = Camera9t2.transform.position;
            Camera9t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera9t1);
            Camera9t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera9t2);
            Camera9t1.transform.eulerAngles = new Vector3(0, 72, 0);
            Camera9t2.transform.eulerAngles = new Vector3(0, 72, 0);
            pa = wall9b.transform.TransformPoint(wall9b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall9b.transform.TransformPoint(wall9b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall9b.transform.TransformPoint(wall9b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera9b1.transform.position;
            pe2 = Camera9b2.transform.position;
            Camera9b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera9b1);
            Camera9b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera9b2);
            Camera9b1.transform.eulerAngles = new Vector3(0, 72, 0);
            Camera9b2.transform.eulerAngles = new Vector3(0, 72, 0);
       //wall 10
            pa = wall10t.transform.TransformPoint(wall10t.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall10t.transform.TransformPoint(wall10t.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall10t.transform.TransformPoint(wall10t.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera10t1.transform.position;
            pe2 = Camera10t2.transform.position;
            Camera10t1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera10t1);
            Camera10t2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera10t2);
            Camera10t1.transform.eulerAngles = new Vector3(0, 90, 0);
            Camera10t2.transform.eulerAngles = new Vector3(0, 90, 0);
            pa = wall10b.transform.TransformPoint(wall10b.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = wall10b.transform.TransformPoint(wall10b.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = wall10b.transform.TransformPoint(wall10b.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = Camera10b1.transform.position;
            pe2 = Camera10b2.transform.position;
            Camera10b1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Camera10b1);
            Camera10b2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Camera10b2);
            Camera10b1.transform.eulerAngles = new Vector3(0, 90, 0);
            Camera10b2.transform.eulerAngles = new Vector3(0, 90, 0);
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

    void initializeCameras()
    {
        Camera1t1 = GameObject.Find("Camera1").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera1t2 = GameObject.Find("Camera1").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera1b1 = GameObject.Find("Camera1").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera1b2 = GameObject.Find("Camera1").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
        Camera2t1 = GameObject.Find("Camera2").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera2t2 = GameObject.Find("Camera2").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera2b1 = GameObject.Find("Camera2").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera2b2 = GameObject.Find("Camera2").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
        Camera3t1 = GameObject.Find("Camera3").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera3t2 = GameObject.Find("Camera3").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera3b1 = GameObject.Find("Camera3").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera3b2 = GameObject.Find("Camera3").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
        Camera4t1 = GameObject.Find("Camera4").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera4t2 = GameObject.Find("Camera4").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera4b1 = GameObject.Find("Camera4").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera4b2 = GameObject.Find("Camera4").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
        Camera5t1 = GameObject.Find("Camera5").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera5t2 = GameObject.Find("Camera5").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera5b1 = GameObject.Find("Camera5").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera5b2 = GameObject.Find("Camera5").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
        Camera6t1 = GameObject.Find("Camera6").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera6t2 = GameObject.Find("Camera6").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera6b1 = GameObject.Find("Camera6").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera6b2 = GameObject.Find("Camera6").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
        Camera7t1 = GameObject.Find("Camera7").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera7t2 = GameObject.Find("Camera7").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera7b1 = GameObject.Find("Camera7").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera7b2 = GameObject.Find("Camera7").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
        Camera8t1 = GameObject.Find("Camera8").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera8t2 = GameObject.Find("Camera8").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera8b1 = GameObject.Find("Camera8").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera8b2 = GameObject.Find("Camera8").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
        Camera9t1 = GameObject.Find("Camera9").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera9t2 = GameObject.Find("Camera9").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera9b1 = GameObject.Find("Camera9").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera9b2 = GameObject.Find("Camera9").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
        Camera10t1 = GameObject.Find("Camera10").transform.GetChild(0).transform.GetChild(0).GetComponent<Camera>();
        Camera10t2 = GameObject.Find("Camera10").transform.GetChild(0).transform.GetChild(1).GetComponent<Camera>();
        Camera10b1 = GameObject.Find("Camera10").transform.GetChild(1).transform.GetChild(0).GetComponent<Camera>();
        Camera10b2 = GameObject.Find("Camera10").transform.GetChild(1).transform.GetChild(1).GetComponent<Camera>();
    }

    void initializeWalls()
    {
        wall1t = GameObject.Find("wall 1").transform.GetChild(0).gameObject;
        wall1b = GameObject.Find("wall 1").transform.GetChild(1).gameObject;
        wall2t = GameObject.Find("wall 2").transform.GetChild(0).gameObject;
        wall2b = GameObject.Find("wall 2").transform.GetChild(1).gameObject;
        wall3t = GameObject.Find("wall 3").transform.GetChild(0).gameObject;
        wall3b = GameObject.Find("wall 3").transform.GetChild(1).gameObject;
        wall4t = GameObject.Find("wall 4").transform.GetChild(0).gameObject;
        wall4b = GameObject.Find("wall 4").transform.GetChild(1).gameObject;
        wall5t = GameObject.Find("wall 5").transform.GetChild(0).gameObject;
        wall5b = GameObject.Find("wall 5").transform.GetChild(1).gameObject;
        wall6t = GameObject.Find("wall 6").transform.GetChild(0).gameObject;
        wall6b = GameObject.Find("wall 6").transform.GetChild(1).gameObject;
        wall7t = GameObject.Find("wall 7").transform.GetChild(0).gameObject;
        wall7b = GameObject.Find("wall 7").transform.GetChild(1).gameObject;
        wall8t = GameObject.Find("wall 8").transform.GetChild(0).gameObject;
        wall8b = GameObject.Find("wall 8").transform.GetChild(1).gameObject;
        wall9t = GameObject.Find("wall 9").transform.GetChild(0).gameObject;
        wall9b = GameObject.Find("wall 9").transform.GetChild(1).gameObject;
        wall10t = GameObject.Find("wall 10").transform.GetChild(0).gameObject;
        wall10b = GameObject.Find("wall 10").transform.GetChild(1).gameObject;
    }

/*    void disableCams()
    {
        string machineName = System.Environment.MachineName;
        Debug.Log(machineName);
        switch (machineName)
        {
            case "C6_V1_HEAD":
                Frontcam1.gameObject.SetActive(true);
                break;
            case "C6_V1_FLOORCEIL":
                Floorcam1.gameObject.SetActive(true);
                Floorcam2.gameObject.SetActive(true);
                break;
            case "C6_V2_FLOORCEIL":
                Ceilingcam1.gameObject.SetActive(true);
                Ceilingcam2.gameObject.SetActive(true);
                break;
            case "C6_V2_DOORLEFT":
                Leftcam1.gameObject.SetActive(true);
                Leftcam2.gameObject.SetActive(true);
                break;
            case "C6_V1_DOORLEFT":
                Backcam1.gameObject.SetActive(true);
                Backcam2.gameObject.SetActive(true);
                break;
            case "C6_V1_FRONTRIGH":
                Frontcam1.gameObject.SetActive(true);
                Frontcam2.gameObject.SetActive(true);
                break;
            case "C6_V2_FRONTRIGH":
                Rightcam1.gameObject.SetActive(true);
                Rightcam2.gameObject.SetActive(true);
                break;

            default:
                Debug.Log("something went wrong when disabling cameras");

                break;
        }
    } */

}
