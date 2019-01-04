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


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used to put multiple displays in one instance of the program
//Start in full screen if you use this
public class PhysicalDisplayManager : MonoBehaviour {

    public string machineName;
    public List<PhysicalDisplay> displays = new List<PhysicalDisplay>();

    public bool ShouldBeActive() {
        return machineName == Util.GetMachineName();
    }

	// Use this for initialization
	void Start () {
		if(!ShouldBeActive()) {
            Debug.Log("Deactivating Display Manager: " + gameObject.name);
            gameObject.SetActive(false);
            return;
        }
        Debug.Log("Display Manager Active: " + gameObject.name);
    }

    // Update is called once per frame
    bool _initialized = false;
	void Update () {
		if(!_initialized) {
            bool displaysInitialized = true;
            for(int i = 0; i < displays.Count; i++) {
                if (displays[i].gameObject.activeSelf && displays[i].enabled && !displays[i].Initialized()) {
                    displaysInitialized = false;
                    break;
                }
                PhysicalDisplayCalibration cali = displays[i].gameObject.GetComponent<PhysicalDisplayCalibration>();
                if(cali != null && !cali.Initialized()) {
                    displaysInitialized = false;
                    break;
                }
            }
            //wait until every other display is initialized

            if(displaysInitialized) {
                for(int i = 0; i < displays.Count; i++) {
                    PhysicalDisplay display = displays[i];

                    PhysicalDisplayCalibration cali = display.gameObject.GetComponent<PhysicalDisplayCalibration>();
                    if (cali == null) {
                        if (!display.exclusiveFullscreen) { //do nothing if fullscreen
                            if (!display.useRenderTextures) {
                                if (display.dualPipe) {
                                    Vector2Int windowSpaceOffset = display.dualInstance ? new Vector2Int(0, 0) : new Vector2Int(display.windowBounds.x, display.windowBounds.y);
                                    display.leftCam.pixelRect = new Rect(
                                        windowSpaceOffset.x + display.leftViewport.x,
                                        windowSpaceOffset.y + display.leftViewport.y,
                                        display.leftViewport.width,
                                        display.leftViewport.height);
                                    display.rightCam.pixelRect = new Rect(
                                        windowSpaceOffset.x + display.rightViewport.x,
                                        windowSpaceOffset.y + display.rightViewport.y,
                                        display.rightViewport.width,
                                        display.rightViewport.height);
                                } else {
                                    foreach (Camera cam in display.GetAllCameras()) {
                                        cam.pixelRect = new Rect(display.windowBounds.x, display.windowBounds.y, display.windowBounds.width, display.windowBounds.height);
                                    }
                                }
                            }
                        }
                    } else {
                        //special case for PhysicalDisplayCalibration
                        Debug.Log("Display:");
                        foreach (Camera cam in cali.postCams) {
                            cam.pixelRect = new Rect(display.windowBounds.x, display.windowBounds.y, display.windowBounds.width, display.windowBounds.height);
                        }
                    }
                }



                _initialized = true;
            }
        }
	}

    [ContextMenu("Assign This Manager to Children")]
    void AssignManagerChildren() {
        AssignManagerChildren_h(null);
    }

    void AssignManagerChildren_h(GameObject it = null) {
        if (it == null) it = gameObject;

        for (int i = 0; i < it.transform.childCount; i++) {
            GameObject child = it.transform.GetChild(i).gameObject;
            PhysicalDisplay disp = child.GetComponent<PhysicalDisplay>();
            if (disp != null) {
                if(disp.manager != null) {
                    disp.manager.displays.Remove(disp);
                }
                displays.Remove(disp);
                disp.manager = this;
                displays.Add(disp);
            }
            AssignManagerChildren_h(child);
        }
    }
}
