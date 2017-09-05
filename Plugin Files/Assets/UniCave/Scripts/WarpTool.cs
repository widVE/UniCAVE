using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpTool : MonoBehaviour, ITool
{
    public GameObject holder;
    public GameObject wandObject;
    private RaycastHit hit;
    public int rayLength;
    public Stack<Vector3> previousWarps;

    /// <summary>
    /// Basically a constructor
    /// </summary>
    private void Start()
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

        //StartCoroutine("Raycaster");
        rayLength = 100;
        previousWarps = new Stack<Vector3>();
    }



    /// <summary>
    /// Teleports the IQstation the length of the raycast or to an object 
    /// </summary>
    public void Warp(Vector3 origin, Vector3 direction)
    {
        //Add the location to the stack of warps
        previousWarps.Push(holder.transform.position);

        //returns true if an object is hit and fills the raycasthit struct with data
        if (Physics.Raycast(origin, direction * rayLength, out hit))
        {
            //Transport the IQstation to the object
            holder.transform.position = Vector3.Lerp(holder.transform.position, hit.point, .75f);
        }
        else
        {
            //Transport the IQstation to the end of the rayCast
            holder.transform.position = Vector3.Lerp(holder.transform.position, holder.transform.position + direction * rayLength, .75f);
        }
    }

    /// <summary>
    /// Keeps a record of all teleportations that the user does and reverts them back if desired
    /// </summary>
    public void undoWarp()
    {
        //If the stack isnt empty pop off the top object
        if(previousWarps.Count != 0)
        {
            holder.transform.position = previousWarps.Pop();
        }
    }

    /// <summary>
    /// Handles the button input for the warp tool
    /// </summary>
    public void ButtonClick(TrackerButton button, Vector3 origin_, Vector3 direction_)
    {
       
        if (button == TrackerButton.Trigger)
        {
            Warp(origin_, direction_);
        }
        if (button == TrackerButton.Button1)
        {
            undoWarp();
        }
    }


    ///////////Unimplemented Functions /////////////////// 
    public void Analog(double x, double y)
    {
        throw new NotImplementedException();
    }

    public IEnumerator ButtonDrag()
    {
        throw new NotImplementedException();
    }

    public void init()
    {
    }

    public void shutDown()
    {
    }

    public void ButtonDrag(RaycastHit hit, Vector3 offset)
    {
        //Do nothing
    }

    public void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction)
    {
        //throw new NotImplementedException();
    }

    public string ToolName
    {
        get
        {
            return "Warp";
        }
    }
}
