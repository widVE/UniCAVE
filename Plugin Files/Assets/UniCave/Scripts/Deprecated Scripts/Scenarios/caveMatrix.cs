using UnityEngine;
using System.Collections;

//This script calculates the off-axis projection matrix for the scene's camera
//written 8/29/16 Brady Boettcher
//Also, I know I hardcoded pretty much all of this for the cave and unrolled almost all of the code
//so if you can shorten it with loops then do that

public class caveMatrix : MonoBehaviour
{

    private Vector3 pa, pb, pc, pe1, pe2, near, far;
    private Camera Frontcam1, Backcam1, Leftcam1, Rightcam1, Floorcam1, Ceilingcam1;
    private Camera Frontcam2, Backcam2, Leftcam2, Rightcam2, Floorcam2, Ceilingcam2;
    private GameObject fc, bc, lc, rc, flc, cc;
    private GameObject front, back, left, right, floor, ceiling;
    private bool shouldProceed;
    public GameObject trackedRotation;

    void Start()
    {
        shouldProceed = false;
        fc = GameObject.Find("Frontcam");
        bc = GameObject.Find("Backcam");
        lc = GameObject.Find("Leftcam");
        rc = GameObject.Find("Rightcam");
        flc = GameObject.Find("Floorcam");
        cc = GameObject.Find("Ceilingcam");

        Frontcam1 = fc.transform.GetChild(0).GetComponent<Camera>();
        Backcam1 = bc.transform.GetChild(0).GetComponent<Camera>();
        Leftcam1 = lc.transform.GetChild(0).GetComponent<Camera>();
        Rightcam1 = rc.transform.GetChild(0).GetComponent<Camera>();
        Floorcam1 = flc.transform.GetChild(0).GetComponent<Camera>();
        Ceilingcam1 = cc.transform.GetChild(0).GetComponent<Camera>();
        Frontcam2 = fc.transform.GetChild(1).GetComponent<Camera>();
        Backcam2 = bc.transform.GetChild(1).GetComponent<Camera>();
        Leftcam2 = lc.transform.GetChild(1).GetComponent<Camera>();
        Rightcam2 = rc.transform.GetChild(1).GetComponent<Camera>();
        Floorcam2 = flc.transform.GetChild(1).GetComponent<Camera>();
        Ceilingcam2 = cc.transform.GetChild(1).GetComponent<Camera>();

        front = GameObject.Find("front wall");
        back = GameObject.Find("back wall");
        left = GameObject.Find("left wall");
        right = GameObject.Find("right wall");
        floor = GameObject.Find("floor");
        ceiling = GameObject.Find("ceiling");
        //disable all cameras until machine name is checked
        fc.SetActive(false);
        bc.SetActive(false);
        lc.SetActive(false);
        rc.SetActive(false);
        flc.SetActive(false);
        cc.SetActive(false);
        Frontcam1.gameObject.SetActive(false);
        Frontcam2.gameObject.SetActive(false);
        Backcam1.gameObject.SetActive(false);
        Backcam2.gameObject.SetActive(false);
        Leftcam1.gameObject.SetActive(false);
        Leftcam2.gameObject.SetActive(false);
        Rightcam1.gameObject.SetActive(false);
        Rightcam2.gameObject.SetActive(false);
        Floorcam1.gameObject.SetActive(false);
        Floorcam2.gameObject.SetActive(false);
        Ceilingcam1.gameObject.SetActive(false);
        Ceilingcam2.gameObject.SetActive(false);

        enableCams();
        shouldProceed = true;
    }

    void LateUpdate()
    {
        //Debug.Log(Frontcam1.transform.position.y);
        if (shouldProceed)
        {
            Quaternion q = trackedRotation.transform.rotation;
            //Vector3 trackedHead = Frontcam1.transform.parent.transform.parent.transform.parent.localPosition;

            /*if (eyePair != null)
            {
                Vector3 planeCenter = transform.position;
                wallSpacePosition = (eyePair.transform.GetChild(0).transform.rotation * trackedHead) - planeCenter;
            }*/

            //go through each wall and get vertices
            //front
            //Vector3 frontCenter = front.transform.position;
            //Vector3 frontWallSpace = (Frontcam1.transform.parent.rotation * trackedHead) - frontCenter;
            //print(trackedHead);
            pa = front.transform.TransformPoint(front.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = front.transform.TransformPoint(front.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = front.transform.TransformPoint(front.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = fc.transform.position + (q * Frontcam1.transform.localPosition);
            pe2 = fc.transform.position + (q * Frontcam2.transform.localPosition);
            //print(pe1);
            //print(pe2);
            Frontcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Frontcam1);
            Frontcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Frontcam2);
            //Frontcam1.transform.position = pe1;
            //Frontcam2.transform.position = pe2;

            /*Debug.LogError("Front Projection Left: " + pe1);
            Debug.LogError("Front Projection Right: " + pe2);
            Debug.LogError("Front View Left: " + Frontcam1.transform.position);
            Debug.LogError("Front View Right: " + Frontcam2.transform.position);*/

            //Frontcam1.transform.rotation = Quaternion.identity;
            //Frontcam2.transform.rotation = Quaternion.identity;
            //Frontcam1.transform.rotation = Frontcam1.transform.rotation * transform.parent.rotation;
            //Frontcam2.transform.rotation = Frontcam2.transform.rotation * transform.parent.rotation;

            //back

            //trackedHead = Backcam1.transform.parent.transform.parent.transform.parent.localPosition;
            //print(trackedHead);
            //Vector3 backCenter = back.transform.position;
            //Vector3 backWallSpace = (Backcam1.transform.parent.rotation * trackedHead) - backCenter;
            pa = back.transform.TransformPoint(back.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = back.transform.TransformPoint(back.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = back.transform.TransformPoint(back.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = bc.transform.position + (q * Backcam1.transform.localPosition);
            pe2 = bc.transform.position + (q * Backcam2.transform.localPosition);
            //print(pe1);
            //print(pe2);
            Backcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Backcam1);
            Backcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Backcam2);
            //Backcam1.transform.position = pe1;
            //Backcam2.transform.position = pe2;

            /*Debug.LogError("Back Projection Left: " + pe1);
            Debug.LogError("Back Projection Right: " + pe2);
            Debug.LogError("Back View Left: " + Backcam1.transform.position);
            Debug.LogError("Back View Right: " + Backcam2.transform.position);*/

            //Backcam1.transform.eulerAngles = new Vector3(0, -180, 0);
            //Backcam2.transform.eulerAngles = new Vector3(0, -180, 0);
            //Backcam1.transform.rotation = Backcam1.transform.rotation * transform.parent.rotation;
            //Backcam2.transform.rotation = Backcam1.transform.rotation * transform.parent.rotation;

            //left
            //trackedHead = Leftcam1.transform.parent.transform.parent.transform.parent.localPosition;
            //print(trackedHead);
            //Vector3 leftCenter = left.transform.position;
            //Vector3 leftWallSpace = (Leftcam1.transform.parent.rotation * trackedHead) - leftCenter;
            pa = left.transform.TransformPoint(left.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = left.transform.TransformPoint(left.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = left.transform.TransformPoint(left.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = lc.transform.position + (q * Leftcam1.transform.localPosition);
            pe2 = lc.transform.position + (q * Leftcam2.transform.localPosition);
            //print(pe1);
            //print(pe2);
            Leftcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Leftcam1);
            Leftcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Leftcam2);
            //Leftcam1.transform.position = pe1;
            //Leftcam2.transform.position = pe2;

            /*Debug.LogError("Left Projection Left: " + pe1);
            Debug.LogError("Left Projection Right: " + pe2);
            Debug.LogError("Left View Left: " + Leftcam1.transform.position);
            Debug.LogError("Left View Right: " + Leftcam2.transform.position);*/

            //Leftcam1.transform.eulerAngles = new Vector3(0, -90, 0);
            //Leftcam2.transform.eulerAngles = new Vector3(0, -90, 0);
            //Leftcam1.transform.rotation = Leftcam1.transform.rotation * transform.parent.rotation;
            //Leftcam2.transform.rotation = Leftcam2.transform.rotation * transform.parent.rotation;

            
            //right
            //trackedHead = Rightcam1.transform.parent.transform.parent.transform.parent.localPosition;
            //print(trackedHead);
            //Vector3 rightCenter = right.transform.position;
            //Vector3 rightWallSpace = (Rightcam1.transform.parent.rotation * trackedHead) - rightCenter;
            pa = right.transform.TransformPoint(right.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = right.transform.TransformPoint(right.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = right.transform.TransformPoint(right.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = rc.transform.position + (q * Rightcam1.transform.localPosition);
            pe2 = rc.transform.position + (q * Rightcam2.transform.localPosition);
            //print(pe1);
            //print(pe2);
            Rightcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Rightcam1);
            Rightcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Rightcam2);
            //Rightcam1.transform.position = pe1;
            //Rightcam2.transform.position = pe2;

            //Rightcam1.transform.eulerAngles = new Vector3(0, 90, 0);
            //Rightcam2.transform.eulerAngles = new Vector3(0, 90, 0);
            //Rightcam1.transform.rotation = Rightcam1.transform.rotation * transform.parent.rotation;
            //Rightcam2.transform.rotation = Rightcam2.transform.rotation * transform.parent.rotation;
            /*Debug.LogError("Right Projection Left: " + pe1);
            Debug.LogError("Right Projection Right: " + pe2);
            Debug.LogError("Right View Left: " + Rightcam1.transform.position);
            Debug.LogError("Right View Right: " + Rightcam2.transform.position);*/

            //floor
            //trackedHead = Floorcam1.transform.parent.transform.parent.transform.parent.localPosition;
            //print(trackedHead);
            //Vector3 floorCenter = floor.transform.position;
            //Vector3 floorWallSpace = (Floorcam1.transform.parent.rotation * trackedHead) - floorCenter;
            pa = floor.transform.TransformPoint(floor.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = floor.transform.TransformPoint(floor.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = floor.transform.TransformPoint(floor.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = flc.transform.position + (q * Floorcam1.transform.localPosition);
            pe2 = flc.transform.position + (q * Floorcam2.transform.localPosition);
            //print(pe1);
            //print(pe2);
            Floorcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Floorcam1);
            Floorcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Floorcam2);
            //Floorcam1.transform.position = pe1;
            //Floorcam2.transform.position = pe2;
            /*Debug.LogError("Floor Projection Left: " + pe1);
            Debug.LogError("Floor Projection Right: " + pe2);
            Debug.LogError("Floor View Left: " + Floorcam1.transform.position);
            Debug.LogError("Floor View Right: " + Floorcam2.transform.position);*/
            //Floorcam1.transform.eulerAngles = new Vector3(90, -90, 0);
            //Floorcam2.transform.eulerAngles = new Vector3(90, -90, 0);

            //Floorcam1.transform.rotation = Floorcam1.transform.rotation * transform.parent.rotation;
            //Floorcam2.transform.rotation = Floorcam2.transform.rotation * transform.parent.rotation;
            
            //ceiling
            //trackedHead = Ceilingcam1.transform.parent.transform.parent.transform.parent.localPosition;
            //print(trackedHead);
            //Vector3 ceilingCenter = ceiling.transform.position;
            //Vector3 ceilingWallSpace = (Ceilingcam1.transform.parent.rotation * trackedHead) - ceilingCenter;
            pa = ceiling.transform.TransformPoint(ceiling.GetComponent<MeshFilter>().mesh.vertices[0]);
            pb = ceiling.transform.TransformPoint(ceiling.GetComponent<MeshFilter>().mesh.vertices[2]);
            pc = ceiling.transform.TransformPoint(ceiling.GetComponent<MeshFilter>().mesh.vertices[3]);
            //set camera projection matrix
            pe1 = cc.transform.position + (q * Ceilingcam1.transform.localPosition);
            pe2 = cc.transform.position + (q * Ceilingcam2.transform.localPosition);
            /*Debug.LogError("Ceiling Projection Left: " + pe1);
            Debug.LogError("Ceiling Projection Right: " + pe2);
            Debug.LogError("Ceiling View Left: " + Ceilingcam1.transform.position);
            Debug.LogError("Ceiling View Right: " + Ceilingcam2.transform.position);*/
            //print(pe1);
            //print(pe2);
            Ceilingcam1.projectionMatrix = getMatrix(pa, pb, pc, pe1, Ceilingcam1);
            Ceilingcam2.projectionMatrix = getMatrix(pa, pb, pc, pe2, Ceilingcam2);
            //Ceilingcam1.transform.position = pe1;
            //Ceilingcam2.transform.position = pe2;
			//Ceilingcam1.transform.rotation = Quaternion.identity;
            //Ceilingcam2.transform.rotation = Quaternion.identity;
			//Ceilingcam1.transform.eulerAngles = new Vector3(-90, -90, 0);
            //Ceilingcam2.transform.eulerAngles = new Vector3(-90, -90, 0);
            //Ceilingcam1.transform.rotation = Ceilingcam1.transform.rotation * transform.parent.rotation;
            //Ceilingcam2.transform.rotation = Ceilingcam2.transform.rotation * transform.parent.rotation;
          
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
    
    void enableCams()
    {
        string machineName = System.Environment.MachineName;
        Debug.Log(machineName);
        switch (machineName)
        {
            case "C6_V1_HEAD":
                fc.SetActive(true);
                Frontcam1.gameObject.SetActive(true);
                break;
            case "C6_V1_FLOORCEIL":
                flc.SetActive(true);
                Floorcam1.gameObject.SetActive(true);
                Floorcam2.gameObject.SetActive(true);
                break;
            case "C6_V2_FLOORCEIL":
                cc.SetActive(true);
                Ceilingcam1.gameObject.SetActive(true);
                Ceilingcam2.gameObject.SetActive(true);
                break;
            case "C6_V2_DOORLEFT":
                lc.SetActive(true);
                Leftcam1.gameObject.SetActive(true);
                Leftcam2.gameObject.SetActive(true);
                break;
            case "C6_V1_DOORLEFT":
                bc.SetActive(true);
                Backcam1.gameObject.SetActive(true);
                Backcam2.gameObject.SetActive(true);
                break;
            case "C6_V1_FRONTRIGH":
                fc.SetActive(true);
                Frontcam1.gameObject.SetActive(true);
                Frontcam2.gameObject.SetActive(true);
                break;
            case "C6_V2_FRONTRIGH":
                rc.SetActive(true);
                Rightcam1.gameObject.SetActive(true);
                Rightcam2.gameObject.SetActive(true);
                break;

            default:
                Debug.Log("something went wrong when disabling cameras");
                break;
        }
    }

}
