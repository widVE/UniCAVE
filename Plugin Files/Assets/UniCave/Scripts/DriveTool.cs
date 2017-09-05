/*
 * Class handles the movement of the wiimote around the worldspace
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriveTool : ITool {

    public float movementSpeed;
    public float rotationSpeed;
    public double deadZone; 
    public bool restrictVerticalMovement = true;
    public GameObject holder;
    public GameObject wand;
    bool rotateVertical = false;
    bool rotateHorizontal = true;

    /// <summary>
    /// DriveTool Constructor. Object handles all the analog 
    /// </summary>
    /// <param name="deadZone_"></param>
    /// <param name="rotationSpeed_"></param>
    /// <param name="movementSpeed_"></param>
    public DriveTool(double deadZone_, float rotationSpeed_, float movementSpeed_)
    {
        wand = GameObject.Find("Wand");
        holder = GameObject.Find("IQWall_Seq_1PC");
        movementSpeed = movementSpeed_;
        rotationSpeed = rotationSpeed_;
        deadZone = deadZone_;        
    }

    /// <summary>
    /// Function handles all the analog data and movement of the IQstation
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void Analog(double x, double y)
    {
        //Create a vector fromt the X and Y data
        Vector2 analogData = new Vector2((float)x, (float)y);
        //Set the movement speed of the wandObject to the magnitude of the vector
        movementSpeed = analogData.magnitude; /// 2f;

        //Check if the Y value is bad input 
        if (y >= deadZone || y <= -deadZone) // || movementSpeed <= -deadZone)
        {
            //Check if the IQ station is initialized
            if (holder != null)
            {
                //Check to see whether you are rotating or translating the object
                if (rotateVertical)
                    Rotate(analogData, true);
                else
                    Translate(analogData, true);
            }
        }

        //Check if the X value is bad input 
        if (x >= deadZone || x <= -deadZone) // || analogHorizontal <= -deadZone)
        {
            //Check if the IQ station is initialized
            if (holder != null)
            {
                //Check to see whether you are rotating or translating the object
                if (rotateHorizontal)
                    Rotate(analogData, false);
                else
                    Translate(analogData, false);
            }
        }
    }

    //Switches the orientation of the rotation of the holder object
    public void setHorizontal()
    {
        rotateHorizontal = !rotateHorizontal;
    }

    public void setVertical()
    {
        rotateVertical = !rotateVertical;
    }
    public void ButtonClick(TrackerButton button)
    {
        throw new NotImplementedException();
    }

    public void ButtonDrag(RaycastHit hit, Vector3 sdlfkg)
    {
        throw new NotImplementedException();
    }

    public void init()
    {
        throw new NotImplementedException();
    }

    public void init(GameObject holder, GameObject wandObject)
    {
        throw new NotImplementedException();
    }

    public void shutDown()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// Function moves the object around the world space
    /// </summary>
    /// <param name="analogData"></param>
    /// <param name="vertical"></param>
    private void Translate(Vector2 analogData, bool vertical)
    {
        Vector3 modTrans;
        Vector3 movement;
        float value;
        float sign;

        //If Vertical is true get the y value from the vector and set other values
        if (vertical)
        {
            movement = Vector3.forward;
            value = analogData.y;
            sign = Mathf.Sign(analogData.y);
        }

        else
        {
            movement = Vector3.left;
            value = analogData.x;
            sign = Mathf.Sign(analogData.x);
        }

        //Calculate the vector the object will be moving on using the rotation of the wandObject
        if (wand != null)
        {
            modTrans = wand.transform.localRotation * movement * movementSpeed;
        }
        else
        {
            modTrans = Vector3.forward * movementSpeed;
        }
        //If you are moving in the horizontal direction set the y to 0
        if (!vertical)
        {
            modTrans.y = 0;
        }

        //If the sign is postive move it accordingly
        if (sign > 0)
        {   //moving forward or right
            holder.transform.Translate(modTrans * .5f);

        }
        else
        {   //moving back or left
            holder.transform.Translate(-modTrans * .5f);

        }
    }

    /// <summary>
    /// Function rotates the object about the world space
    /// </summary>
    /// <param name="analogData"></param>
    /// <param name="Vertical"></param>
    private void Rotate(Vector2 analogData, bool Vertical)
    {
        if (Vertical)
        {
            float sign = Mathf.Sign(analogData.y);
            if (sign > 0)
            {   //rotating up
                holder.transform.Rotate(new Vector3(rotationSpeed * movementSpeed, 0, 0));
            }
            else
            {   //rotating down
                holder.transform.Rotate(new Vector3(-rotationSpeed * movementSpeed, 0, 0));
            }
        }
        else
        {
            float sign = Mathf.Sign(analogData.x);
            if (sign > 0)
            {   //rotating right
                holder.transform.Rotate(new Vector3(0, rotationSpeed * movementSpeed, 0));
            }
            else
            {   //rotating left
                holder.transform.Rotate(new Vector3(0, -rotationSpeed * movementSpeed, 0));
            }
        }
    }

    /// <summary>
    /// Takes in the button input which tells the object which way to rotate
    /// </summary>
    /// <param name="button"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="isPressed"></param>
    public void ButtonClick(TrackerButton button, Vector3 origin, Vector3 direction)
    {
        
        if (button == TrackerButton.Button3)
            setHorizontal();
        else if (button == TrackerButton.Button4)
            setVertical();
    }

    public void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction)
    {
        throw new NotImplementedException();
    }

    public string ToolName
    {
        get
        {
            return "Drive";
        }
    }
}
