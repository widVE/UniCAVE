//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
//Brady Boettcher
//Living Environments Laboratory
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//Edited by Luke Kingsley Summer 2017

using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class moves the user around the scene 
/// </summary>
public class DriveTool : ITool {

    private const string IQ_WALL = "IQWall_Seq_1PC";
    private const string WAND = "Wand";

    private float movementSpeed;
    private float rotationSpeed;
    private double deadZone;
    private bool restrictVerticalMovement = true;
    private GameObject holder;
    private GameObject wand;
    bool rotateVertical = false;
    bool rotateHorizontal = false;
    bool one = false;
    bool two = false;
    string text = "Default";


    /// <summary>
    /// DriveTool Constructor. Object handles all the analog 
    /// </summary>
    /// <param name="deadZone_"></param>
    /// <param name="rotationSpeed_"></param>
    /// <param name="movementSpeed_"></param>
    public DriveTool(double deadZone_, float rotationSpeed_, float movementSpeed_)
    {
        wand = GameObject.Find(WAND);
        holder = GameObject.Find(IQ_WALL);
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
                {
                    Rotate(analogData, true);
                }

                else
                {
                    Translate(analogData, true);
                }
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
                {
                    Rotate(analogData, false);
                }

                else
                {
                    Translate(analogData, false);
                }
            }
        }
    }

    //Switches the orientation of the rotation of the holder object
    public void setHorizontal()
    {
        if (rotateHorizontal)
        {
            rotateHorizontal = false;
        }
        else
        {
            rotateHorizontal = true;
        }
    }

    public void setVertical()
    {
        if (rotateVertical)
        {
            rotateVertical = false;
        }
        else
        {
            rotateVertical = true;
        }
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
    public void ButtonClick(int button, Vector3 origin, Vector3 direction, bool cave, bool rotate)
    {
        if(!cave)
        {
            if (button == 1)
            {
                if(one)
                {
                    one = false;
                }
                else
                {
                    one = true;
                }
                setHorizontal();
                setText();
            }

            if (button == 2)
            {
                if (two)
                {
                    two = false;
                }
                else
                {
                    two = true;
                }
                setVertical();
                setText();
            }
        }     
    }

    public void setText()
    {
        

        if (one && two)
        {
            text = "Vertical and Horizontal camera mode";
        }

        else if (one)
        {
            text = "Horizontal camera mode";
        }
        else if (two)
        {
            text = "Vertical camera mode";
        }
        else
        {
            text = "No camera mode";
        }


        GameObject.Find("buttons").GetComponent<Text>().text = text;
    }

    //////////////////Unimplemented Methods//////////////////

    public void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction)
    {
        throw new NotImplementedException();
    }

    public void ButtonClick(int button)
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

    public void buttonPress(int button, Vector3 origin, Vector3 direction)
    {
        throw new NotImplementedException();
    }
}
