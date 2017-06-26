using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class GrabberTool : MonoBehaviour, ITool
{
    //Initialize 
    public GameObject holder;
    public GameObject wandObject;
    private RaycastHit hit;
    public int rayLength;
    public Vector3 origin, direction, previousOrigin, previousDirection;
    
    //Initializes all the necessary fields while rendering the scene
    private void Start()
    {
        //Get all necessary game objects
        if (wandObject == null)
        {
            wandObject = GameObject.Find("Wand");
        }

        if (holder == null)
        {
            holder = GameObject.Find("IQWall_Seq_1PC");
        }
        rayLength = 200;
    }

    /// <summary>
    /// Handles the button input for the grabberTool 
    /// Takes in every button and whether it is currently being pressed or not 
    /// </summary>
    public void ButtonClick(int button, Vector3 origin_, Vector3 direction_)
    {
      if((Physics.Raycast(origin_, direction_ * rayLength, out hit)))
        {
            if(button == 7 )//&& isPressed_)
            {
                hit.transform.Rotate(0, 20, 0);
            }

            if (button == 8)// && isPressed_)
            {
                hit.transform.Rotate(0, -20, 0);
            }

            if (button == 9)// && isPressed_)
            {
                hit.transform.Rotate(-20, 0, 0);
            }

            if (button == 10)// && isPressed_)
            {
                hit.transform.Rotate(20, 0, 0);
            }
        }
    }

    /// <summary>
    /// Asyncrounous methof that moves whatever object the raycast from the wand has hit.
    /// </summary>
    /// <returns></returns>
    public void ButtonDrag(RaycastHit hit_, Vector3 offset_, Vector3 origin_, Vector3 direction_)
    {
            //objectTransform = hit.transform.position;
            //Set the direction of the wand.
            direction = wandObject.transform.forward;   //NOTE: hit.point does not update...

            //offset = hit.transform.position - hit.point;
            //Set the transform of the object hit
            Vector3 number = origin_ + (direction * hit_.distance) + offset_;
            hit_.transform.position = origin_ + (direction * hit_.distance) + offset_;
    }

   
    ///////////Unimplemented Functions /////////////////// 

    public void init()
    {
        throw new NotImplementedException();
    }

    public void shutDown()
    {
        //throw new NotImplementedException();
    }

    public void Analog(double x, double y)
    {
        throw new NotImplementedException();
    }
}




