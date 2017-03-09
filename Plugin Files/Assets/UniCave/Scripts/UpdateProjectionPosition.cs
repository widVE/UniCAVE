using UnityEngine;
using System.Collections;

//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

public class UpdateProjectionPosition : MonoBehaviour {

	// Use this for initialization
    public bool LeftEye = false;
    private GameObject trackerRotation;

	void Start () {
        trackerRotation = GameObject.Find("TrackerRotation");
	}

    void LateUpdate() {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
#endif
        {
            Vector3 trackedHead = transform.parent.transform.parent.position;
            if (trackedHead != null)
            {
                if (LeftEye)
                {
                    transform.position = (trackedHead + trackerRotation.transform.rotation * MasterTrackingData.LeftEyeOffset);
                }
                else
                {
                    transform.position = (trackedHead + trackerRotation.transform.rotation * MasterTrackingData.RightEyeOffset);
                }
            }
        }
    }
}
