using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Synchronizes scene loading between machines in the render network.
/// </summary>
public class UnicaveSceneManager : NetworkBehaviour
{
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            startingScene = SceneAsset.name;
        }
    }
#endif

    /// <summary>
    /// Path of the scene to load when the object is spawned.
    /// </summary>
    [SerializeField] private string startingScene;
    
    /// <summary>
    /// The scene that is currently loaded.
    /// </summary>
    private Scene _loadedScene;

    public bool SceneIsLoaded => _loadedScene.IsValid() && _loadedScene.isLoaded;

    /// <summary>
    /// Singleton instance.
    /// </summary>
    private static GameObject _instance = null;

    /// <summary>
    /// Singleton check: if another instance exists, destroy this one.
    /// </summary>
    private void Awake()
    {
        if (UnicaveSceneManager._instance)
        {
            UnityEngine.Object.Destroy(gameObject);
        }
        else
        {
            UnicaveSceneManager._instance = gameObject;
        }
    }

    /// <summary>
    /// When started, if a starting scene is specified and we're the server, tell the clients to load the scene.
    /// </summary>
    public override void OnNetworkSpawn()
    {
        if (IsServer && !string.IsNullOrEmpty(startingScene))
        {
            NetworkManager.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
            SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene(startingScene, LoadSceneMode.Single);
            CheckStatus(status);
        }

        base.OnNetworkSpawn();
    }

    /// <summary>
    /// Loads a scene as a single scene on the server (previous will be unloaded) and notify clients to load it.
    /// </summary>
    /// <param name="sceneName">scene name</param>
    public void LoadScene(string sceneName)
    {
        GameObject caveGameObject = GameObject.Find("CAVE");
        caveGameObject.GetComponent<Transform>().position = new Vector3(0, 0, 0);
        caveGameObject.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, 0);
        ResetPositionAndRotationClientRpc();

        // Only server loads a scene
        if (!IsServer || string.IsNullOrEmpty(sceneName)) return;
        SceneEventProgressStatus status = NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        CheckStatus(status);

        // Remove cameras other than UniCAVE's created "eye" cameras to prevent them overriding the UniCAVE cameras.
        foreach (Camera cam in UnityEngine.Object.FindObjectsOfType<Camera>(true))
        {
            cam.enabled = cam.name.Contains("Eye");
        }
    }

    [ClientRpc]
    private void ResetPositionAndRotationClientRpc()
    {
        GameObject caveGameObject = GameObject.Find("CAVE");
        caveGameObject.GetComponent<Transform>().position = new Vector3(0, 0, 0);
        caveGameObject.GetComponent<Transform>().eulerAngles = new Vector3(0, 0, 0);
    }

    /// <summary>
    /// Loads the starting scene.
    /// </summary>
    public void LoadStartingScene()
    {
        LoadScene(startingScene);
    }

    /// <summary>
    /// Unloads the current scene on the server and tells all clients to do so.
    /// </summary>
    public void UnloadScene()
    {
        if (!IsServer || !IsSpawned || !_loadedScene.IsValid() || !_loadedScene.isLoaded)
        {
            Debug.LogWarning("Trying to unload scene that is not loaded!");
            return;
        }

        // Unload the scene
        SceneEventProgressStatus status = NetworkManager.SceneManager.UnloadScene(_loadedScene);
        CheckStatus(status, false);
    }

    /// <summary>
    /// Checks a status value returned from a <c>NetworkSceneManager</c> operation and logs an error if it is
    /// not successfully started.
    /// </summary>
    /// <param name="status">The status value returned by the Unity <c>NetworkSceneManager</c> operation.</param>
    /// <param name="isLoading">Whether the error message logged should refer to loading or unloading.</param>
    private void CheckStatus(SceneEventProgressStatus status, bool isLoading = true)
    {
        string sceneEventAction = isLoading ? "load" : "unload";
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to {sceneEventAction} {startingScene} with a {nameof(SceneEventProgressStatus)}: {status}");
        }
    }

    /// <summary>
    /// Delegate for events from Unity NetworkSceneManager on server. Doesn't run on clients (delegate is never added)
    /// but receives events from clients. Other than debug messages, just updates <c>_loadedScene</c>.
    /// </summary>
    /// <param name="sceneEvent">The event that triggered the delegate.</param>
    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        string clientOrServer;

        if (sceneEvent.ClientId == NetworkManager.ServerClientId)
        {
            clientOrServer = "server";
        }
        else
        {
            clientOrServer = "client";
        }

        switch (sceneEvent.SceneEventType)
        {
            case SceneEventType.LoadComplete:
                {
                    // We want to handle this for only the server-side
                    if (sceneEvent.ClientId == NetworkManager.ServerClientId)
                    {
                        // *** IMPORTANT ***
                        // Keep track of the loaded scene, needed to unload it later
                        _loadedScene = sceneEvent.Scene;
                    }
                    Debug.Log($"Loaded the {sceneEvent.SceneName} scene on {clientOrServer}-({sceneEvent.ClientId}).");

                    break;
                }
            case SceneEventType.UnloadComplete:
                {
                    Debug.Log($"Unloaded the {sceneEvent.SceneName} scene on {clientOrServer}-({sceneEvent.ClientId}).");
                    break;
                }
            case SceneEventType.LoadEventCompleted:
            case SceneEventType.UnloadEventCompleted:
                {
                    string loadUnload = sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted ? "Load" : "Unload";
                    Debug.Log($"{loadUnload} event completed for the following client identifiers:({sceneEvent.ClientsThatCompleted})");
                    if (sceneEvent.ClientsThatTimedOut.Count > 0)
                    {
                        Debug.LogWarning($"{loadUnload} event timed out for the following client identifiers:({sceneEvent.ClientsThatTimedOut})");
                    }
                    break;
                }
        }
    }
}
