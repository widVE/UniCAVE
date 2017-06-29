/*
The MIT License(MIT)
Copyright(c) 2016 Digital Ruby, LLC
http://www.digitalruby.com

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using UnityEngine;
using System.Collections;

namespace DigitalRuby.Earth
{
    public class MoveScript : MonoBehaviour
    {
        public float Speed = 10.0f;

        private void Start()
        {

        }

        private void Update()
        {
            float move = Input.GetAxis("Vertical");
            transform.position += (transform.forward * Speed * Time.deltaTime * move);
            move = Input.GetAxis("Horizontal");
            transform.position += (transform.right * Speed * Time.deltaTime * move);

            var pitch = Input.GetAxis("Mouse Y") * -150.0f * Time.deltaTime;
            var yaw = Input.GetAxis("Mouse X") * 150.0f * Time.deltaTime;
            var roll = (Input.GetMouseButton(1) ? yaw : 0.0f);
            if (roll != 0.0f)
            {
                yaw = 0.0f;
            }
            transform.Rotate(pitch, yaw, roll);
        }
    }
}