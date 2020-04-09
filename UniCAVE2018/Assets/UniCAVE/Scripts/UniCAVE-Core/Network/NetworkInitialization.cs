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
using UnityEngine.Networking;

/// <summary>
/// Starts the program as either client or server depending on machine
/// </summary>
public class NetworkInitialization : MonoBehaviour {

    public NetworkManager networkManager;

    public string headMachine = "C6_V1_HEAD";

    [Tooltip("This can be overriden at runtime with parameter serverAddress, for example \"serverAddress 192.168.0.100\"")]
    public string serverAddress = "192.168.4.140";

    [Tooltip("This can be overriden at runtime with parameter serverPort, for example \"serverPort 8421\"")]
    public int serverPort = 7568;

    /// <summary>
    /// Starts as client or server
    /// </summary>
    void Start () {
        var serverArg = Util.GetArg("serverAddress");
        if(serverArg != null) {
            serverAddress = serverArg;
        }
        var portArg = Util.GetArg("serverPort");
        if(portArg != null) {
            int.TryParse(portArg, out serverPort);
        }

        Debug.Log("serverAddress = " + serverAddress + ", serverPort = " + serverPort + ", headMachine = " + headMachine + ", running machine = " + Util.GetMachineName());

        networkManager.networkAddress = serverAddress;
        networkManager.networkPort = serverPort;
#if !UNITY_EDITOR
        if ((Util.GetArg("forceClient") == "1") || (Util.GetMachineName() != headMachine)) {
            networkManager.StartClient();
        } else {
            networkManager.StartServer();
        }
#else
        networkManager.StartServer();
#endif
    }

    /// <summary>
    /// Quit after 20 seconds if no connection is made to server
    /// </summary>
    void Update() {
        if (Util.GetMachineName() != headMachine) {
            if(networkManager.client == null)
                networkManager.StartClient();

            if (Time.time > 20 && !networkManager.IsClientConnected()) {
                Application.Quit(); //kill it after 20 seconds if the client isn't connected, for convenience
            }
        }
    }
}
