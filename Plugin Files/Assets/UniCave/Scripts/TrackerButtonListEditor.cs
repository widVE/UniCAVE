using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TrackerButtonList))]
public class TrackerButtonListEditor : Editor
{ 

	TrackerButtonList t;
	SerializedObject GetTarget;
	SerializedProperty ThisList;
	int ListSize;
    private void Start()
    {
        
    }


	void OnEnable()
    {
		t = (TrackerButtonList)target;
		GetTarget = new SerializedObject(t);
		ThisList = GetTarget.FindProperty("list"); // Find the List in our script and create a refrence of it
	}

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
                if (button >-1)
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