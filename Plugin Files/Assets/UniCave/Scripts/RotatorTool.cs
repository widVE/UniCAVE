using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorTool : MonoBehaviour, ITool
{
    GameObject wandObject;
    GameObject holder;
    RaycastHit refHit;
    private const string IQ_WALL = "IQWall_Seq_1PC";
    private const string WAND = "Wand";

    private Vector3 prevRotation;
    private Vector3 tempRotation;
    private DateTime previousTime;

    /// <summary>
    /// Rotates the object when the user has selected it
    /// NOTE: ON WII REMOTE, BUTTON IS "B" BUTTON!!!
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="offset"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    public void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction)
    {

        // If no previous rotation was set, set it now and then wait for another rotation
        if (previousTime == null || (DateTime.Now - previousTime).TotalMilliseconds > 100f)
        {
            prevRotation = wandObject.transform.rotation.eulerAngles;
            previousTime = DateTime.Now;
            return;
        }

        // Calculate the difference between the previous rotation state and the current rotation state.
        //      hit.transform.localEulerAngles= prevRotation + (wandObject.transform.localEulerAngles- tempRotation);
        tempRotation = wandObject.transform.rotation.eulerAngles - prevRotation;

        // Rotate the selected object by the change in rotation
        hit.transform.Rotate(tempRotation);

        // Save current rotation for next step
        prevRotation = wandObject.transform.rotation.eulerAngles;
        previousTime = DateTime.Now;

    }

    // Use this for initialization
    void Start()
    {
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
    void Update()
    {

    }

    public void init()
    {
        throw new NotImplementedException();
    }

    public void shutDown()
    {
        
    }

    public void Analog(double x, double y)
    {
        throw new NotImplementedException();
    }

    public void ButtonClick(int button, Vector3 origin, Vector3 direction, bool cave)
    {
        //Debug.Log("THis is a button " + button);
    }

    public void ButtonClick(int button, Vector3 origin, Vector3 direction, bool cave, bool rotate)
    {
        throw new NotImplementedException();
    }

    public void buttonPress(int button, Vector3 origin, Vector3 direction)
    {
        throw new NotImplementedException();
    }
}
