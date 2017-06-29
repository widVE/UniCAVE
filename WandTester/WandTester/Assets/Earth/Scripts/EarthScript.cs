/*
The MIT License(MIT)
Copyright(c) 2016 Digital Ruby, LLC
http://www.digitalruby.com

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using System.Collections.Generic;

namespace DigitalRuby.Earth
{
    public class EarthScript : SphereScript
    {
        [Range(-1000.0f, 1000.0f)]
        [Tooltip("Rotation speed")]
        public float RotationSpeedX = 0.0f;

        [Range(-1000.0f, 1000.0f)]
        [Tooltip("Rotation speed Y axis")]
        public float RotationSpeedY = 5f;

        [Range(-1000.0f, 1000.0f)]
        [Tooltip("Rotation speed Z axis")]
        public float RotationSpeedZ = 0.0f;

        protected override void Update()
        {
            base.Update();

#if UNITY_EDITOR

            if (Application.isPlaying)
            {

#endif

                transform.Rotate(RotationSpeedX * Time.deltaTime, Time.deltaTime * RotationSpeedY, RotationSpeedZ * Time.deltaTime);

#if UNITY_EDITOR

            }

#endif

        }
    }
}