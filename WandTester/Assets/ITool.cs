/*
 * Interface that provides the methods for the various tools which the wand will use
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITool 
{
    void init(); 
    void shutDown();
<<<<<<< HEAD
    void ButtonClick(int button, Vector3 origin, Vector3 direction);
=======
    void ButtonClick(int button, Vector3 origin, Vector3 direction, bool cave);
>>>>>>> 18b14ea308eb5bdebe912452d748a21ee7cbc337
    void Analog(double x, double y);
    void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction);
}
