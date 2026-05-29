//MIT License
//Copyright 2016-Present 
//James H. Money
//Luke Kingsley
//Idaho National Laboratory
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using UnityEngine;

namespace UniCAVE
{
    /// <summary>
    /// Class to update the ray drawn via a cylinder to have different colors.
    /// </summary>
    public class WandCylinder : MonoBehaviour
    {

        public VRPNTrack wandObject;
        public Material red;
        public Material green;
        public bool currentValue = false;

        private Renderer _renderer;

        /// <summary>
        /// Starts up and inits the ray to red.
        /// </summary>
        // Use this for initialization
        void Start()
        {
            currentValue = false;
            _renderer = GetComponent<Renderer>();
            _renderer.material = red;
            if(wandObject == null)
                Debug.LogError("Wand object must be set!");
        }


        /// <summary>
        /// Updates the ray to be red on no hit and green when an object is hit.
        /// </summary>
        void Update()
        {
            if (wandObject == null) return;
            bool rayHit = Physics.Raycast(wandObject.transform.position, wandObject.transform.forward);
            if (currentValue == rayHit) return;
            currentValue = rayHit;
            _renderer.material = currentValue ? green : red;
        }
    }
}