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

public class VRPNInput : MonoBehaviour
{
    [SerializeField]
    private string trackerAddress = "Wand0@C6_V1_HEAD";
    private int channelVertical = 1;
    private int channelHorizontal = 0;
    [SerializeField]
    private int channel = 1; //channel for buttons, not analog
    [SerializeField]
    private bool trackButton = true;
    [SerializeField]
    private bool trackAnalog = true;

    public GameObject wandObject = null;
    public bool debugOutput = false;
    public int numButtons = 6;
    public float movementSpeed = 0.01f;
    public float rotationSpeed = 5.0f;
    public double deadZone = 0.5;
    public bool restrictVerticalMovement = true;

    public GameObject holder;
  
    public int Channel
    {
        get { return channel; }
        set
        {
            channel = value;
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

    private void Start()
    {
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
        }
    }

    private IEnumerator Button()
    {
        while (true)
        {
            for(int i = 0; i < numButtons; ++i)
            {
                if (VRPN.vrpnButton(trackerAddress, i))
                {
                    if (debugOutput)
                    {
                        Debug.Log("Button " + i + " pressed on channel " + channel);
                    }
                    yield return new WaitForSeconds(.2f);
                }
            }
           
            yield return null;
        }
    }

    private IEnumerator Analog()
    {
        //some basic default analog wand movement.
        if (wandObject == null)
        {
            wandObject = GameObject.Find("Wand");
        }

        Vector3 modTrans;
        while (true)
        {
            double analogVertical = VRPN.vrpnAnalog(trackerAddress, channelVertical);
            double analogHorizontal = VRPN.vrpnAnalog(trackerAddress, channelHorizontal);
            if (analogVertical >= deadZone || analogVertical <= -deadZone)
            {
                if (debugOutput)
                {
                    Debug.Log("Analog input value " + analogVertical + " on channel " + channelVertical);
                }

                if (holder != null)
                {
                   
                    if (wandObject != null)
                    {
                        modTrans = wandObject.transform.localRotation * Vector3.forward * movementSpeed;
                    }
                    else 
                    {
                        modTrans = Vector3.forward * movementSpeed;
                    }

                    if (restrictVerticalMovement)
                    {
                        modTrans.y = 0;
                    }

                    if (analogVertical >= deadZone)
                    {   //moving forward
                        holder.transform.Translate(modTrans);
                    }
                    else
                    {   //moving back
                        holder.transform.Translate(-modTrans);
                    }
                }
            }
            if (analogHorizontal >= deadZone || analogHorizontal <= -deadZone)
            {
                if (debugOutput)
                {
                    Debug.Log("Analog input value " + analogHorizontal + " on channel " + channelHorizontal);
                }

                if (holder != null)
                {

                    if (analogHorizontal >= deadZone)
                    {   //rotating right
                        holder.transform.Rotate(new Vector3(0, rotationSpeed, 0));
                    }
                    else
                    {   //rotating left
                        holder.transform.Rotate(new Vector3(0, -rotationSpeed, 0));
                    }
                }
            }

            yield return null;
        }
    }
}
