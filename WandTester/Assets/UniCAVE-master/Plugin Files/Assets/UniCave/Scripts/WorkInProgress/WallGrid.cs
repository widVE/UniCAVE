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
