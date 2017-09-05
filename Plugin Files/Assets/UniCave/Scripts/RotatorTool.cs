using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorTool : MonoBehaviour, ITool {
    GameObject wandObject;
    GameObject holder;
    RaycastHit refHit;
    private const string IQ_WALL = "IQWall_Seq_1PC";
    private const string WAND = "Wand";


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
            wandObject = GameObject.Find(WAND);
        }

        if (holder == null)
        {
            holder = GameObject.Find(IQ_WALL);
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
