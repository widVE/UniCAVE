using UnityEngine;
using System.Collections;

//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

public class networkingSync : MonoBehaviour {

    public string headNodeIP;
    public int port;
    public int numSlaveNodes;
    public float networkUpdatesPerSecond = 60.0f;
    private string machineName;
    private float myTimeScale = 1.0f;
    private float lastTime = 0.0f;
    private float headTime = 0.0f;
    private bool syncedRandomSeed = false;
    private int frameCount = 0;
	
	void Start () {
        machineName = System.Environment.MachineName;
        if (machineName == MasterTrackingData.HeadNodeMachineName)
        {
            Debug.Log("Initializing server on " + machineName);
            Network.InitializeServer(numSlaveNodes, port, false);
        } else{
            Network.Connect(headNodeIP, port);
            Debug.Log(machineName + " connecting");
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

    void Update()
    {
        if (System.Environment.MachineName == MasterTrackingData.HeadNodeMachineName)
        {
            if(Input.inputString.Length > 0)
            {
                GetComponent<NetworkView>().RPC("sendKeys", RPCMode.Others, Input.inputString);
            }

            if (!syncedRandomSeed && frameCount > 500)
            {
                GetComponent<NetworkView>().RPC("syncRandomSeed", RPCMode.Others, UnityEngine.Random.seed);
                syncedRandomSeed = true;
            }
            
            float t = Time.time;
            if (t - lastTime > 0.1)
            {
                GetComponent<NetworkView>().RPC("getTimeFromHeadnode", RPCMode.Others, t);
                lastTime = t;
            }

            frameCount++;
        }
        /*else 
        {
            if (lastTime == 0.0f)
            {
                lastTime = headTime;
            }
            else
            {
                float ourTime = Time.time;
                //time diff is the time between syncs
                float timeDiff = headTime - lastTime;
                if (timeDiff > 0.0f)
                {
                    //global time is the head node's time...
                    float scale = ((headTime - ourTime) + timeDiff) / timeDiff;

                    //float scale = (1.0f + ((globalTime - ourTime) + Time.deltaTime)) / (Time.deltaTime + 1.0f);
                    //float scale = ((globalTime - ourTime) + Time.unscaledDeltaTime) / Time.unscaledDeltaTime;

                    lastTime = headTime;

                    if (scale < 0.0f)
                    {
                        scale = 0.001f;
                    }
                    else if (scale > 100.0f)
                    {
                        scale = 100.0f;
                    }

                    myTimeScale = Mathf.Lerp(myTimeScale, scale, Time.deltaTime);
                    Time.timeScale = myTimeScale;// scale;// myTimeScale;
                }
            }
        }*/
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
