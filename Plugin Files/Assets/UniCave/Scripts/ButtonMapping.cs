using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class handles a mapping buttons
/// </summary>
[System.Serializable]
public class ButtonMapping{

    /// <summary>
    /// The button enumeration value
    /// </summary>
	public TrackerButton MappedButton;

    /// <summary>
    /// The number of the button (0...n-1) for n buttons.
    /// </summary>
	public int ButtonNumber;

}