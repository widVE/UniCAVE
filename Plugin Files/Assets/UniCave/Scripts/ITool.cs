/*
 * Interface that provides the methods for the various tools which the wand will use
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITool
{
    /// <summary>
    /// initializes all the objects
    /// </summary>
    void init();

    /// <summary>
    /// Basic cleanup and garbage collection
    /// </summary>
    void shutDown();

    /// <summary>
    /// Handles button clicks from the input device
    /// </summary>
    /// <param name="button"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    /// <param name="cave"></param>
    void ButtonClick(int button, Vector3 origin, Vector3 direction, bool cave);

    /// <summary>
    /// Takes in input from the analog stick
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    void Analog(double x, double y);

    /// <summary>
    /// Handles click and drags
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="offset"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction);
}
