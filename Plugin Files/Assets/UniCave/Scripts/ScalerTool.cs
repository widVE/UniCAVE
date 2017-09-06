using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;


/// <summary>
/// The tool for scaling an object - currently not used.
/// </summary>
public class ScalerTool : MonoBehaviour, ITool

{
    GameObject wandObject;
    GameObject holder;
    Vector3 origin;

	// Use this for initialization
	void Start ()
    {
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
	void Update () {
		
	}

    public void init()
    {
        throw new NotImplementedException();
    }

    public void shutDown()
    {
        //throw new NotImplementedException();
    }

    public void ButtonClick(TrackerButton button, Vector3 origin, Vector3 direction)
    {
        //throw new NotImplementedException();
    }

    public void Analog(double x, double y)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Handles the scaling for the tool
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="offset"></param>
    /// <param name="position"></param>
    /// <param name="direction"></param>
    public void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 position, Vector3 direction)
    {
        if(origin == null)
        {
            origin = hit.point;
        }
        Vector3 objectScale = hit.transform.localScale;
        Vector3 a = position - origin;
        float magnitude = Vector3.Magnitude(a);
        float dotProduct = Vector3.Dot(direction, a);
        float scale = magnitude * dotProduct;

        hit.transform.localScale = new Vector3(objectScale.x + scale, objectScale.y + scale, objectScale.z + scale);
    }

    /// <summary>
    /// The tool name - scale tool.
    /// </summary>
    public string ToolName
    {
        get
        {
            return "Scaler";
        }
    }
}
