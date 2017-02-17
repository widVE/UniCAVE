using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (unicaveInput.Instance().GetButtonDown("customd"))
        {
            Debug.LogError("D!");
        }
        if (unicaveInput.Instance().GetButtonDown("customa"))
        {
            Debug.LogError("A!");
        }
    }
}