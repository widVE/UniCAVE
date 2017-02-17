using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TrackerSettings))]
public class TrackerSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TrackerSettings settings = target as TrackerSettings;

        if (settings != null)
        {
            settings.HostSettings = (TrackerHostSettings)EditorGUILayout.ObjectField("Host Settings", settings.HostSettings, typeof(TrackerHostSettings), true);
            settings.ObjectName = EditorGUILayout.TextField("Object Name", settings.ObjectName);
            settings.Channel = EditorGUILayout.IntField("Channel", settings.Channel);
            settings.TrackPosition = EditorGUILayout.Toggle("Track Position", settings.TrackPosition);
            settings.TrackRotation = EditorGUILayout.Toggle("Track Rotation", settings.TrackRotation);

            if (GUI.changed)
            {
                EditorUtility.SetDirty(settings);
            }
        }
    }
}
