//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

using UnityEngine;
using System.Collections;

public class VRPNTrack : MonoBehaviour
{
    [SerializeField]
    private string trackerAddress = "Isense900@C6_V1_HEAD";
    [SerializeField]
    private int channel = 0;
    [SerializeField]
    private bool trackPosition = true;
    [SerializeField]
    private bool trackRotation = true;

    public bool debugOutput = false;

    public int Channel
    {
        get { return channel; }
        set
        {
            channel = value;
        }
    }

    public bool TrackPosition
    {
        get { return trackPosition; }
        set
        {
            trackPosition = value;
            StopCoroutine("Position");
            if (trackPosition && Application.isPlaying)
            {
                StartCoroutine("Position");
            }
        }
    }

    public bool TrackRotation
    {
        get { return trackRotation; }
        set
        {
            trackRotation = value;
            StopCoroutine("Rotation");
            if (trackRotation && Application.isPlaying)
            {
                StartCoroutine("Rotation");
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

        if (trackPosition)
        {
            StartCoroutine("Position");
        }

        if (trackRotation)
        {
            StartCoroutine("Rotation");
        }
    }

    private IEnumerator Position()
    {
        while (true)
        {
            transform.localPosition = VRPN.vrpnTrackerPos(trackerAddress, channel);
            yield return null;
        }
    }

    Quaternion relativeQuat = new Quaternion();
    private IEnumerator Rotation()
    {
        while (true)
        {
            transform.rotation = VRPN.vrpnTrackerQuat(trackerAddress, channel);
            yield return null;
        }
    }
}
