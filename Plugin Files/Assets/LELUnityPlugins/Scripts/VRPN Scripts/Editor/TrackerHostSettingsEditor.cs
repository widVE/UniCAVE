using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TrackerHostSettings))]
public class TrackerHostSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TrackerHostSettings settings = target as TrackerHostSettings;

        if (settings != null)
        {
            settings.Hostname = EditorGUILayout.TextField("Hostname", settings.Hostname);
            settings.Preset = (TrackerPreset)EditorGUILayout.EnumPopup("Type", settings.Preset);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(settings);
            }
        }
    }
}
