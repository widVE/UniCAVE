using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumeration of the buttons we can map. This includes the trigger, 6 buttons, previous & next buttons, and Unknown for a bad mapping.
/// </summary>
public enum TrackerButton
{
	Trigger,
	Button1,
	Button2,
	Button3,
	Button4,
	Button5,
	Button6,

    PreviousTool,
    NextTool,
    Unknown
}