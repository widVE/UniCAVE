using UnityEngine;
using System.Collections;

public class TrackerSettings : MonoBehaviour
{
    [SerializeField]
    private TrackerHostSettings hostSettings;
    [SerializeField]
    private string objectName = "";
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
    public bool pressed;
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
        get { return objectName; }
        set
        {
            objectName = value;
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
        if (System.Environment.MachineName != "C6_V1_HEAD")
        {
			if (System.Environment.MachineName != "DL_V1_HEAD") {
			Debug.Log(System.Environment.MachineName);
				Destroy(this);
			}
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
            transform.localPosition = hostSettings.GetPosition(objectName, channel);
            yield return null;
        }
    }

    Quaternion relativeQuat = new Quaternion();
    private IEnumerator Rotation()
    {
        while (true)
        {
            if (gameObject.name == "eyeRotFront")
            {
                Quaternion q = hostSettings.GetRotation(objectName, channel);
                relativeQuat.eulerAngles = new Vector3(0, 0, 0);
                transform.rotation = relativeQuat * q;
                //Vector3 eulerAngles = new Vector3(0, -90, 0);
                //transform.rotation = eulerAngles * q;
                //transform.GetChild(0).rotation =
                //transform.GetChild(0).rotation =
            }
            else if (gameObject.name == "eyeRotBack")
            {
                Quaternion q = hostSettings.GetRotation(objectName, channel);
                relativeQuat.eulerAngles = new Vector3(0, 180, 0);
                transform.rotation = relativeQuat * q;
            }
            else if (gameObject.name == "eyeRotLeft")
            {
                Quaternion q = hostSettings.GetRotation(objectName, channel);
                relativeQuat.eulerAngles = new Vector3(0, 270, 0);
                transform.rotation = relativeQuat * q;
            }
            else if (gameObject.name == "eyeRotRight")
            {
                Quaternion q = hostSettings.GetRotation(objectName, channel);
                relativeQuat.eulerAngles = new Vector3(0, 90, 0);
                transform.rotation = relativeQuat * q;
            }
            else if (gameObject.name == "eyeRotFloor")
            {
                Quaternion q = hostSettings.GetRotation(objectName, channel);
                relativeQuat.eulerAngles = new Vector3(90, 270, 0);
                transform.rotation = relativeQuat * q;
            }
            else if (gameObject.name == "eyeRotCeiling")
            {
                Quaternion q = hostSettings.GetRotation(objectName, channel);
                relativeQuat.eulerAngles = new Vector3(270, 270, 0);
                transform.rotation = relativeQuat * q;
            }
            else {
                transform.rotation = hostSettings.GetRotation(objectName, channel);
            }
            //transform.Rotate(q.ToEuler());
            //transform.rotation = hostSettings.GetRotation(objectName, channel);
            yield return null;
        }
    }

    private IEnumerator Button()
    {
        while (true)
        {
            indicator.transform.localPosition = Vector3.zero;
            if (hostSettings.GetButton(objectName, 0)) {
                Debug.Log("Button 0 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 0);
				yield return new WaitForSeconds(.2f);
            } else if (hostSettings.GetButton(objectName, 1)) {
                Debug.Log("Button 1 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 1);
                yield return new WaitForSeconds(.2f);
            } else if (hostSettings.GetButton(objectName, 2)) {
                Debug.Log("Button 2 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 2);
                yield return new WaitForSeconds(.2f);
            } else if (hostSettings.GetButton(objectName, 3)) {
                Debug.Log("Button 3 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 3);
                yield return new WaitForSeconds(.2f);
            } else if (hostSettings.GetButton(objectName, 4)) {
                Debug.Log("Button 4 pressed");
                indicator.transform.localPosition = new Vector3(1, 1, 4);
                yield return new WaitForSeconds(.2f);
            } else if (hostSettings.GetButton(objectName, 5)) {
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
        GameObject holder = GameObject.Find("Holder");
        GameObject wand = GameObject.Find("Wand");
        Vector3 modTrans;
        while (true)
        {
            if (hostSettings.GetAnalog(objectName, channel) >= 0.5)
            {
                //moving forward
                modTrans = wand.transform.localRotation * Vector3.forward * 0.1f;
                modTrans.y = 0;
                holder.transform.Translate(modTrans);
            }
            else if (hostSettings.GetAnalog(objectName, channel) <= -0.5)
            {
                //moving backwards
                modTrans = wand.transform.localRotation * Vector3.forward * 0.1f;
                modTrans.y = 0;
                holder.transform.Translate(-modTrans);
            }
            yield return null;
        }
    }
}
