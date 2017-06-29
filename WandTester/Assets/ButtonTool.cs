using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ButtonTool : MonoBehaviour, ITool
{
    public GameObject canvas;
    public Canvas c;
    RaycastHit hit;
    public GameObject wandObject;
    public GameObject holder;
    bool hide = true;
    public Text txt;

    public float point;
    int click;
    int buttonDrag;


    public void Analog(double x, double y)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// Handles the UI button click interactions 

    /// </summary>
    /// <param name="buttonNum"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>

    public void ButtonClick(int buttonNum, Vector3 origin, Vector3 direction, bool cave)
    {
        if (cave)
        {
            click = 2;
        }
        else
        {
            click = 3;
        }

        Physics.Raycast(origin, direction, out hit);

        if (buttonNum == click)
        {
            //If the object is a dropdown show or hide the menu
            if (hit.collider != null && hit.transform.GameObject.GetComponent<Dropdown>() != null)
            {
                Dropdown dropdown = hit.transform.gameObject.GetComponent<Dropdown>();
                Debug.Log(dropdown.value);

                if (hide)
                {
                    dropdown.Show();
                    hide = false;
                }
                else
                {
                    dropdown.Hide();
                    hide = true;
                }
            }

        }

        else if (hit.collider != null && hit.transform.gameObject.GetComponent<Toggle>() != null)
        {
            Toggle t = hit.transform.gameObject.GetComponent<Toggle>();
            Dropdown d = t.GetComponentInParent<Dropdown>();

            int number = t.name[5] - 48;

            d.value = number;
            d.RefreshShownValue();
            d.Hide();
            hide = true;

            if (buttonNum == 3)
            {
                Debug.Log(t);
            }

        }

        else if (hit.collider != null && hit.transform.gameObject.GetComponent<Button>() != null)
        {
            Button button = hit.transform.gameObject.GetComponent<Button>();
            //EventSystem.current.SetSelectedGameObject(button.gameObject);
            if (buttonNum == 3)
            {
                button.onClick.Invoke();
            }

        }

        else
        {
            //EventSystem.current.SetSelectedGameObject(null);
        }
    }


    public void ButtonDrag(RaycastHit hit_, Vector3 offset, Vector3 origin, Vector3 direction)
    {
        Debug.Log(hit_.transform.gameObject.GetType());
        //Check the type of the object to know what to slide
        if (hit_.transform.GameObject.GetComponent<Slider>() != null)
        {
            slide(hit_.transform.gameObject.GetComponent<Slider>(), origin, direction);
        }
        else if (hit_.transform.GameObject.GetComponent<Scrollbar>() != null)
        {
            slide(hit_.transform.gameObject.GetComponent<Scrollbar>(), origin, direction);
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
    void Start()
    {
        //Get all necessary game objects
        if (wandObject == null)
        {
            wandObject = GameObject.Find("Wand");
        }

        if (holder == null)
        {
            holder = GameObject.Find("WALL");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void slide(Scrollbar s, Vector3 origin, Vector3 direction)
    {
        //Get the canvas component
        Physics.Raycast(origin, direction, out hit);
        canvas = GameObject.Find("Canvas");
        c = canvas.GetComponent<Canvas>();

        //Get the dimensions in the canvas space

        Vector3 position = s.transform.localPosition;
        float width = s.GetComponent<RectTransform>().rect.width;
        float height = s.GetComponent<RectTransform>().rect.height;


        Vector3 sliderMiddle = s.transform.TransformPoint(s.transform.TransformPoint(position));
        Vector3 sliderRight = c.transform.TransformPoint(new Vector3(position.x + width / 2, position.y, position.z));
        Vector3 sliderLeft = c.transform.TransformPoint(new Vector3(position.x - width / 2, position.y, position.z));

        Vector3 sliderTop = c.transform.TransformPoint(new Vector3(position.x, position.y + height / 2, position.z));
        Vector3 sliderBottom = c.transform.TransformPoint(new Vector3(position.x, position.y - height / 2, position.z));

        float sliderWidth = Math.Abs(sliderRight.x - sliderLeft.x);
        float sliderHeight = Math.Abs(sliderTop.y - sliderBottom.y);


        float point;
        if (s.direction == Scrollbar.Direction.LeftToRight)
        {

            if (hit.point.x > sliderMiddle.x)
            {
                //Get the dimensions of the slider or scrollbar in worldspace 
                Vector3 sliderMiddle = s.transform.TransformPoint(s.transform.TransformPoint(position));
                Vector3 sliderRight = c.transform.TransformPoint(new Vector3(position.x + width / 2, position.y, position.z));
                Vector3 sliderLeft = c.transform.TransformPoint(new Vector3(position.x - width / 2, position.y, position.z));
                Vector3 sliderTop = c.transform.TransformPoint(new Vector3(position.x, position.y + height / 2, position.z));
                Vector3 sliderBottom = c.transform.TransformPoint(new Vector3(position.x, position.y - height / 2, position.z));
                //Get the dimensions in the worldspace
                float sliderWidth = Math.Abs(sliderRight.x - sliderLeft.x);
                float sliderHeight = Math.Abs(sliderTop.y - sliderBottom.y);

                //Check the direction of the scroll bar
                if (s.direction == Scrollbar.Direction.LeftToRight)
                {
                    if (hit.point.x > sliderMiddle.x)
                    {
                        //Calculate the value depending on the direction
                        point = (hit.point.x - sliderMiddle.x) / (sliderWidth / 2);
                        s.value = .5f + Math.Abs(point) / 2;
                    }
                    else
                    {
                        point = (sliderMiddle.x - hit.point.x) / (sliderWidth / 2);
                        s.value = .5f - Math.Abs(point) / 2;
                    }
                }

                else if (s.direction == Scrollbar.Direction.RightToLeft)
                {

                    if (hit.point.x > sliderMiddle.x)
                    {
                        point = (sliderMiddle.x - hit.point.x) / (sliderWidth / 2);
                        s.value = .5f - Math.Abs(point) / 2;
                    }
                    else
                    {
                        point = (hit.point.x - sliderMiddle.x) / (sliderWidth / 2);
                        s.value = .5f + Math.Abs(point) / 2;
                    }
                }

                else if (s.direction == Scrollbar.Direction.BottomToTop)
                {

                    if (hit.point.y > sliderMiddle.y)
                    {
                        point = (hit.point.y - sliderMiddle.y) / (sliderHeight / 2);
                        s.value = .5f + Math.Abs(point) / 2;
                    }
                    else
                    {
                        point = (sliderMiddle.y - hit.point.y) / (sliderHeight / 2);
                        s.value = .5f - Math.Abs(point) / 2;
                    }
                }

                else
                {

                    if (hit.point.y > sliderMiddle.y)
                    {
                        point = (sliderMiddle.y - hit.point.y) / (sliderHeight / 2);
                        s.value = .5f - Math.Abs(point) / 2;
                    }
                    else
                    {
                        point = (hit.point.y - sliderMiddle.y) / (sliderHeight / 2);
                        s.value = .5f + Math.Abs(point) / 2;
                    }
                }
            }
        }
    }

    public void slide(Slider s, Vector3 origin, Vector3 direction)
    {
        Physics.Raycast(origin, direction, out hit);
        canvas = GameObject.Find("Canvas");
        c = canvas.GetComponent<Canvas>();

        Vector3 position = s.transform.localPosition;
        float width = s.GetComponent<RectTransform>().rect.width;
        float height = s.GetComponent<RectTransform>().rect.height;

        Vector3 sliderMiddle = s.transform.TransformPoint(s.transform.TransformPoint(position));
        Vector3 sliderRight = c.transform.TransformPoint(new Vector3(position.x + width / 2, position.y, position.z));
        Vector3 sliderLeft = c.transform.TransformPoint(new Vector3(position.x - width / 2, position.y, position.z));

        Vector3 sliderTop = c.transform.TransformPoint(new Vector3(position.x, position.y + height / 2, position.z));
        Vector3 sliderBottom = c.transform.TransformPoint(new Vector3(position.x, position.y - height / 2, position.z));

        float sliderWidth = Math.Abs(sliderRight.x - sliderLeft.x);
        float sliderHeight = Math.Abs(sliderTop.y - sliderBottom.y);


        float point;
        if (s.direction == Slider.Direction.LeftToRight)
        {

            if (hit.point.x > sliderMiddle.x)
            {
                point = (hit.point.x - sliderMiddle.x) / (sliderWidth / 2);
                s.value = .5f + Math.Abs(point) / 2;
            }
            else
            {
                point = (sliderMiddle.x - hit.point.x) / (sliderWidth / 2);
                s.value = .5f - Math.Abs(point) / 2;
            }
        }

        else if (s.direction == Slider.Direction.RightToLeft)
        {

            if (hit.point.x > sliderMiddle.x)
            {
                point = (sliderMiddle.x - hit.point.x) / (sliderWidth / 2);
                s.value = .5f - Math.Abs(point) / 2;
            }
            else
            {
                point = (hit.point.x - sliderMiddle.x) / (sliderWidth / 2);
                s.value = .5f + Math.Abs(point) / 2;
            }
        }

        else if (s.direction == Slider.Direction.BottomToTop)
        {

            if (hit.point.y > sliderMiddle.y)
            {
                point = (hit.point.y - sliderMiddle.y) / (sliderHeight / 2);
                s.value = .5f + Math.Abs(point) / 2;
            }
            else
            {
                point = (sliderMiddle.y - hit.point.y) / (sliderHeight / 2);
                s.value = .5f - Math.Abs(point) / 2;
            }
        }

        else
        {

            if (hit.point.y > sliderMiddle.y)
            {
                point = (sliderMiddle.y - hit.point.y) / (sliderHeight / 2);
                s.value = .5f - Math.Abs(point) / 2;
            }
            else
            {
                point = (hit.point.y - sliderMiddle.y) / (sliderHeight / 2);
                s.value = .5f + Math.Abs(point) / 2;
            }
        }
    }
}


