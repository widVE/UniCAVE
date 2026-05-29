//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
//Kevin Ponto
//Benny Wysong-Grass
//University of Wisconsin - Madison Virtual Environments Group
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

namespace UniCAVE
{
    /// <summary>
    /// Synchronizes video playback across server and clients.
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class NetworkVideo : MonoBehaviour //: NetworkBehaviour
    {
        /// <summary>
        /// The video player object being affected.
        /// </summary>
        private VideoPlayer _player;

        /// <summary>
        /// The video playhead position at the server.
        /// </summary>
        private float _canonicalVideoTime = 0.0f;
        
        /// <summary>
        /// The video playback speed at the server.
        /// </summary>
        /// 
        private float _canonicalPlaybackSpeed = 1.0f;
        
        /// <summary>
        /// Whether the video is playing at the server or not.
        /// </summary>
        private bool _canonicalPlaying = true;

        /// <summary>
        /// Real time clock value when canonical values were last updated.
        /// </summary>
        private float _lastUpdateRealTime = 0.0f;

        /// <summary>
        /// Maximum playback speed we will adjust to to catch up to the server.
        /// </summary>
        public float maxPlaybackRatio = 1.5f;
        
        /// <summary>
        /// If true, if server is too far ahead of client, client will jump directly to server position.
        /// </summary>
        public bool scrubWhenPlaying = true;

        /// <summary>
        /// Amount that is considered "too far" for <c>scrubWhenPlaying</c>.
        /// </summary>
        public float scrubThreshold = 1.0f;
        
        /// <summary>
        /// Calculate playback speeds to aim to catch up to server in this amount of time (within max limit).
        /// </summary>
        public float convergenceRate = 1.0f;
        
        // TODO this never seems to be actually set 
        private bool _isServer;

        /// <summary>
        /// Called by the server on clients to notify them of the server's video state. Also updates real time.
        /// </summary>
        /// <param name="canonicalVideoTime">Server's playhead position.</param>
        /// <param name="canonicalPlaybackSpeed">Server's playback speed.</param>
        /// <param name="canonicalPlaying">Server's playing status.</param>
        [ClientRpc]
        private void UpdateFieldsClientRpc(float canonicalVideoTime, float canonicalPlaybackSpeed, bool canonicalPlaying)
        {
            _lastUpdateRealTime = Time.time;
            _canonicalVideoTime = canonicalVideoTime;
            _canonicalPlaybackSpeed = canonicalPlaybackSpeed;
            _canonicalPlaying = canonicalPlaying;
        }

        /// <summary>
        /// Get reference to the video player component.
        /// </summary>
        void Start()
        {
            _player = GetComponent<VideoPlayer>();
        }

        /// <summary>
        /// If server, send state to clients each physics frame. If client, recieve and deal with it.
        /// </summary>
        void FixedUpdate()
        {
            // If we're the server, send our video state to clients.
            if(_isServer)
            {
                _canonicalVideoTime = (float)_player.time;
                _canonicalPlaybackSpeed = _player.playbackSpeed;
                _canonicalPlaying = _player.isPlaying;
                UpdateFieldsClientRpc(_canonicalVideoTime, _canonicalPlaybackSpeed, _canonicalPlaying);
            }
            else
            {
                if(_canonicalPlaying)
                {
                    // If server is playing, we should be playing.
                    if(!_player.isPlaying) _player.Play();
                    // Adjust playback speed based on the difference between server and client.
                    _player.playbackSpeed = _canonicalPlaybackSpeed * CorrectedPlaybackRatio;
                }
                else
                {
                    // If server is not playing, we should not be playing.
                    if(_player.isPlaying) _player.Pause();
                    // Update playhead position to be paused at the same place as the server.
                    _player.time = _canonicalVideoTime;
                    // Since we are now in sync, we can reset the playback speed.
                    _player.playbackSpeed = _canonicalPlaybackSpeed;
                }
                // If set to scrub when playing and difference is too big, just jump to correct point.
                // TODO: if _canonicalPlaying is false we should have already set time above, so do we need that branch?
                if((scrubWhenPlaying && Mathf.Abs(ExpectedTimeDif) > scrubThreshold) || (Mathf.Abs(ExpectedTimeDif) > 0.1f && !_canonicalPlaying))
                {
                    _player.time = _canonicalVideoTime;
                }
            }
        }

        /// <summary>
        /// Estimated position of the server's playhead at the current real time.
        /// </summary>
        private float ExpectedCanonicalVideoTime
        {
            get
            {
                float timeSinceLastUpdate = Time.time - _lastUpdateRealTime;
                return _canonicalVideoTime + _canonicalPlaybackSpeed * timeSinceLastUpdate;
            }
        }
        
        /// <summary>
        /// Difference between our playhead and the estimate of the server's playhead.
        /// </summary>
        private float ExpectedTimeDif => (float)(ExpectedCanonicalVideoTime - _player.time);

        private float CorrectedPlaybackRatio => Mathf.Clamp(Mathf.Exp(ExpectedTimeDif * convergenceRate), 0.125f, maxPlaybackRatio);
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NetworkVideo))]
    public class NetworkVideoEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            NetworkVideo netvid = target as NetworkVideo;

            //EditorGUILayout.FloatField("CanonicalVideoTime", netvid.canonicalVideoTime);
            //EditorGUILayout.FloatField("canonicalPlaybackSpeed", netvid.canonicalPlaybackSpeed);

            netvid.maxPlaybackRatio = EditorGUILayout.Slider(new GUIContent("Max Playback Ratio",
                "The maximum ratio of this clip's playback to the canonical clip's playback, for example Max Playback Ratio = 1.5 means the clip can go 50% faster."), netvid.maxPlaybackRatio, 1.1f, 4.0f);

            netvid.scrubWhenPlaying = EditorGUILayout.Toggle(new GUIContent("Scrub When Playing", "Scrub the video while it is playing if the video is out of sync more than a certain threshold"), netvid.scrubWhenPlaying);
            netvid.convergenceRate = EditorGUILayout.Slider(new GUIContent("Convergence Rate", "Relative value of how fast the videos will converge to the same time"), netvid.convergenceRate, 0.01f, 100.0f);

            if(netvid.scrubWhenPlaying)
            {
                netvid.scrubThreshold = EditorGUILayout.Slider(new GUIContent("Scrub Threshold",
                "If the clip is out of sync by more than this amount, it will scrub to the correct time instead of accelerating up to it."), netvid.scrubThreshold, 0.25f, 5.0f);
            }
        }
    }
#endif
}