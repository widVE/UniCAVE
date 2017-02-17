using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uInput : ScriptableObject
{
    public string inputName;
    public bool isPressed;
    public uInput(string inputName, bool pressed)
    {
        this.inputName = inputName;
        this.isPressed = pressed;
        //Debug.Log(inputName);
        //name = inputName;
    }

    //setter
    public void setPressed(bool isPressed)
    {
        this.isPressed = isPressed;
    }
    public void setName(string inputName)
    {
        this.inputName = inputName;
    }
    //getters
    public bool getPressed()
    {
        return isPressed;
    }
    public string getName()
    {
        return this.inputName;
    }
}