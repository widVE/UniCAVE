using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandCylinder : MonoBehaviour {

    public VRPNTrack wandObject;
    public Material red;
    public Material green;
    public bool currentValue = false;


    // Use this for initialization
    void Start ()
    {
        currentValue = false;
        this.GetComponent<Renderer>().material = red;
        if (wandObject == null)
            Debug.LogError("Wand object must be set!");
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (wandObject != null)
        {
            bool rayHit = Physics.Raycast(wandObject.transform.position, wandObject.transform.forward);
            if (currentValue != rayHit)
            {
                currentValue = rayHit;
                if (currentValue)
                    this.GetComponent<Renderer>().material = green;
                else
                    this.GetComponent<Renderer>().material = red;
            }
        }
            
    }
}
