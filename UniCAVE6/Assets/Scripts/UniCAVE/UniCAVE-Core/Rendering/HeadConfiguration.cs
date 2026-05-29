//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
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


using UnityEngine;
#if UNITY_EDITOR
#endif

namespace UniCAVE
{
    /// <summary>
    /// Class responsible for creating the cameras necessary to render for this machine's displays
    /// Cameras are created as children of this object, so movement and rotation correctly positions cameras
    /// Head and eyes should correspond to the viewer's real head and eyes
    /// </summary>
    [System.Serializable]
    public class HeadConfiguration : MonoBehaviour
    {
        /// <summary>
        /// Prefab to create, should contain a Camera component (default used if not)
        /// </summary>
        [Tooltip("Prefab object with camera component")]
        public GameObject camPrefab = null;

        /// <summary>
        /// Offset of the left eye from center
        /// </summary>
        public Vector3 leftEyeOffset;
        /// <summary>
        /// Offset of the right eye from center
        /// </summary>
        public Vector3 rightEyeOffset;

        /// <summary>
        /// Offset of the center "eye", always exactly between the left and right
        /// </summary>
        public Vector3 CenterEyeOffset => (leftEyeOffset + rightEyeOffset) * 0.5f;
        
        /// <summary>
        /// Default values for near and far clipping
        /// TODO pop-out means that near clipping plane isn't in screen space, where is it?
        /// </summary>
        public float nearClippingPlane = 0.01f, farClippingPlane = 100.0f;

        public enum Eyes { Left, Center, Right }

        /// <summary>
        /// Get the offset of the specified eye.
        /// </summary>
        /// <param name="eye">Member of the Eyes enum type representing which eye to get</param>
        /// <returns>The offset for the specified eye.</returns>
        public Vector3 GetEyeOffset(Eyes eye)
        {
            return eye switch
            {
                Eyes.Left => leftEyeOffset,
                Eyes.Center => CenterEyeOffset,
                Eyes.Right => rightEyeOffset,
                _ => CenterEyeOffset
            };
        }

        public Camera CreateLeftEye(string name)
        {
            return CreateEye(Eyes.Left, name);
        }

        public Camera CreateCenterEye(string name)
        {
            return CreateEye(Eyes.Center, name);
        }

        public Camera CreateRightEye(string name)
        {
            return CreateEye(Eyes.Right, name);
        }

        /// <summary>
        /// Create eye camera object.
        /// </summary>
        /// <param name="eye">Which eye to create.</param>
        /// <param name="name">Name of camera's display.</param>
        /// <returns>The created eye camera.</returns>
        public Camera CreateEye(Eyes eye, string name, bool forceNoPrefab=false)
        {
            // Instantiate specified camera prefab, or an empty GO if there isn't one.
            GameObject obj = (camPrefab != null && !forceNoPrefab) ? UnityEngine.Object.Instantiate(camPrefab) : new GameObject("EyeCam");

            // If the camera prefab doesn't have a camera component, add one.
            Camera res = obj.GetComponent<Camera>();
            if (!res)
            {
                res = obj.AddComponent<Camera>();
            }

            //obj.AddComponent<> // todo: add here a network component for syncing between IGs?

            // Give camera default name
            obj.name = $"{System.Enum.GetName(typeof(Eyes), eye)} Eye for: {name}";

            // Set clipping planes to standard values
            res.nearClipPlane = nearClippingPlane;
            res.farClipPlane = farClippingPlane;

            // Create as a geometry child of this (head) object, with offset equal to eye offset
            res.transform.parent = transform;
            res.transform.localPosition = GetEyeOffset(eye);
            
            // Set camera's target eye on stereo systems - if system has 3D stereo support, this will 
            // cause frame interleaved 3D to be handled at the hardware level
            res.stereoTargetEye = eye switch
            {
                Eyes.Left => StereoTargetEyeMask.Left,
                Eyes.Right => StereoTargetEyeMask.Right,
                _ => StereoTargetEyeMask.Both
            };

            return res;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Draw a sphere at the position of the head, and smaller spheres for each eye (including center eye)
        /// </summary>
        private void OnDrawGizmos()
        {
            Matrix4x4 ltwm = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(ltwm * new Vector4(leftEyeOffset.x, leftEyeOffset.y, leftEyeOffset.z, 1.0f), 0.01f);
            Gizmos.DrawWireSphere(ltwm * new Vector4(CenterEyeOffset.x, CenterEyeOffset.y, CenterEyeOffset.z, 1.0f), 0.01f);
            Gizmos.DrawWireSphere(ltwm * new Vector4(rightEyeOffset.x, rightEyeOffset.y, rightEyeOffset.z, 1.0f), 0.01f);
            Gizmos.DrawWireSphere(transform.position, Mathf.Max(leftEyeOffset.magnitude, rightEyeOffset.magnitude) + 0.02f);
        }
#endif
    }
}