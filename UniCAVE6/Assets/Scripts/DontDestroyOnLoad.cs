using UnityEngine;

/// <summary>
/// Objects with this script won't be destroyed when a new scene is loaded
/// </summary>
public class DontDestroyOnLoad : MonoBehaviour
{
    void Start()
    {
        Object.DontDestroyOnLoad(gameObject);
    }
}
