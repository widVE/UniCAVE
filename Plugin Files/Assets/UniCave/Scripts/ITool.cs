/*
 * Luke Kingsley July 2017
 * Interface that provides the methods for the various tools which the wand will use
 */
using UnityEngine;

public interface ITool 
{
    /// <summary>
    /// Initializes all necessary objects for the class
    /// </summary>
    void init(); 

    /// <summary>
    /// Basic garbage collection and cleanup
    /// </summary>
    void shutDown();

    /// <summary>
    /// Handles all the button clicks from the input device
    /// </summary>
    /// <param name="button"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="cave"></param>
    /// <param name="rotate"></param>
    void ButtonClick(int button, Vector3 origin, Vector3 direction, bool cave, bool rotate);

    /// <summary>
    /// Takes in the analog data from the wand so the user can move around the unity scene 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void Analog(double x, double y);

    /// <summary>
    /// Lets the user click and hold buttons to have different functionality than just button clicks
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="offset"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction);

    /// <summary>
    /// Used only for when a button is immediatly pressed
    /// </summary>
    /// <param name="button"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    void buttonPress(int button, Vector3 origin, Vector3 direction);
}
