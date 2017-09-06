using UnityEngine;
using System.Runtime.InteropServices;

public static class VRPN
{
    [DllImport("unityVrpn")]
    private static extern double vrpnAnalogExtern(string address, int channel, int frameCount);

    [DllImport("unityVrpn")]
    private static extern bool vrpnButtonExtern(string address, int channel, int frameCount);

    [DllImport("unityVrpn")]
    private static extern double vrpnTrackerExtern(string address, int channel, int component, int frameCount);

    public static double vrpnAnalog(string address, int channel)
    {
        return vrpnAnalogExtern(address, channel, Time.frameCount);
    }

    public static bool vrpnButton(string address, int channel)
    {
        return vrpnButtonExtern(address, channel, Time.frameCount);
    }

    /// <summary>
    /// Get if a tracker button is pressed. Needed for editor mode since framecount is not available.
    /// </summary>
    /// <param name="address">The address of the tracker</param>
    /// <param name="channel">The button number of the button (0 based).</param>
    /// <param name="frameCount">The count of the frame.</param>
    /// <returns></returns>
    public static bool vrpnButton(string address, int channel, int frameCount)
    {
        return vrpnButtonExtern(address, channel, frameCount);
    }

    //todo - need to allow different transforms here...
    //need to adjust two below functions to match up with your own tracking system's transform
    public static Vector3 vrpnTrackerPos(string address, int channel)
    {
        return new Vector3(
            (float)vrpnTrackerExtern(address, channel, 1, Time.frameCount) ,
            -(float)vrpnTrackerExtern(address, channel, 2, Time.frameCount) ,
            (float)vrpnTrackerExtern(address, channel, 0, Time.frameCount)) + MasterTrackingData.TrackingSystemOffset;
    }

    public static Quaternion vrpnTrackerQuat(string address, int channel)
    {
        return new Quaternion(
            (float)vrpnTrackerExtern(address, channel, 4, Time.frameCount),
            -(float)vrpnTrackerExtern(address, channel, 5, Time.frameCount),
            (float)vrpnTrackerExtern(address, channel, 3, Time.frameCount),
            -(float)vrpnTrackerExtern(address, channel, 6, Time.frameCount));
    }
}
