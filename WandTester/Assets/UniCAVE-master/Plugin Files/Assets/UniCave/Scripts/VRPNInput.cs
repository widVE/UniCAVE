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

public class VRPNInput : MonoBehaviour
{
    public bool Cave = false;


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

    private DriveTool driver;

    public GameObject wandObject = null;
    public bool debugOutput = false;
    public int numButtons = 6;
    public float movementSpeed = 0.01f;
    public float rotationSpeed = 0.05f;
    public double deadZone = 0.05;
    public Text tool;
    public GameObject canvas;
    public GameObject holder;
    private ToolManager2 toolManager;
    private Vector3 origin;
    private Vector3 direction;
    public float rayLength = 200;
    bool[] buttonState;
    bool curValue;
    RaycastHit hit;
    RaycastHit tester;
    Vector3 offset;
    bool hasStarted = false;
    RaycastHit temp;
    public GameObject cylinder;
    public Material red;
    public Material green;
    public GameObject panel;
    public Event eventsystem;
    public Dropdown dropdown;
    public Toggle toggle;
<<<<<<< HEAD
=======
    int click;
    int drag;
    int next;
    int previous;
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337

    public int Channel
    {
        get { return channel; }
        set
        {
            channel = value;
        }
    }


    //private void Update()
    //{
    //    toolName();
    //}

    /// <summary>
    /// Show the user what tool is currently selected.
    /// </summary>
    public IEnumerator toolName()
    {
        while (true)
        {
            if (toolManager.toolNumber == 0)
            {
                tool.text = "Tool: Warp";
                //line.material.mainTexture = texture;
            }

            else if (toolManager.toolNumber == 1)
            {
                tool.text = "Tool: Grabber";
                //cylinder.GetComponent<Renderer>().material.color = 
            }

            else if(toolManager.toolNumber == 2)
            {
                tool.text = "Tool: Clicker";
            }
            else
            {

            }

            yield return null;
        }
    }

    public IEnumerator buttonInput()
    {
        bool hide = true;
        while (true)
        {
            if(toolManager.toolNumber == 2)
            {
                Physics.Raycast(origin, direction, out tester);
                if (tester.collider != null)
                {
<<<<<<< HEAD
                
                if(tester.collider.tag == "button")
                    {
                        Button button = tester.transform.gameObject.GetComponent<Button>();
                        EventSystem.current.SetSelectedGameObject(button.gameObject);
                    }
                else if (tester.collider.tag == "dropdown")
=======
                             
                    if (hit.transform.GameObject.GetComponent<Dropdown>() != null)
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
                    {
                        dropdown = tester.transform.gameObject.GetComponent<Dropdown>();
                        EventSystem.current.SetSelectedGameObject(dropdown.gameObject);
                        //yield return new WaitForSeconds(2f);
                    }

<<<<<<< HEAD
                    else if (tester.collider.tag == "selectable")
=======
                    else if (hit.transform.GameObject.GetComponent<Toggle>() != null)
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
                    {
                        toggle = tester.transform.gameObject.GetComponent<Toggle>();
                        EventSystem.current.SetSelectedGameObject(toggle.gameObject);
                        //yield return new WaitForSeconds(2f);
                    }
<<<<<<< HEAD
=======

                    else if (hit.transform.GameObject.GetComponent<Button>() != null)
                    {
                        Button button = tester.transform.gameObject.GetComponent<Button>();
                        EventSystem.current.SetSelectedGameObject(button.gameObject);
                    }

>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
                }
                else
                {
                    if(hide == false)
                    {
                       
                    }
                    EventSystem.current.SetSelectedGameObject(null);
                }
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

    private IEnumerator colorChange()
    {
        while (true)
<<<<<<< HEAD
        {           
=======
        {
            //Get the origin of the object
            origin = wandObject.transform.position;

            //Get the direction of the object
            direction = wandObject.transform.forward;

>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
            if (Physics.Raycast(origin, direction))
            {
                cylinder.GetComponent<Renderer>().material = green;
            }
            else
            {
                cylinder.GetComponent<Renderer>().material = red;
            }
            yield return new WaitForSeconds(.15f);
        }
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        //Create a driver object to handle movement
        driver = new DriveTool(deadZone, rotationSpeed, movementSpeed);
        //Add a toolManager to the wandObject to shuffle between tools
        wandObject.AddComponent<ToolManager2>();
        //toolManager = wandObject.GetComponent<ToolManager2>();
        toolManager = new ToolManager2(wandObject, holder);

        buttonState = new bool[numButtons];

        cylinder.GetComponent<Renderer>().material = red;

        for (int i = 0; i < numButtons; i++)
        {
            buttonState[i] = false;
        }

        if (Cave)
        {
            //Change the address to the remote in the cave
            //Switch the channels to match those of the Cave remote
<<<<<<< HEAD
        }

=======
            trackerAddress = "DTrack@localhost";
            click = 2;
            drag = 0;
            next = 5;
            //Channel horizontal and vertical
        }
        else
        {
            click = 3;
            drag = 4;
            previous = 5;
            next = 6;
        }
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
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

        if (trackButton)
        {
            StartCoroutine("Button");
        }

        if (trackAnalog)
        {
            StartCoroutine("Analog");
            StartCoroutine("colorChange");
            StartCoroutine("toolName");
<<<<<<< HEAD
            StartCoroutine("Raycaster");
=======
            //StartCoroutine("Raycaster");
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
            StartCoroutine("buttonInput");
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
                curValue = VRPN.vrpnButton(trackerAddress, i);

                if (buttonState[i] && !curValue)
                {
                    toolManager.list[toolManager.toolNumber].ButtonClick(i, origin, direction);
                    driver.ButtonClick(i, origin, direction);

                    hasStarted = false;
                    hit = temp;

                }
<<<<<<< HEAD
                else if (buttonState[i] && curValue && i == 4)
=======
                else if (buttonState[i] && curValue && i == drag)
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
                {
                    if (hit.distance > 0)
                    {
                        if (!hasStarted)
                        {
                            offset = hit.transform.position - hit.point;
                            hasStarted = true;
                        }

<<<<<<< HEAD
                        //if (toolManager.toolNumber == 2)
                       // {
//toolManager.list[toolManager.toolNumber].ButtonClick(i, origin, direction);
                       /// }
                        //else
                        //{
                            toolManager.list[toolManager.toolNumber].ButtonDrag(hit, offset, origin, direction);
                        //}
                    }
                }

                else if (!buttonState[i] && curValue && i == 4)
=======
                        toolManager.list[toolManager.toolNumber].ButtonDrag(hit, offset, origin, direction);
                    }
                }

                else if (!buttonState[i] && curValue && i == drag)
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
                {
                    Physics.Raycast(origin, direction * rayLength, out hit);
                }

                buttonState[i] = curValue;

<<<<<<< HEAD
                //Sends the button currently being iterated over and whether or not it is being pressed


                //Handles all movement of the wandObject

=======
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337

                //Change between tools
                if (VRPN.vrpnButton(trackerAddress, i))
                {
                    switch (i)
                    {
<<<<<<< HEAD
                        case 5:
                            toolManager.PreviousTool();
                            yield return new WaitForSeconds(.2f);
                            break;
                        case 6:
=======
                        case previous:
                            toolManager.PreviousTool();
                            yield return new WaitForSeconds(.2f);
                            break;
                        case next:
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
                            toolManager.NextTool();
                            yield return new WaitForSeconds(.2f);
                            break;
                        default:
                            break;
                    }
                    // yield return new WaitForSeconds(.02f);
                }
            }
            yield return null;
        }
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

            driver.Analog(analogHorizontal, analogVertical);

            yield return null;
        }
    }

    /// <summary>
    /// Asyncronous method that continually casts a Raycast from the wands direction
    /// </summary>
    /// <returns></returns>
<<<<<<< HEAD
    private IEnumerator Raycaster()
    {
        //Debug.Log("!");
        //Continually update the wandObjects direction and position
        while (true)
        {
            //Get the origin of the object
            origin = wandObject.transform.position;

            //Get the direction of the object
            direction = wandObject.transform.forward;

            //Create the RayCast and Draw it for debugging purposes
            //Physics.Raycast(origin, direction * rayLength);
            Debug.DrawRay(origin, direction, Color.black);

            yield return null;
        }
    }
=======
    //private IEnumerator Raycaster()
    //{
    //    //Debug.Log("!");
    //    //Continually update the wandObjects direction and position
    //    while (true)
    //    {


    //        //Create the RayCast and Draw it for debugging purposes
    //        //Physics.Raycast(origin, direction * rayLength);
    //        Debug.DrawRay(origin, direction, Color.black);

    //        yield return null;
    //    }
    //}
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337

}
