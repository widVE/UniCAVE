//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
//Brady Boettcher
//Living Environments Laboratory
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
using System;
using System.IO;

namespace UniCAVE
{
    //[NetworkSettings(sendInterval = 0.016f)]
    public class VRPNTrack : MonoBehaviour //: NetworkBehaviour
    {
        /// <summary>
        /// Address where it can get the VRPN signal
        /// </summary>
        [SerializeField] private string trackerAddress = "Glasses1@127.0.0.1:3883";
        
        /// <summary>
        /// VRPN channel
        /// </summary>
        [SerializeField] private int channel = 0;

        /// <summary>
        /// Decide if object should track the VRPN position
        /// </summary>
        [SerializeField] private bool _trackPosition = true;

        /// <summary>
        /// Decide if the object should track the VRPN rotation
        /// </summary>
        [SerializeField] private bool _trackRotation = true;

        /// <summary>
        /// Offset for position
        /// </summary>
        public Vector3 TrackerPositionOffset;

        /// <summary>
        /// Offset for rotation
        /// </summary>
        public Vector3 TrackerRotationOffset;

        /// <summary>
        /// Debug output to the console
        /// </summary>
        public bool DebugOutput = false;

        /// <summary>
        /// get the head name, needed to decide if the instance should try and get VRPN signal
        /// </summary>
        [SerializeField] MachineName headMachineAsset;
        public string HeadMachine => MachineName.GetMachineName(headMachineAsset);
        
        /// <summary>
        /// Track position of head
        /// </summary>
        public bool TrackPosition
        {
            get => _trackPosition;
            set
            {
                _trackPosition = value;
                StopCoroutine(nameof(VRPNTrack.Position));
                if (_trackPosition && Application.isPlaying)
                {
                    StartCoroutine(nameof(VRPNTrack.Position));
                }
            }
        }

        /// <summary>
        /// Track the rotation of head
        /// </summary>
        public bool TrackRotation
        {
            get => _trackRotation;
            set
            {
                _trackRotation = value;
                StopCoroutine(nameof(VRPNTrack.Rotation));
                if (_trackRotation && Application.isPlaying)
                {
                    StartCoroutine(nameof(VRPNTrack.Rotation));
                }
            }
        }

        /// <summary>
        /// Destoy the object if it is not a server or a host and then starts the VRPN tracking
        /// </summary>
        private void Start()
        {
            //this gets rid of this object from non-head nodes...we only want this running on the machine that connects to VRPN...
            //this assumes a distributed type setup, where one machine connects to the tracking system and distributes information

            if (Util.GetMachineName() != HeadMachine)
            {
                Debug.LogWarning("It is not server, destroying TrackVRPN");
                UnityEngine.Object.Destroy(this);
                return;
            }

            if (_trackPosition)
            {
                StartCoroutine(nameof(VRPNTrack.Position));
            }

            if (_trackRotation)
            {
                StartCoroutine(nameof(VRPNTrack.Rotation));
            }
        }

        /// <summary>
        /// Track a position of an object
        /// </summary>
        private IEnumerator Position()
        {
            while (true)
            {
                Vector3 pos = VRPN.vrpnTrackerPos(trackerAddress, channel) + TrackerPositionOffset;

                // If there is no VRPN signal coming (for example when running it locally)
                // head will teleport to x=-505 y=-505 which is outside the cave and prevents from seeing anything
                // instead the head can be placed in the middle of the CAVE 
                /*if (pos.x == -505)
                {
                    pos.x = 0f;
                    pos.y = -1f;
                    pos.z = 0f;
                }

                pos = new Vector3(-pos.z, -pos.y * 0.93f, -pos.x);*/

                transform.localPosition = pos;

                if(DebugOutput)
                {
                    Debug.Log($"VRPN Position: {pos}");
                }

                yield return null;
            }
        }

        /// <summary>
        /// Tracks a rotation of an object
        /// </summary>
        /// <returns></returns>
        private IEnumerator Rotation()
        {
            while (true)
            {
                Quaternion rotation = VRPN.vrpnTrackerQuat(trackerAddress, channel);
                /*if(rotation.x == -505)
                {
                    rotation.x = 0;
                    rotation.y = 0;
                    rotation.z = 0;
                }

                rotation = new Quaternion(-rotation.z, -rotation.y, -rotation.x, rotation.w);*/
                transform.localRotation = rotation * Quaternion.Euler(TrackerRotationOffset);
 
                if (DebugOutput)
                {
                    Debug.Log($"VRPN Rotation: {rotation}");
                }

                yield return null;
            }
        }
    }
}