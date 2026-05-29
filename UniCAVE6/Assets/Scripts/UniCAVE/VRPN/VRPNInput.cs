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
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Threading;
#if UNITY_EDITOR
#endif

namespace UniCAVE
{
    /// <summary>
    /// Updated VRPNInput module with tool management.
    /// </summary>\
    [ExecuteInEditMode]
    public class VRPNInput : MonoBehaviour //: NetworkBehaviour
    {

        [SerializeField]
        private string trackerAddress = "WiiMote0@localhost";

        [SerializeField]
        private int channelHorizontal = 2;
        [SerializeField]
        private int channelVertical = 1;
        [SerializeField]
        private bool trackButton = true;
        [SerializeField]
        private bool trackAnalog = true;
        public float rotationSpeed = 0.05f;


        private bool _isServer;

        public TrackerButtonList trackerButtonList;
        public GameObject TopLevelUniCAVE;
        public GameObject wandObject = null;

        public bool debugOutput = false;
        public float movementSpeed = 0.01f;
        public double deadZone = 0.05;
        public Text tool;
        public GameObject canvas;
        public bool rotationMovement = false;
        public GameObject panel;
        public int maxButtons = 20;
        public float rayLength = 200;
        public bool negativeAnalogX = false;
        public bool negativeAnalogY = true;


        private RaycastHit _hit;
        private Vector3 _offset;
        private ToolManager _toolManager;
        private int _lastButtonPressed = -1;
        private Dictionary<TrackerButton, bool> _buttonState = new();

        private const int MaxLoopsButtonCheck = 20;
        private const int SleepTimeout = 100;

        [SerializeField] MachineName headMachineAsset;
        public string HeadMachine => MachineName.GetMachineName(headMachineAsset);
        


        public bool TrackButton
        {
            get => trackButton;
            set
            {
                trackButton = value;
                StopCoroutine("Button");
                if(trackButton && Application.isPlaying)
                {
                    StartCoroutine("Button");
                }
            }
        }

        public bool TrackAnalog
        {
            get => trackAnalog;
            set
            {
                trackAnalog = value;
                StopCoroutine("Analog");
                if(trackAnalog && Application.isPlaying)
                {
                    StartCoroutine("Analog");
                }
            }
        }


        /// <summary>
        /// Handles the startup
        /// </summary>
        private void Start()
        {
            // check if application is running
            if (!Application.isPlaying)
            {
                return;
            }


            if(trackerButtonList == null)
            {
                trackerButtonList = GetComponent<TrackerButtonList>();
            }

            if(trackerButtonList == null)
            {
                gameObject.AddComponent(typeof(TrackerButtonList));
                trackerButtonList = GetComponent<TrackerButtonList>();
            }
            //Add a toolManager to the wandObject to shuffle between tools
            _toolManager = new ToolManager(wandObject, gameObject, TopLevelUniCAVE, deadZone, rotationSpeed, movementSpeed, tool, negativeAnalogX, negativeAnalogY);
            //add state of each button
            foreach(TrackerButton btn in Enum.GetValues(typeof(TrackerButton)))
            {
                _buttonState.Add(btn, false);
            }

            
            // TODO: there is need to set the flag as True because even if it is server it is destoyed
            //if(!_isServer)
            if (Util.GetMachineName() != HeadMachine)
            {
                UnityEngine.Object.Destroy(this);
                return;
            }

            // TODO: testing in editor, uncommend later
//#if !UNITY_EDITOR
            //Start the coroutines
            if (trackButton)
                StartCoroutine(nameof(VRPNInput.Button));

            if (trackAnalog)
                StartCoroutine(nameof(VRPNInput.Analog));
//#endif

        }


        /// <summary>
        /// Asynchronous method taking in button input and sending it to the current selected tool
        /// </summary>
        /// <returns></returns>
        private IEnumerator Button()
        {
            int maxButtons = trackerButtonList.GetMaxButtons();

            while(true)
            {
                Vector3 origin = wandObject.transform.position;
                Vector3 direction = wandObject.transform.forward;

                //i tracks the number of the current button
                for(int i = 0; i < maxButtons; ++i)
                {
                    //Current value of the button
                    bool curValue = VRPN.vrpnButton(trackerAddress, i);
                    TrackerButton currentButton = trackerButtonList.MapButton(i);

                    //If the previous state is true and the current value is false it is a button click
                    if(_buttonState.ContainsKey(currentButton) && _buttonState[currentButton] && !curValue)
                    {
                        //Fire the event
                        _toolManager.handleButtonClick(currentButton, origin, direction);

                        //hasStarted = false;
                        _hit = new RaycastHit(); //temp;

                    }
                    //If the current and previous are true then it is a buttondrag event
                    else if(_buttonState.ContainsKey(currentButton) && _buttonState[currentButton] && curValue && (currentButton == TrackerButton.Trigger))
                    {
                        if(_hit.distance > 0)
                        {
                            //Fire the event
                            _toolManager.handleButtonDrag(currentButton, _hit, _offset, origin, direction);
                        }
                    }
                    //If the previous is false and the current is true 
                    else if(!(_buttonState.ContainsKey(currentButton) && _buttonState[currentButton]) && curValue && (currentButton == TrackerButton.Trigger))
                    {
                        Physics.Raycast(origin, direction * rayLength, out _hit);
                        if(_hit.distance > 0)
                            _offset = _hit.transform.position - _hit.point;

                        _toolManager.handleButtonDown(currentButton, _hit, origin, direction);
                    }

                    //update the previous value
                    _buttonState[currentButton] = curValue;
                }
                yield return null;
            }
        }

        /// <summary>
        /// Asynchronous method that handles all the input from the analog stick
        /// </summary>
        /// <returns></returns>
        private IEnumerator Analog()
        {
            while(true)
            {
                //Get the X and Y values from the joystick
                double analogVertical = VRPN.vrpnAnalog(trackerAddress, channelVertical);
                double analogHorizontal = VRPN.vrpnAnalog(trackerAddress, channelHorizontal);

                //Translate the holder
                _toolManager.handleAnalog(analogHorizontal, analogVertical);
                yield return null;
            }
        }

        /// <summary>
        /// Gets the latest pushed button on tracker from vrpn. Stops after a preset amount of time (2 seconds currently).
        /// </summary>
        /// <returns>The number of the pushed button (0...n-1) or -1 if no button is pushed on tracker.</returns>
        public int GetPushedButton()
        {
            _lastButtonPressed = -1;
            for(int ii = 0; ii < VRPNInput.MaxLoopsButtonCheck; ii++)
            {
                for(int jj = 0; jj < maxButtons; jj++)
                {
                    bool btnValue = VRPN.vrpnButton(trackerAddress, jj, ii);
                    if(btnValue)
                        _lastButtonPressed = jj;

                }
                if(_lastButtonPressed > -1)
                {
                    return _lastButtonPressed;
                }
                Thread.Sleep(VRPNInput.SleepTimeout);
            }
            return -1;

        }

    }
}