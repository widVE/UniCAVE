//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

using UnityEngine;
using System.Collections;

public class VRPNInput : MonoBehaviour
{
    [SerializeField]
    private string trackerAddress = "Wand0@C6_V1_HEAD";
    [SerializeField]
    private int channel = 1;
    [SerializeField]
    private bool trackButton = true;
    [SerializeField]
    private bool trackAnalog = true;

    public GameObject wandObject = null;
    public bool debugOutput = false;
    public int numButtons = 6;
    public float movementSpeed = 0.01f;
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
            double analog = VRPN.vrpnAnalog(trackerAddress, channel);
            if (analog >= deadZone || analog <= -deadZone)
            {
                if (debugOutput)
                {
                    Debug.Log("Analog input value " + analog + " on channel " + channel);
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

                    if (analog >= deadZone)
                    {   //moving forward
                        holder.transform.Translate(modTrans);
                    }
                    else
                    {   //moving back
                        holder.transform.Translate(-modTrans);
                    }
                }
            }

            yield return null;
        }
    }
}
