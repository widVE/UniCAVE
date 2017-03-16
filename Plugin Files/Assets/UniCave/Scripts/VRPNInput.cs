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
    
    public GameObject holder;

    private GameObject indicator;
        
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

        indicator = GameObject.Find("WandButtonIndicator");

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
            indicator.transform.localPosition = Vector3.zero;
            if (VRPN.vrpnButton(trackerAddress, 0))
            {
                Debug.Log("Button 0 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 0);
                yield return new WaitForSeconds(.2f);
            }
            else if (VRPN.vrpnButton(trackerAddress, 1))
            {
                Debug.Log("Button 1 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 1);
                yield return new WaitForSeconds(.2f);
            }
            else if (VRPN.vrpnButton(trackerAddress, 2))
            {
                Debug.Log("Button 2 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 2);
                yield return new WaitForSeconds(.2f);
            }
            else if (VRPN.vrpnButton(trackerAddress, 3))
            {
                Debug.Log("Button 3 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 3);
                yield return new WaitForSeconds(.2f);
            }
            else if (VRPN.vrpnButton(trackerAddress, 4))
            {
                Debug.Log("Button 4 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 4);
                yield return new WaitForSeconds(.2f);
            }
            else if (VRPN.vrpnButton(trackerAddress, 5))
            {
                Debug.Log("Trigger pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 5);
                //pressed = true;
                yield return new WaitForSeconds(.2f);
            }
            yield return null;
        }
    }

    private IEnumerator Analog()
    {
        //some basic default analog wand movement.
        GameObject wand = GameObject.Find("Wand");
        Vector3 modTrans;
        while (true)
        {
            if (VRPN.vrpnAnalog(trackerAddress, channel) >= 0.5)
            {
                //moving forward
                modTrans = wand.transform.localRotation * Vector3.forward * 0.01f;
                modTrans.y = 0;
                if (holder != null)
                {
                    holder.transform.Translate(modTrans);
                }
            }
            else if (VRPN.vrpnAnalog(trackerAddress, channel) <= -0.5)
            {
                //moving backwards
                modTrans = wand.transform.localRotation * Vector3.forward * 0.01f;
                modTrans.y = 0;
                if (holder != null)
                {
                    holder.transform.Translate(-modTrans);
                }
            }
            yield return null;
        }
    }
}
