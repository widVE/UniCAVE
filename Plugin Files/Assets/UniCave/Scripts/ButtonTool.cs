using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTool : MonoBehaviour, ITool {
    public Canvas canvas;
    RaycastHit hit;
    public GameObject wandObject;
    
    public void Analog(double x, double y)
    {
        throw new NotImplementedException();
    }

    public void ButtonClick(int buttonNum, Vector3 origin, Vector3 direction)
    {
        Physics.Raycast(origin, direction, out hit);
        //float distance = (canvas.transform.position - origin).magnitude;
        //hitInput = Physics2D.Raycast(origin, direction, distance + 10, Physics2D.DefaultRaycastLayers, distance - 1, distance + 1);
        //if(hitInput)
        //    Debug.Log("I am being hit button name " + hitInput.rigidbody.name);
        // Debug.DrawRay(origin, direction);
        //if (hitInput!=null)
        //{
        //hitInput.collider != null && hitInput.collider.tag == "button"

        if (hit.collider != null && hit.collider.tag == "button")
        {
            Button button = hit.transform.gameObject.GetComponent<Button>();
            //EventSystem.current.SetSelectedGameObject(button.gameObject);
            if (buttonNum == 3)
            {
                button.onClick.Invoke();
            }
                
        }

        else if (hit.collider != null && hit.collider.tag == "slider")
        {
            //float distanceFromWand = canvas.transform.position.z - origin.z;
            Slider s = hit.transform.gameObject.GetComponent<Slider>();
            //canvas = canvas.GetComponent<Canvas>();


            //Debug.Log(s.transform.localPosition);
            //Debug.Log("World Position " + s.transform.position);
            //Debug.Log("Local Position " + s.transform.localPosition);
            float width = s.GetComponent<RectTransform>().rect.width;
            //Vector3 left = s.transform.Translate(new Vector3(width / 2, 0, 0));
            Vector3 sliderMiddle = s.transform.TransformPoint(s.transform.TransformPoint(s.transform.localPosition));

            Vector3 sliderRight = canvas.transform.TransformPoint(transform.TransformPoint(new Vector3(0, 0, 0)));
            Vector3 sliderLeft = s.transform.TransformPoint(s.transform.TransformPoint(new Vector3(s.transform.localPosition.x - width / 2, s.transform.localPosition.y, s.transform.localPosition.z)));
            float sliderWidth = sliderRight.x - sliderLeft.x;

            Debug.Log("slider left " + sliderLeft);
            Debug.Log("slider right " + sliderRight);
            Debug.Log("slider middle " + sliderMiddle);



            //Debug.Log("Test " + sliderPos);
            //float sliderMiddle = s.transform.localPosition.x;
            //float sliderLeft = s.transform.localPosition.x - (s.GetComponent<RectTransform>().rect.width/2);
            //float sliderRight = s.transform.localPosition.x + (s.GetComponent<RectTransform>().rect.width / 2);
            //Debug.Log("X values of the slider " + sliderLeft + " " + sliderMiddle + " " + sliderRight);

            //sliderPos = transform.TransformDirection(sliderLeft, s.transform.localPosition.y, s.transform.localPosition.z);
            //Debug.Log(sliderPos);
            if (buttonNum == 3)
            {
                float point = Math.Abs(hit.point.x - sliderLeft.x) / sliderWidth;
                //float pos = point / sliderWidth;
                if (hit.point.x > sliderMiddle.x)
                {
                    s.value = point;
                }
                else
                {
                    s.value = 1 - point;
                }




                //float width = s.GetComponent<Renderer>().bounds.size.x; //s.GetComponent<RectTransform>().rect.width * .0042f;
                float middle = s.transform.position.x;
                float distanceFromZero;
                float value;

                //if (middle > 0)
                //{
                //    //distanceFromZero = middle - width / 2f;
                //}
                //else
                //{
                //   // distanceFromZero = middle + width / 2f;
                //}

                ////width = (width * CanvasScaler);
                ////Do the slider dragging here
                ////Width of slider = .66
                //Debug.Log(hit.point.x);
                ////value = Math.Abs((distanceFromZero - (hit.point.x))) / (width);
                //if (distanceFromZero > 0)
                //{
                //    value = (Math.Abs(hit.point.x) - Math.Abs(distanceFromZero)) / width;
                //    s.value = value;
                //}
                //else
                //{
                //    value = (Math.Abs(hit.point.x) - Math.Abs(distanceFromZero)) / width;
                //    s.value = 1 - value;
                //}

            }

        }

        else
        {
            //EventSystem.current.SetSelectedGameObject(null);
        }
    }

    public void ButtonDrag(RaycastHit hit_, Vector3 offset, Vector3 origin, Vector3 direction)
    {
        Physics.Raycast(origin, direction, out hit);
        if (hit_.collider.tag == "handle")
        {
            hit_.transform.position = hit.point;
            //do nothing
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
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
