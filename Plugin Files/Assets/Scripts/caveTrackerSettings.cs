using UnityEngine;
using System.Collections;

//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

public class caveTrackerSettings : MonoBehaviour
{
    [SerializeField]
    private TrackerHostSettings hostSettings;
    [SerializeField]
    private string trackerPrefix = "Isense900";
    [SerializeField]
    private int channel = 0;
    [SerializeField]
    private bool trackPosition = true;
    [SerializeField]
    private bool trackRotation = true;
    [SerializeField]
    private bool trackButton = true;
    [SerializeField]
    private bool trackAnalog = true;
    
    public GameObject holder;

    private GameObject indicator;
        
    public TrackerHostSettings HostSettings
    {
        get { return hostSettings; }
        set
        {
            hostSettings = value;
        }
    }

    public string ObjectName
    {
        get { return trackerPrefix; }
        set
        {
            trackerPrefix = value;
        }
    }

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
        //this gets rid of this object from non-head nodes...
        //this is causing problems for some setups because in some cases the VRPN machine isn't the head node.
        if (System.Environment.MachineName != MasterTrackingData.HeadNodeMachineName)
        {
                Debug.Log(System.Environment.MachineName);
                Destroy(this);
                return;
        }

        indicator = GameObject.Find("WandButtonIndicator");

        if (trackPosition)
        {
            StartCoroutine("Position");
        }

        if (trackRotation)
        {
            StartCoroutine("Rotation");
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

    private IEnumerator Position()
    {
        while (true)
        {
            transform.localPosition = hostSettings.GetPosition(trackerPrefix, channel);
            yield return null;
        }
    }

    Quaternion relativeQuat = new Quaternion();
    private IEnumerator Rotation()
    {
        while (true)
        {
            transform.rotation = hostSettings.GetRotation(trackerPrefix, channel);
            yield return null;
        }
    }

    private IEnumerator Button()
    {
        while (true)
        {
            indicator.transform.localPosition = Vector3.zero;
            if (hostSettings.GetButton(trackerPrefix, 0))
            {
                Debug.Log("Button 0 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 0);
                yield return new WaitForSeconds(.2f);
            }
            else if (hostSettings.GetButton(trackerPrefix, 1))
            {
                Debug.Log("Button 1 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 1);
                yield return new WaitForSeconds(.2f);
            }
            else if (hostSettings.GetButton(trackerPrefix, 2))
            {
                Debug.Log("Button 2 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 2);
                yield return new WaitForSeconds(.2f);
            }
            else if (hostSettings.GetButton(trackerPrefix, 3))
            {
                Debug.Log("Button 3 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 3);
                yield return new WaitForSeconds(.2f);
            }
            else if (hostSettings.GetButton(trackerPrefix, 4))
            {
                Debug.Log("Button 4 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 4);
                yield return new WaitForSeconds(.2f);
            }
            else if (hostSettings.GetButton(trackerPrefix, 5))
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
        
        GameObject wand = GameObject.Find("Wand");
        Vector3 modTrans;
        while (true)
        {
            if (hostSettings.GetAnalog(trackerPrefix, channel) >= 0.5)
            {
                //moving forward
                modTrans = wand.transform.localRotation * Vector3.forward * 0.01f;
                modTrans.y = 0;
                if (holder != null)
                {
                    holder.transform.Translate(modTrans);
                }
            }
            else if (hostSettings.GetAnalog(trackerPrefix, channel) <= -0.5)
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
