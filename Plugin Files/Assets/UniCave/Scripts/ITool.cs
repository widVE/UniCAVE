/*
 * Interface that provides the methods for the various tools which the wand will use
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The basic tool interface used by all tools
/// </summary>
public interface ITool 
{
    /// <summary>
    /// Gets the name of the tool
    /// </summary>
    string ToolName { get; }

    /// <summary>
    /// Starts the tool.
    /// </summary>
    void init(); 

    /// <summary>
    /// Stops the tool
    /// </summary>
    void shutDown();

    /// <summary>
    /// Handles the button click event. Not this happens on button up.
    /// </summary>
    /// <param name="buttonNum">The button enum to pressed.</param>
    /// <param name="origin">The position of the tracker</param>
    /// <param name="direction">The forward direction of the tracker</param>
    void ButtonClick(TrackerButton buttonNum, Vector3 origin, Vector3 direction);

    /// <summary>
    /// Handles the analog input from tracker.
    /// </summary>
    /// <param name="x">The x coord of the tracker. From [-1,.1].</param>
    /// <param name="y">The y coord of the tracker. From [-1,.1].</param>
    void Analog(double x, double y);

    /// <summary>
    /// Handles a drag event if a button is held down.
    /// </summary>
    /// <param name="hit">The object hit.</param>
    /// <param name="offset">The offset of the hit from the object center.</param>
    /// <param name="origin">The tracker position</param>
    /// <param name="direction">The forward direction of the tracker.</param>
    void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction);
}
