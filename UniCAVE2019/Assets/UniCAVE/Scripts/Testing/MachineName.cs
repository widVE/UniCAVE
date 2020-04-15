using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniCAVE
{
    [CreateAssetMenu(fileName = nameof(MachineName), menuName = nameof(UniCAVE) + "/MachineName asset", order = 3000)]
    public class MachineName : ScriptableObject
    {
        [SerializeField]
        string _name;
        public string Name
        {
            get => _name;
            set => _name = value;
        }

#if UNITY_EDITOR
        [CanEditMultipleObjects]
        [CustomEditor(typeof(MachineName), true)]
        class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.LabelField("Machine Name:", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(_name)));

                serializedObject.ApplyModifiedProperties();

                if(GUILayout.Button("Get Local Machine Name"))
                {
                    string localMachineName = System.Environment.MachineName;

                    Undo.SetCurrentGroupName("Set Machine Name");

                    foreach(MachineName mn in targets)
                    {
                        Undo.RecordObject(mn, "Set Machine Name");

                        mn.Name = localMachineName;
                        EditorUtility.SetDirty(mn);
                    }

                    Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
                }
            }
        }
#endif
    }
}