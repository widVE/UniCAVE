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

public class NetworkingSync : MonoBehaviour {

    public string headNodeIP = "192.168.4.140";
    public int port = 7568;
    public int numSlaveNodes = 12;
    public float networkUpdatesPerSecond = 60.0f;
    private string machineName;
    private float myTimeScale = 1.0f;
    private float lastTime = 0.0f;
    private float headTime = 0.0f;
    private bool syncedRandomSeed = false;
    private bool actuallyConnected = false;
    private int connectionTries = 0;
    private const int MAX_CONNECTION_TRIES = 50;
    private int frameCount = 0;

	void Start () {
        machineName = System.Environment.MachineName;
        if (machineName == MasterTrackingData.HeadNodeMachineName)
        {
            Network.InitializeServer(numSlaveNodes, port, false);
        }
        else
        {
            Network.Connect(headNodeIP, port);
            connectionTries++;
		}
        
        Time.fixedDeltaTime = 0.05f;
        Network.sendRate = networkUpdatesPerSecond;

        Rigidbody[] gO = GameObject.FindObjectsOfType<Rigidbody>();
        foreach (Rigidbody rb in gO)
        {
            GameObject gameObject = rb.gameObject;
            gameObject.AddComponent<NetworkView>();
            NetworkView nv = gameObject.GetComponent<NetworkView>();
            nv.observed = rb;
        }
	}

    void OnServerInitialized()
    {
        Debug.Log("Server initialized and ready on " + System.Environment.MachineName);
    }
    
    void OnConnectedToServer()
    {
        Debug.Log(System.Environment.MachineName + " successfully connected to " + MasterTrackingData.HeadNodeMachineName);
        actuallyConnected = true;
    }

    void OnFailedToConnect(NetworkConnectionError error)
    {
        Debug.Log("Could not connect to server on " + MasterTrackingData.HeadNodeMachineName + ": " + error);
        machineName = System.Environment.MachineName;
        if (machineName != MasterTrackingData.HeadNodeMachineName && connectionTries < MAX_CONNECTION_TRIES && !actuallyConnected)
        {
            connectionTries++;
            Debug.Log("Retrying connection... attempt # " + connectionTries);
            Network.Connect(headNodeIP, port);
        }
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        if (Network.isServer)
            Debug.Log("Local server connection disconnected");
        else
            if (info == NetworkDisconnection.LostConnection)
                Debug.Log("Lost connection to the server");
        else
            Debug.Log("Successfully diconnected from the server");
    }

    void Update()
    {
        if (System.Environment.MachineName == MasterTrackingData.HeadNodeMachineName)
        {
            if (Input.inputString.Length > 0)
            {
                GetComponent<NetworkView>().RPC("sendKeys", RPCMode.Others, Input.inputString);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GetComponent<NetworkView>().RPC("quitApplication", RPCMode.Others);
                Application.Quit();
            }

            if (!syncedRandomSeed && frameCount > 500)
            {
                //don't sync this until all connections have occurred.
                if (Network.connections.Length == numSlaveNodes)
                {
                    GetComponent<NetworkView>().RPC("syncRandomSeed", RPCMode.Others, UnityEngine.Random.seed);
                    syncedRandomSeed = true;
                }
            }

            float t = Time.time;
            if (t - lastTime > 0.1)
            {
                GetComponent<NetworkView>().RPC("getTimeFromHeadnode", RPCMode.Others, t);
                lastTime = t;
            }

            frameCount++;
        }
    }

    [RPC]
    void quitApplication()
    {
        Application.Quit();
    }

    [RPC]
    void sendKeys(string keysPressed)
    {
        //this will be received on the slave end..
        Debug.LogError(keysPressed);
    }

    [RPC]
    void syncRandomSeed(int seed)
    {
        Debug.LogError("Synced random seed to " + seed);
        ParticleSystem[] gO = GameObject.FindObjectsOfType<ParticleSystem>();
        foreach (ParticleSystem ps in gO)
        {
            ps.Stop();
        }

        UnityEngine.Random.InitState(seed);

        foreach (ParticleSystem ps in gO)
        {
            ps.useAutoRandomSeed = false;
            ps.randomSeed = (uint)seed;
            ps.Simulate(0.0f, true, true, false);
            ps.Play();
        }
    }

    [RPC]
    void getTimeFromHeadnode(float globalTime)
    {
        //headTime = globalTime;
        //delta time should be time between we receive these calls...
        //float ourTime = Time.realtimeSinceStartup;
        if (lastTime == 0.0f)
        {
            lastTime = globalTime;
        }
        else
        {
            float ourTime = Time.time;
            //time diff is the time between syncs
            float timeDiff = globalTime - lastTime;
            //global time is the head node's time...
            float scale = ((globalTime - ourTime) + timeDiff) / timeDiff;

            //float scale = (1.0f + ((globalTime - ourTime) + Time.deltaTime)) / (Time.deltaTime + 1.0f);
            //float scale = ((globalTime - ourTime) + Time.unscaledDeltaTime) / Time.unscaledDeltaTime;

            lastTime = globalTime;

            if (scale < 0.0f)
            {
                scale = 0.001f;
            }
            else if (scale > 100.0f)
            {
                scale = 100.0f;
            }

            //myTimeScale = Mathf.Lerp(myTimeScale, scale, Time.deltaTime);
            Time.timeScale = scale;// myTimeScale;
            //Time.fixedDeltaTime = 
        }
    }
}
