using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorTool : MonoBehaviour, ITool {
    GameObject wandObject;
    GameObject holder;
    RaycastHit refHit;
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
        if(refHit.collider == null)
        {
            refHit = hit;            
        }
        float distance = Vector3.Distance(origin, hit.transform.position);
        Vector3 point = origin + (direction * distance);
        
        if(Vector3.Distance(hit.transform.position, point) >= hit.transform.GetComponent<RectTransform>().rect.width/2)
            hit.transform.LookAt(point);


    }

    public void init()
    {
        throw new NotImplementedException();
    }

    public void shutDown()
    {
        refHit = new RaycastHit();
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
