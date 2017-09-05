using UnityEngine;
using System;
using System.Collections.Generic; // Import the System.Collections.Generic class to give us access to List<>

public class TrackerButtonList : MonoBehaviour {

    //This is our custom class with our variables


    //This is our list we want to use to represent our class as an array.
    public List<ButtonMapping> list = new List<ButtonMapping>(1);
    private Dictionary<int, TrackerButton> buttonDictionary = new Dictionary<int, TrackerButton>();


    private void Start()
    {
        updateButtonMappings();
    }

    void AddNew(){
		//Add a new index position to the end of our list
		list.Add(new ButtonMapping());
	}

	void Remove(int index){
		//Remove an index position from our list at a point in our list array
		list.RemoveAt(index);
	}


    public TrackerButton MapButton(int buttonNumber)
    {
        if (buttonDictionary.Keys.Count == 0)
            updateButtonMappings();

        if (buttonDictionary.ContainsKey(buttonNumber))
            return buttonDictionary[buttonNumber];
        else
            return TrackerButton.Unknown;
    }

    public void updateButtonMappings()
    {
        buttonDictionary.Clear();
        foreach(ButtonMapping map in list)
        {
            buttonDictionary.Add(map.ButtonNumber, map.MappedButton);
        }
    }

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
