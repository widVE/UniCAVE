using UnityEngine;
using System.Collections;

public class WallGrid : MonoBehaviour {
    public float gridDisplayDistance = .1f; //in meters
    public Color gridColor;
    private Color invisible;
    private Plane plane;
    private Material wallMat;
    private GameObject trackedHead;

	void Start () {
        wallMat = gameObject.GetComponent<MeshRenderer>().material;
        trackedHead = GameObject.Find("CameraHolder");
        plane = new Plane(-transform.forward, transform.localPosition);
        invisible = new Color(gridColor.r, gridColor.g, gridColor.b, 0f);
        if(wallMat != null) wallMat.SetColor("_Color", invisible);
	}
	
	void Update () {
        if ((wallMat != null) && (trackedHead != null))
        {
            float d = plane.GetDistanceToPoint(trackedHead.transform.localPosition);
            if (d < 0f)
            {
                Color newColor = new Color(gridColor.r, gridColor.g, gridColor.b, 1f);
                wallMat.SetColor("_Color", newColor);
            }
            else if ((d >= 0f) && (d < gridDisplayDistance))
            {
                Color newColor = new Color(gridColor.r, gridColor.g, gridColor.b, opacityFunction(d));
                wallMat.SetColor("_Color", newColor);
            }
            else if (wallMat.color != invisible) wallMat.SetColor("_Color", invisible);
        }
	}

    //opacity = 0 at minimum grid display distance, 1 when touching the wall
    private float opacityFunction(float d)
    {
        return (gridDisplayDistance - d) / gridDisplayDistance;
    }
}
