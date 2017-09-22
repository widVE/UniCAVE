//MIT License
//Copyright 2016-Present 
//James H. Money
//Luke Kingsley
//Idaho National Laboratory
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using UnityEngine;
using System;
using System.Collections.Generic; // Import the System.Collections.Generic class to give us access to List<>


/// <summary>
/// Tracks the list of button mappings
/// </summary>
[ExecuteInEditMode]
public class TrackerButtonList : MonoBehaviour {

    //This is our custom class with our variables


    //This is our list we want to use to represent our class as an array.
    public List<ButtonMapping> list = new List<ButtonMapping>(1);
    private Dictionary<int, TrackerButton> buttonDictionary = new Dictionary<int, TrackerButton>();
    public VRPNInput vrpnInput;

    /// <summary>
    /// The startup code. Gets the VRPN input and maps the current buttons.
    /// </summary>
    private void Start()
    {
        vrpnInput = this.GetComponent<VRPNInput>();
        updateButtonMappings();
    }

    /// <summary>
    /// Adds a new button mapping.
    /// </summary>
    void AddNew(){
		//Add a new index position to the end of our list
		list.Add(new ButtonMapping());
	}

    /// <summary>
    /// Removes a button mapping
    /// </summary>
    /// <param name="index">The 0 based index of the mapping to remove from the list.</param>
	void Remove(int index){
		//Remove an index position from our list at a point in our list array
		list.RemoveAt(index);
	}


    /// <summary>
    /// Maps a button from number to the enumeration.
    /// </summary>
    /// <param name="buttonNumber">The 0 based index to map to an enum</param>
    /// <returns>The mapped enumeration or Unknown if not found in the list.</returns>
    public TrackerButton MapButton(int buttonNumber)
    {
        if (buttonDictionary.Keys.Count == 0)
            updateButtonMappings();

        if (buttonDictionary.ContainsKey(buttonNumber))
            return buttonDictionary[buttonNumber];
        else
            return TrackerButton.Unknown;
    }


    /// <summary>
    /// Updates the mapping in the dictionary for quick lookup.
    /// </summary>
    public void updateButtonMappings()
    {
        buttonDictionary.Clear();
        foreach(ButtonMapping map in list)
        {
            buttonDictionary.Add(map.ButtonNumber, map.MappedButton);
        }
    }

    /// <summary>
    /// Iterates and gets the max button number in the list.
    /// </summary>
    /// <returns>The last index + 1 of the button number in last that is maximum. If there are none, 0 is returned.</returns>
    public int getMaxButtons()
    {
       

        if (buttonDictionary.Keys.Count == 0)
            updateButtonMappings();

        int maxButtons = -1;
        foreach(int btn in buttonDictionary.Keys)
        {
            maxButtons = Math.Max(maxButtons, btn);
        }
        return maxButtons+1;
    }
}
