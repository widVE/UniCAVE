using UnityEngine;

public class TrackerHostSettings : MonoBehaviour
{
    [SerializeField]
    private string hostname = "localhost";
    [SerializeField]
    private TrackerPreset preset = TrackerPreset.Standard;

    private delegate Vector3 Position(string tracker, int channel);
    private delegate Quaternion Rotation(string tracker, int channel);
    private delegate bool Button(string tracker, int channel);
    private delegate double Analog(string tracker, int channel);

    private Position position;
    private Rotation rotation;
    private Button button;
    private Analog analog;

    public string Hostname
    {
        get { return hostname; }
        set { hostname = value; }
    }

    public TrackerPreset Preset
    {
        get { return preset; }
        set
        {
            preset = value;

            switch (preset)
            {
                case TrackerPreset.Standard:
                    position = VRPN.vrpnTrackerPos;
                    rotation = VRPN.vrpnTrackerQuat;
                    button = VRPN.vrpnButton;
                    analog = VRPN.vrpnAnalog;
                    break;
                case TrackerPreset.Vicon:
                    //position = ViconVRPN.vrpnTrackerPos;
                    //rotation = ViconVRPN.vrpnTrackerQuat;
                    break;
            }
        }
    }

    public Vector3 GetPosition(string tracker, int channel)
    {
        return position(tracker + "@" + hostname, channel);
    }

    public Quaternion GetRotation(string tracker, int channel)
    {
        return rotation(tracker + "@" + hostname, channel);
    }

    public bool GetButton(string tracker, int channel)
    {
        return button(tracker + "@" + hostname, channel);
    }

    public double GetAnalog(string tracker, int channel)
    {
        return analog(tracker + "@" + hostname, channel);
    }

    private void Awake()
    {
        Preset = preset;
    }
}
