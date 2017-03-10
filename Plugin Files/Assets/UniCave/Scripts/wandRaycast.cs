using UnityEngine;
using System.Collections;

//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

public class wandRaycast : MonoBehaviour {

    private Ray ray;
    private bool isGrabbed, isPressed;
    RaycastHit hitObject;
    int layerMask;
    LineRenderer lRend;
    //caveTrackerSettingsTrackerSettings tSettings;
    GameObject indicator;

	// Use this for initialization
	void Start () {
        indicator = GameObject.Find("WandButtonIndicator");
        ray = new Ray(Vector3.zero, Vector3.zero);
        layerMask = 1 << 8;
        lRend = gameObject.GetComponent<LineRenderer>();
        if (System.Environment.MachineName != "C6_V1_HEAD")
        {
            Destroy(this);
        }
        //tSettings = gameObject.GetComponent<caveTrackerSettings>();
        
	}
	
	// Update is called once per frame
	void Update () {
        updateRay();
        //if (tSettings != null)
        //{
        //    isPressed = tSettings.pressed;
        //}

        //if (checkRay() && isPressed)
        //{
        //    hitObject.transform.position = gameObject.transform.position;
        //}
	}

    bool checkRay()
    {
        if (Physics.Raycast(ray, out hitObject, 20, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void updateRay()
    {
        ray.origin = gameObject.transform.localPosition;
        ray.direction = gameObject.transform.forward; //check if forward is right
        lRend.SetPosition(0, gameObject.transform.localPosition);
        lRend.SetPosition(1, gameObject.transform.localPosition + (gameObject.transform.forward * 20)); //check if forward is right
        if (Physics.Raycast(ray, out hitObject, 20, layerMask))
        {
            lRend.material.color = Color.green;
            lRend.SetColors(Color.green, Color.green);
            indicator.transform.localPosition = new Vector3(5, 0, 0);
        }
        else
        {
            lRend.material.color = Color.red;
            lRend.SetColors(Color.red, Color.red);
            indicator.transform.localPosition = new Vector3(0, 10, 0);
        }
    }

}
