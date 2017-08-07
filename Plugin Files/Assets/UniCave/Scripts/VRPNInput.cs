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

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

/// <summary>
/// Class handles all input from the controller
/// </summary>
public class VRPNInput : MonoBehaviour
{
   
    [SerializeField]
    private string trackerAddress = "WiiMote0@localhost";
    private int channelVertical = 22;
    private int channelHorizontal = 21;
    private int channelMagnitude = 20;
    [SerializeField]
    private int channel = 1; //channel for buttons, not analog
    [SerializeField]
    private bool trackButton = true;
    [SerializeField]
    private bool trackAnalog = true;

    //Unity Objects    
    public Material red;
    public Material green;
    public Text tool;
    public GameObject canvas;
    public GameObject holder;
    public GameObject wandObject = null;
    public GameObject cylinder;
    private GameObject panel;
    private ToolManager toolManager;
    private DriveTool driver;

    //button input
    bool[] buttonState;
    private bool hasStarted = false;
    bool curValue;
    public bool debugOutput = false;
    public int numButtons = 6;
    public float movementSpeed = 0.01f;
    public float rotationSpeed = 0.05f;
    public double deadZone = 0.05;
    public float rayLength = 200;

    //Collision Detection
    private Vector3 offset;
    private Vector3 origin;
    private Vector3 direction;
    private RaycastHit hit, temp, ray;

    //button mapping
    private int click, previous, drag, next;
    public bool Cave = false;
    public bool rotate;
    private bool isDrag = false;

    private const string TELEPORT = "Tool: Warp";
    private const string GRABBER = "Tool: Grabber";
    private const string BUTTONCLICK = "Tool: ButtonClick";
    private const string ROTATE = "Tool: Rotate";



    public int Channel
    {
        get { return channel; }
        set
        {
            channel = value;
        }
    }

    /// <summary>
    /// Show the user what tool is currently selected.
    /// </summary>
    public IEnumerator toolName()
    {
        while (true)
        {
            if (toolManager.toolNumber == 0)
            {
                tool.text = TELEPORT;
            }

            else if (toolManager.toolNumber == 1)
            {
                tool.text = GRABBER;
            }

            else if (toolManager.toolNumber == 2)
            {
                tool.text = BUTTONCLICK;
            }

            else if (toolManager.toolNumber == 3)
            {
                tool.text = ROTATE;
            }

            yield return null;
        }
    }

  
    public bool TrackButton
    {
        get { return trackButton; }
        set
        {
            trackButton = value;
            StopCoroutine("Button");
            if (trackButton && Application.isPlaying)
            {
                StartCoroutine("Button");
            }
        }
    }

    public bool TrackAnalog
    {
        get { return trackAnalog; }
        set
        {
            trackAnalog = value;
            StopCoroutine("Analog");
            if (trackAnalog && Application.isPlaying)
            {
                StartCoroutine("Analog");
            }
        }
    }

    /// <summary>
    /// Changes the color of raycast depending on whether an object is detected
    /// </summary>
    /// <returns></returns>
    private IEnumerator colorChange()
    {
        while (true)
        {
            //Get the origin of the object
            origin = wandObject.transform.position;

            //Get the direction of the object
            direction = wandObject.transform.forward;

            if (Physics.Raycast(origin, direction, out ray))
            {
                cylinder.GetComponent<Renderer>().material = green;
                Vector3 scale = cylinder.transform.localScale;
                float scalingFactor = Vector3.Distance(ray.transform.position, wandObject.transform.position);
                float currentSize = cylinder.GetComponent<MeshRenderer>().bounds.size.z;
                scale.y = 2 * scalingFactor * scale.y / currentSize;
                cylinder.transform.localScale = scale;
            }
            else
            {
                cylinder.GetComponent<Renderer>().material = red;
                cylinder.transform.localScale = new Vector3(.025f, 40, .025f);

            }
            yield return new WaitForSeconds(.15f);
        }
    }



    private void Start()
    {
        //Create a driver object to handle movement
        driver = new DriveTool(deadZone, rotationSpeed, movementSpeed);
        //Add a toolManager to the wandObject to shuffle between tools
        wandObject.AddComponent<ToolManager>();
        toolManager = new ToolManager(wandObject, holder);
        //Create an array of bools to track the button states
        buttonState = new bool[numButtons];
        //Set the cylinders original color
        cylinder.GetComponent<Renderer>().material = red;

        //Fill the array with false values
        for (int i = 0; i < numButtons; i++)
        {
            buttonState[i] = false;
        }

        if (Cave)
        {
            isCave();
        }
        else
        {
            IQwall();
        }
        //this gets rid of this object from non-head nodes...we only want this running on the machine that connects to VRPN...
        //this assumes a distributed type setup, where one machine connects to the tracking system and distributes information
        //to other machines...
        //some setups may try to connect each machine to vrpn...
        //in that case, we wouldn't want to destroy this object..
        if (System.Environment.MachineName != MasterTrackingData.HeadNodeMachineName)
        {
            Debug.Log("Removing tracker settings from " + System.Environment.MachineName);
            Destroy(this);
            return;
        }

        //Start the coroutines
        if (trackButton)
        {
            StartCoroutine("Button");
        }

        if (trackAnalog)
        {
            StartCoroutine("Analog");
            StartCoroutine("colorChange");
            StartCoroutine("toolName");
        }
    }

    /// <summary>
    /// Asynchronous method taking in button input and sending it to the current selected tool
    /// </summary>
    /// <returns></returns>
    private IEnumerator Button()
    {
        while (true)
        {
            //i tracks the number of the current button
            for (int i = 0; i < numButtons; ++i)
            {
                //Current value of the button
                curValue = VRPN.vrpnButton(trackerAddress, i);

                if (i == drag)
                    isDrag = VRPN.vrpnButton(trackerAddress, i);

                //If the previous state is true and the current value is false it is a button click
                if (buttonState[i] && !curValue)
                {
                    //Change between the different tools
                    if (i == previous)
                    {
                        toolManager.PreviousTool();
                        yield return new WaitForSeconds(.2f);
                    }

                    else if (i == next)
                    {
                        toolManager.NextTool();
                        yield return new WaitForSeconds(.2f);
                    }

                    //Fire the event
                    toolManager.list[toolManager.toolNumber].ButtonClick(i, origin, direction, Cave, rotate);
                    driver.ButtonClick(i, origin, direction, Cave, rotate);

                    if(!buttonState[drag])
                    {
                        hit = temp;
                        hasStarted = false;
                    }                    
                   
                }
                //If the current and previous are true then it is a buttondrag event
                if (buttonState[drag] && isDrag)
                {
                    if (hit.distance > 0)
                    {
                        //If this is the start of the drag event get the offset
                        if (!hasStarted)
                        {
                            offset = hit.transform.position - hit.point;
                            hasStarted = true;
                        }
                        //Fire the event
                        toolManager.list[toolManager.toolNumber].ButtonDrag(hit, offset, origin, direction);
                    }
                }
                //If the previous is false and the current is true 
                else if (!buttonState[drag] && isDrag)
                {
                    Physics.Raycast(origin, direction * rayLength, out hit);
                }

                ////Change between tools
                //if (VRPN.vrpnButton(trackerAddress, i))
                //{
                //    if (i == previous)
                //    {
                //        toolManager.PreviousTool();
                //        yield return new WaitForSeconds(.2f);
                //    }

                //    else if (i == next)
                //    {
                //        toolManager.NextTool();
                //        yield return new WaitForSeconds(.2f);
                //    }
                //}

                //update the previous value
                buttonState[i] = curValue;
            }
            yield return null;
        }
    }

    /// <summary>
    /// Sets all the necessary variables for cave interaction
    /// </summary>
    public void isCave()
    {
        //Change the address to the remote in the cave
        //Switch the channels to match those of the Cave remote
        trackerAddress = "DTrack@localhost";
        click = 2;
        drag = 0;
        next = 5;
        //Channel horizontal and vertical
        channelHorizontal = 0;
        channelVertical = 1;
        numButtons = 6;
    }


    /// <summary>
    /// Sets all the necessary variables for IQ_wall interaction
    /// </summary>
    public void IQwall()
    {
        click = 3;
        drag = 4;
        previous = 5;
        next = 6;
        numButtons = 11;
    }

    /// <summary>
    /// Asyncronous method that handles all the input from the analog stick
    /// </summary>
    /// <returns></returns>
    private IEnumerator Analog()
    {
        while (true)
        {
            //Get the X and Y values from the joystick
            double analogVertical = VRPN.vrpnAnalog(trackerAddress, channelVertical);
            double analogHorizontal = VRPN.vrpnAnalog(trackerAddress, channelHorizontal);

            //Translate the holder
            driver.Analog(analogHorizontal, analogVertical);
            yield return null;
        }
    }

}
