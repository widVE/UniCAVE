using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkInitialization : MonoBehaviour {

    public NetworkManager networkManager;

    public string headMachine = "C6_V1_HEAD";

    [Tooltip("This can be overriden at runtime with parameter serverAddress, for example \"serverAddress 192.168.0.100\"")]
    public string serverAddress = "192.168.4.140";

    [Tooltip("This can be overriden at runtime with parameter serverPort, for example \"serverPort 8421\"")]
    public int serverPort = 7568;

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
        
        if (Util.GetMachineName() == headMachine) {
            networkManager.StartServer();
        } else {
            networkManager.StartClient();
        }
    }

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
