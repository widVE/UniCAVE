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
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;



/// <summary>
/// Editor for the list of mappings. We could potentially move this to VRPNInput in the future.
/// </summary>
[CustomEditor(typeof(TrackerButtonList))]
public class TrackerButtonListEditor : Editor
{ 

	TrackerButtonList t;
	SerializedObject GetTarget;
	SerializedProperty ThisList;
	int ListSize;

    /// <summary>
    /// Noting to start yet.
    /// </summary>
    private void Start()
    {
        
    }

    /// <summary>
    /// Handles the enabling of the editor. Gets the list currently.
    /// </summary>
	void OnEnable()
    {
		t = (TrackerButtonList)target;
		GetTarget = new SerializedObject(t);
		ThisList = GetTarget.FindProperty("list"); // Find the List in our script and create a refrence of it
	}


    /// <summary>
    /// Handles the update to the GUI
    /// </summary>
	public override void OnInspectorGUI()
    {
		//Update our list

		GetTarget.Update();

		//Choose how to display the list<> Example purposes only

		//Resize our list
		EditorGUILayout.LabelField("Define the list size with a number");
		ListSize = ThisList.arraySize;
		ListSize = EditorGUILayout.IntField ("List Size", ListSize);

		if(ListSize != ThisList.arraySize)
        {
			while(ListSize > ThisList.arraySize)
            {
				ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
			}
			while(ListSize < ThisList.arraySize)
            {
				ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
			}
		}

		
		//Or add a new item to the List<> with a button

		if(GUILayout.Button("Add New"))
        {
			t.list.Add(new ButtonMapping());
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//Display our list to the inspector window

		for(int i = 0; i < ThisList.arraySize; i++)
		{
			SerializedProperty ListRef = ThisList.GetArrayElementAtIndex(i);
			SerializedProperty MyButtonNumber = ListRef.FindPropertyRelative("ButtonNumber");
			SerializedProperty MyMappedButton = ListRef.FindPropertyRelative("MappedButton");


            // Choose to display automatic or custom field types. This is only for example to help display automatic and custom fields.
			EditorGUILayout.PropertyField(MyButtonNumber);
			EditorGUILayout.PropertyField(MyMappedButton);

            
            if (GUILayout.Button("Map Button"))
            {
                int button = t.vrpnInput.GetPushedButton();
                if (button > -1)
                    MyButtonNumber.intValue = button;
                
            }

            EditorGUILayout.Space ();

			//Remove this index from the List
			if(GUILayout.Button("Remove This Index (" + i.ToString() + ")"))
            {
				ThisList.DeleteArrayElementAtIndex(i);
			}
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
			EditorGUILayout.Space ();
		}

		//Apply the changes to our list
		GetTarget.ApplyModifiedProperties();
	}
}

#endif