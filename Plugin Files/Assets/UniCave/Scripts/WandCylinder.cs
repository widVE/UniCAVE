using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to update the ray drawn via a cylinder to have different colors.
/// </summary>
public class WandCylinder : MonoBehaviour {

    public VRPNTrack wandObject;
    public Material red;
    public Material green;
    public bool currentValue = false;


    /// <summary>
    /// Starts up and inits the ray to red.
    /// </summary>
    // Use this for initialization
    void Start ()
    {
        currentValue = false;
        this.GetComponent<Renderer>().material = red;
        if (wandObject == null)
            Debug.LogError("Wand object must be set!");
    }
	

    /// <summary>
    /// Updates the ray to be red on no hit and green when an object is hit.
    /// </summary>
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
