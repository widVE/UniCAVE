using UnityEngine;
using System;
using System.Collections.Generic; // Import the System.Collections.Generic class to give us access to List<>

public class TrackerButtonList : MonoBehaviour {

	//This is our custom class with our variables


	//This is our list we want to use to represent our class as an array.
	public List<ButtonMapping> list = new List<ButtonMapping>(1);


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
        for(int ii=0;ii<list.Count;ii++)
        {
            if (list[ii].ButtonNumber == buttonNumber)
                return list[ii].MappedButton;
        }
        return TrackerButton.Unknown;
    }
}
