using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniCAVE
{
    /// <summary>
    /// Represents a machine name in UniCave. Used instead of a string for interning and use of the asset editor.
    /// </summary>
    [CreateAssetMenu(fileName = nameof(MachineName), menuName = nameof(UniCAVE) + "/MachineName asset", order = 3000)]
    public class MachineName : ScriptableObject
    {
        [SerializeField]
        string _name;
        /// <summary>
        /// The name of the machine.
        /// </summary>
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        /// <summary>
        /// Returns the machine name stored in an asset if it exists. Otherwise returns the old machine name value.
        /// </summary>
        public static string GetMachineName(string deprecatedMachineName, MachineName machineNameAsset)
        {
            //prioritize the machine name stored in the asset
            if(machineNameAsset) return machineNameAsset.Name;

            //otherwise, return the old machine name string
            return !string.IsNullOrEmpty(deprecatedMachineName) ? deprecatedMachineName : string.Empty;
        }
        /// <summary>
        /// Returns machine name stored in an asset
        /// </summary>
        /// <returns></returns>
        public static string GetMachineName(MachineName machineNameAsset)
        {
            //prioritize the machine name stored in the asset
            return machineNameAsset.Name;
        }

#if UNITY_EDITOR
        const string MachineNameWarning = "String-based machine names are deprecated! Please create a MachineName asset (Create->UniCAVE->MachineName).";

        public static void DrawDeprecatedMachineName(SerializedProperty machineNameString, SerializedProperty machineNameAsset, string machineNameAssetLabel = null)
        {
            //if there is a MachineName asset, the old machine name string will be hidden

            //if there is no MachineName asset and there is an old machine name value, display it with a warning
            if(machineNameAsset.objectReferenceValue == null && !string.IsNullOrEmpty(machineNameString.stringValue))
            {
                EditorGUILayout.HelpBox(MachineName.MachineNameWarning, MessageType.Warning);

                GUI.enabled = false;
                EditorGUILayout.PropertyField(machineNameString);
                GUI.enabled = true;
            }

            //draw the MachineName asset with an optional custom display name
            GUIContent machineNameAssetGUIContent = new(string.IsNullOrEmpty(machineNameAssetLabel) ? machineNameAsset.displayName : machineNameAssetLabel);
            EditorGUILayout.PropertyField(machineNameAsset, machineNameAssetGUIContent);
        }

        [CanEditMultipleObjects]
        [CustomEditor(typeof(MachineName), true)]
        class Editor : UnityEditor.Editor
        {
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUILayout.LabelField("Machine Name:", EditorStyles.boldLabel);

                EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(MachineName._name)));

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