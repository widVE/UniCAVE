using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorTool : MonoBehaviour, ITool {
    public GameObject wandObject;
    public GameObject holder;
    RaycastHit refHit;


    /// <summary>
    /// Rotates the object when the user has selected it
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="offset"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    public void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction)
    {
        if(refHit.collider == null)
        {
            refHit = hit;            
        }

        hit.transform.rotation = wandObject.transform.rotation;

    }

    // Use this for initialization
    void Start () {
        //Get all necessary game objects
        if (wandObject == null)
        {
            Debug.LogError("Need to set wand object!");
        }

        if (holder == null)
        {
            Debug.LogError("Need to set top level UniCAVE object!");
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void init()
    {
        throw new NotImplementedException();
    }

    public void shutDown()
    {
        refHit = new RaycastHit();
    }

    public void Analog(double x, double y)
    {
        throw new NotImplementedException();
    }

    public void ButtonClick(TrackerButton button, Vector3 origin, Vector3 direction)
    {

    }

    public string ToolName
    {
        get
        {
            return "Rotator";
        }
    }
}
