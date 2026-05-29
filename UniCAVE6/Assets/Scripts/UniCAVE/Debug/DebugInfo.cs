using TMPro;
using UnityEngine;

namespace UniCAVE
{
    /// <summary>
    /// Provides debug display of current status and machine name.
    /// </summary>
    //[RequireComponent(typeof(TextMeshProUGUI), typeof(TextMeshProUGUI))]
    public class DebugInfo : MonoBehaviour
    {
        /// <summary>
        /// UProperty to TextMesh for machine name.
        /// </summary>
        public TextMeshProUGUI DebugTextMachineName;
        
        /// <summary>
        /// UProperty to TextMesh for status.
        /// </summary>
        public TextMeshProUGUI DebugTextStatus;

        /// <summary>
        /// Initializes TextMeshes with machine name and generic initial status.
        /// </summary>
        void Start()
        {
            if (DebugTextMachineName == null)
            {
                Debug.LogError("Missing debug text machine name!");
            }
            else
            {
                DebugTextMachineName.text = Util.GetMachineName();
            }

            if (DebugTextStatus == null)
            {
                Debug.LogError("Missing debug text status!");
            }
            else
            {
                DebugTextStatus.text = "Status: Unavailable";
            }
        }

        /// <summary>
        /// Updates displayed debug machine name based on values from Util class and provided message.
        /// </summary>
        /// <param name="msg">Message to be displayed in addition to machine name.</param>
        public void UpdateMachineName(string msg)
        {
            if (DebugTextMachineName != null)
            {
                DebugTextMachineName.text = Util.GetMachineName() + " " + msg + " eye: " + Util.GetArg("eye");
            }
        }

        /// <summary>
        /// Updates value shown in status display.
        /// </summary>
        /// <param name="connectionStatus">New boolean value for status display.</param>
        public void UpdateConnectionStatus(bool connectionStatus)
        {
            if (DebugTextStatus != null)
            {
                DebugTextStatus.text = "Status: " + connectionStatus;
            }
        }
    }

}
