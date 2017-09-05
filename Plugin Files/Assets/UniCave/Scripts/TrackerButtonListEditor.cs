using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(TrackerButtonList))]

public class TrackerButtonListEditor : Editor {

	enum displayFieldType {DisplayAsAutomaticFields, DisplayAsCustomizableGUIFields}
	displayFieldType DisplayFieldType;

	TrackerButtonList t;
	SerializedObject GetTarget;
	SerializedProperty ThisList;
	int ListSize;

	void OnEnable(){
		t = (TrackerButtonList)target;
		GetTarget = new SerializedObject(t);
		ThisList = GetTarget.FindProperty("list"); // Find the List in our script and create a refrence of it
	}

	public override void OnInspectorGUI(){
		//Update our list

		GetTarget.Update();

		//Choose how to display the list<> Example purposes only
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		DisplayFieldType = (displayFieldType)EditorGUILayout.EnumPopup("",DisplayFieldType);

		//Resize our list
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField("Define the list size with a number");
		ListSize = ThisList.arraySize;
		ListSize = EditorGUILayout.IntField ("List Size", ListSize);

		if(ListSize != ThisList.arraySize){
			while(ListSize > ThisList.arraySize){
				ThisList.InsertArrayElementAtIndex(ThisList.arraySize);
			}
			while(ListSize < ThisList.arraySize){
				ThisList.DeleteArrayElementAtIndex(ThisList.arraySize - 1);
			}
		}

		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.LabelField("Or");
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//Or add a new item to the List<> with a button
		EditorGUILayout.LabelField("Add a new item with a button");

		if(GUILayout.Button("Add New")){
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


			// Display the property fields in two ways.

			if(DisplayFieldType == 0)
			{
				// Choose to display automatic or custom field types. This is only for example to help display automatic and custom fields.
				//1. Automatic, No customization <-- Choose me I'm automatic and easy to setup
				EditorGUILayout.LabelField("Automatic Field By Property Type");
				EditorGUILayout.PropertyField(MyButtonNumber);
				EditorGUILayout.PropertyField(MyMappedButton);


			}
			else
			{
				//Or

				//2 : Full custom GUI Layout <-- Choose me I can be fully customized with GUI options.
				EditorGUILayout.LabelField("Customizable Field With GUI");

				MyButtonNumber.intValue = EditorGUILayout.IntField("Button Number",MyButtonNumber.intValue);
				EditorGUILayout.PropertyField(MyMappedButton);
			}

			EditorGUILayout.Space ();

			//Remove this index from the List
			EditorGUILayout.LabelField("Remove an index from the List<> with a button");
			if(GUILayout.Button("Remove This Index (" + i.ToString() + ")")){
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