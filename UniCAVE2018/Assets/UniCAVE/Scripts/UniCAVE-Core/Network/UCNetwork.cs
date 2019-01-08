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
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;


[NetworkSettings(channel = 1, sendInterval = 0.016f)]
public class UCNetwork : NetworkBehaviour {
    
    [Tooltip("This object will be transformed by this script")]
    public HeadConfiguration head;

    [TextArea(20, 20)]
    public string launchScript;

    private float lastTime = 0.0f;
	private bool syncedRandomSeed = false;
	private int frameCount = 0;
	private int numSlaveNodes = 12;
	
    void Update() {
        if (isServer) {
            if(Input.GetKeyDown(KeyCode.Escape)) {
                RpcQuitApplication();
				Application.Quit();
            }

            if (head != null)
                RpcSetTransforms(transform.position, transform.rotation, head.transform.position, head.transform.rotation);
            else
                RpcSetTransforms(transform.position, transform.rotation, Vector3.zero, Quaternion.identity);
			
			RpcSetTime(Time.time);
			
			if (!syncedRandomSeed && frameCount > 600)
            {
                //don't sync this until all connections have occurred.
				/*NetworkManager m = gameObject.transform.parent.GetComponent<NetworkManager>();
				if(m != null)
				{
					if (m.connections.Length == numSlaveNodes)
					{*/
						RpcSetRandomSeed(UnityEngine.Random.seed);
						syncedRandomSeed = true;
					//}
				//}
            }
			
			frameCount++;
        }
    }

    [ClientRpc]
    void RpcSetTransforms(Vector3 myPos, Quaternion myOri, Vector3 headPos, Quaternion headOri) {
        transform.SetPositionAndRotation(myPos, myOri);
        if (head != null)
            head.transform.SetPositionAndRotation(headPos, headOri);
    }

    [ClientRpc]
    void RpcSetTime(float canonicalTime) {
        if (lastTime == 0.0f) {
            lastTime = canonicalTime;
        } else {
            float ourTime = Time.time;
            float timeDiff = canonicalTime - lastTime;
            float scale = ((canonicalTime - ourTime) + timeDiff) / timeDiff;
            
            lastTime = canonicalTime;

            if (scale < 0.0f) {
                scale = 0.01f;
            } else if (scale > 100.0f) {
                scale = 100.0f;
            }
            
            Time.timeScale = scale;
        }
    }

    [ClientRpc]
    void RpcSetRandomSeed(int seed) {
        Debug.LogError("Synced random seed to " + seed);
        ParticleSystem[] particleSystems = FindObjectsOfType<ParticleSystem>();
        foreach (ParticleSystem ps in particleSystems) {
            ps.Stop();
        }

        Random.InitState(seed);

        foreach (ParticleSystem ps in particleSystems) {
            ps.useAutoRandomSeed = false;
            ps.randomSeed = (uint)seed;
            ps.Simulate(0.0f, true, true, false);
            ps.Play();
        }
    }

    [ClientRpc]
    void RpcQuitApplication() {
        Application.Quit();
    }

    
    [Tooltip("You can load PhysicalDisplay settings for all children recursively, right click the name of this script and settings will be loaded from this file path")]
    public string settingsToLoad;
    [ContextMenu("Load Settings For All Children")]
    void LoadSettingsChildren() {
        LoadSettingsChildren_h(null);
    }
    void LoadSettingsChildren_h(GameObject it = null) {
        if (it == null) it = gameObject;

        for (int i = 0; i < it.transform.childCount; i++) {
            GameObject child = it.transform.GetChild(i).gameObject;
            if (child.GetComponent<PhysicalDisplay>() != null) {
                child.GetComponent<PhysicalDisplay>().TryToDeSerialize(settingsToLoad);
            }
            LoadSettingsChildren_h(child);
        }
    }
    
    string GenerateLaunchScript() {
        List<PhysicalDisplay> displays = new List<PhysicalDisplay>();
        List<PhysicalDisplayManager> managers = new List<PhysicalDisplayManager>();
        IterateAllRelevantChildren(gameObject, displays, managers);

        string res = "# Windows Powershell Launch Script\n";
        res += "# Script Generated On " + System.DateTime.Now.ToLongDateString() + ", " + System.DateTime.Now.ToLongTimeString() + "\n";
        res += "# Setup contains " + displays.Count + " displays and " + managers.Count + " display managers";

        for (int i = 0; i < displays.Count; ++i) {
            if (displays[i].manager != null) continue;

            res += "\n\n# Display: " + displays[i].name;
            res += "\nIf ($env:ComputerName -eq \"" + displays[i].machineName + "\") {";
            if(displays[i].dualPipe && displays[i].dualInstance) {
                for (int j = 0; j < 2; j++) {
                    res += "\n\t" + Application.productName + ".exe";
                    res += " " + (displays[i].exclusiveFullscreen ? "-screen-fullscreen 1 -adapter " + displays[i].display : "-screen-fullscreen 0 -popupwindow");
                    res += " " + ((displays[i].is3D && !displays[i].dualPipe) ? "-vrmode stereo" : "");
                    res += " " + "eye " + (j == 0 ? "left" : "right");
                }
            } else {
                res += "\n\t" + Application.productName + ".exe";
                res += " " + (displays[i].exclusiveFullscreen ? "-screen-fullscreen 1 -adapter " + displays[i].display : "-screen-fullscreen 0 -popupwindow");
                res += " " + ((displays[i].is3D && !displays[i].dualPipe) ? "-vrmode stereo" : "");
            }
            
            res += "\n}";
        }

        for(int i = 0; i < managers.Count; i++) {
            res += "\n\n# Display Group: " + managers[i].name;
            res += "\nIf ($env:ComputerName -eq \"" + managers[i].machineName + "\") {";
            
            res += "\n\t" + Application.productName + ".exe";
            res += " " + (displays[i].exclusiveFullscreen ? "-screen-fullscreen 1 -adapter " + displays[i].display : "-screen-fullscreen 0 -popupwindow");
            res += " " + ((displays[i].is3D && !displays[i].dualPipe) ? "-vrmode stereo" : "");

            res += "\n}";
        }

        return res;
    }
    private void IterateAllRelevantChildren(GameObject it, List<PhysicalDisplay> displays, List<PhysicalDisplayManager> managers) {
        for (int i = 0; i < it.transform.childCount; i++) {
            GameObject child = it.transform.GetChild(i).gameObject;
            if (child.GetComponent<PhysicalDisplay>() != null) {
                displays.Add(child.GetComponent<PhysicalDisplay>());
            } else if(child.GetComponent<PhysicalDisplayManager>()) {
                managers.Add(child.GetComponent<PhysicalDisplayManager>());
            }
            IterateAllRelevantChildren(child, displays, managers);
        }
    }

    void OnValidate() {
        launchScript = GenerateLaunchScript();
    }
}
