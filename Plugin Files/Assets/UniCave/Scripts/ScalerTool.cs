using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalerTool : MonoBehaviour, ITool {
    GameObject wandObject;
    GameObject holder;


    public void Analog(double x, double y)
    {
        throw new NotImplementedException();
    }

    public void ButtonClick(int button, Vector3 origin, Vector3 direction, bool cave)
    {
        //throw new NotImplementedException();
    }

    public void ButtonDrag(RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction)
    {
        if (origin == null)
        {
            origin = hit.point;
        }
        Vector3 position = wandObject.transform.position;
        Vector3 objectScale = hit.transform.localScale;
        Vector3 a = position - origin;
        float magnitude = Vector3.Magnitude(a);
        float dotProduct = Vector3.Dot(direction, a);
        float scale = magnitude * dotProduct / 2;

        if(objectScale.x + scale > 0 && objectScale.y + scale > 0 && objectScale.z + scale > 0 )
        {
            hit.transform.localScale = new Vector3(objectScale.x + scale, objectScale.y + scale, objectScale.z + scale);
        }
        

    }

    public void init()
    {
        throw new NotImplementedException();
    }

    public void shutDown()
    {
        //throw new NotImplementedException();
    }

    // Use this for initialization
    void Start () {
        //Get all necessary game objects
        if (wandObject == null)
        {
            wandObject = GameObject.Find("Wand");
        }

        if (holder == null)
        {
            holder = GameObject.Find("C4_Seq_4Displays_1PC_CAES");
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
