using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandCylinder : MonoBehaviour {

    public GameObject wandObject;
    public Material red;
    public Material green;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
      
        if (Physics.Raycast(wandObject.transform.position, wandObject.transform.forward))
            this.GetComponent<Renderer>().material = green;
        else
            this.GetComponent<Renderer>().material = red;
    }
}
