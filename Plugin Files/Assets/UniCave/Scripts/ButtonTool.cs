//Luke Kingsley July 2017

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class handles all UI interactions for the controller including: buttons, sliders, scroll bars etc.
/// </summary>
public class ButtonTool : MonoBehaviour, ITool
{
    private const string IQ_WALL = "IQWall_Seq_1PC";
    private const string WAND = "Wand";
    private Canvas c;
    private RaycastHit hit, tester;
    private GameObject wandObject;
    private GameObject holder;
    private bool hide = true;
    private Text txt;
    private float point;
    private int click;
    private Event eventsystem;
    private Dropdown dropdown;
    private Toggle toggle;
    private Vector3 origin, direction;


    public GameObject canvas;

    /// <summary>
    /// Selects or highlights UI elements 
    /// </summary>
    /// <returns></returns>
    public void buttonInput()
    {
        while (true)
        {
            //check to see that we are on the buttonclick tool

            //Raycast into the scene
            Physics.Raycast(origin, direction, out tester);
            if (tester.collider != null)
            {
                //Check what object is returned
                if (tester.transform.gameObject.GetComponent<Dropdown>() != null)
                {
                    //Get the correct compnent and select it
                    dropdown = tester.transform.gameObject.GetComponent<Dropdown>();
                    EventSystem.current.SetSelectedGameObject(dropdown.gameObject);
                }
                else if (tester.transform.gameObject.GetComponent<Toggle>() != null)
                {
                    toggle = tester.transform.gameObject.GetComponent<Toggle>();
                    EventSystem.current.SetSelectedGameObject(toggle.gameObject);
                }
                else if (tester.transform.gameObject.GetComponent<Button>() != null)
                {
                    Button button = tester.transform.gameObject.GetComponent<Button>();
                    EventSystem.current.SetSelectedGameObject(button.gameObject);
                }
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }


    /// <summary>
    /// Handles the UI button click interactions 
    /// </summary>
    /// <param name="buttonNum"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
    public void ButtonClick(int buttonNum, Vector3 origin, Vector3 direction, bool cave, bool rotate)
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
            if (hit.collider != null && hit.transform.gameObject.GetComponent<Dropdown>() != null)
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

            //If the object is a dropdown menu selectable set that as the new dropdown value and call the method attatched 
            else if (hit.collider != null && hit.transform.gameObject.GetComponent<Toggle>() != null)
            {
                Toggle t = hit.transform.gameObject.GetComponent<Toggle>();
                Dropdown d = t.GetComponentInParent<Dropdown>();
                int number = t.name[5] - 48;
                d.value = number;
                d.RefreshShownValue();
                d.Hide();
                hide = true;
            }



            //If the object is a button call the onClick method
            else if (hit.collider != null && hit.transform.gameObject.GetComponent<Button>() != null)
            {
                Button button = hit.transform.gameObject.GetComponent<Button>();
                if (buttonNum == 3)
                {
                    button.onClick.Invoke();
                }

            }
        }
    }

    /// <summary>
    /// Allows the user to interact with sliders and scrollbars 
    /// </summary>
    /// <param name="hit_"></param>
    /// <param name="offset"></param>
    /// <param name="origin_"></param>
    /// <param name="direction_"></param>
    public void ButtonDrag(RaycastHit hit_, Vector3 offset, Vector3 origin_, Vector3 direction_)
    {
        origin = origin_;
        direction = direction_;

        Debug.Log(hit_.transform.gameObject.GetType());
        //Check the type of the object to know what to slide
        if (hit_.transform.gameObject.GetComponent<Slider>() != null)
        {
            slide(hit_.transform.gameObject.GetComponent<Slider>(), origin, direction);
        }
        else if (hit_.transform.gameObject.GetComponent<Scrollbar>() != null)
        {
            slide(hit_.transform.gameObject.GetComponent<Scrollbar>(), origin, direction);
        }
    }



    // Use this for initialization
    void Start()
    {
        //Get all necessary game objects
        if (wandObject == null)
        {
            wandObject = GameObject.Find(WAND);
        }

        if (holder == null)
        {
            holder = GameObject.Find(IQ_WALL);
        }
    }

    /// <summary>
    /// User can interact with scrollbars
    /// </summary>
    /// <param name="s"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
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
                s.value = .5f + Math.Abs(point);
            }
            else
            {
                point = (sliderMiddle.x - hit.point.x) / (sliderWidth / 2);
                s.value = .5f - Math.Abs(point);
            }
        }
        //Check the direction of the scroll bar
        else if (s.direction == Scrollbar.Direction.RightToLeft)
        {
            //Check to see which part of the slider was hit
            if (hit.point.x > sliderMiddle.x)
            {
                point = (sliderMiddle.x - hit.point.x) / (sliderWidth / 2);
                //Set the Value
                s.value = .5f - Math.Abs(point);
            }
            else
            {
                point = (hit.point.x - sliderMiddle.x) / (sliderWidth / 2);
                s.value = .5f + Math.Abs(point);
            }
        }
        //Check the direction of the scroll bar
        else if (s.direction == Scrollbar.Direction.BottomToTop)
        {

            if (hit.point.y > sliderMiddle.y)
            {
                point = (hit.point.y - sliderMiddle.y) / (sliderHeight / 2);
                s.value = .5f + Math.Abs(point);
            }
            else
            {
                point = (sliderMiddle.y - hit.point.y) / (sliderHeight / 2);
                s.value = .5f - Math.Abs(point);
            }
        }

        else
        {

            if (hit.point.y > sliderMiddle.y)
            {
                point = (sliderMiddle.y - hit.point.y) / (sliderHeight / 2);
                s.value = .5f - Math.Abs(point);
            }
            else
            {
                point = (hit.point.y - sliderMiddle.y) / (sliderHeight / 2);
                s.value = .5f + Math.Abs(point);
            }
        }
    }
    /// <summary>
    /// Method allows the user to interact with sliders
    /// </summary>
    /// <param name="s"></param>
    /// <param name="origin"></param>
    /// <param name="direction"></param>
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


    //Unimplemented Methods

    void Update()
    {

    }

    public void Analog(double x, double y)
    {
        //throw new NotImplementedException();
    }

    public void init()
    {
        //throw new NotImplementedException();
    }

    public void shutDown()
    {
        //throw new NotImplementedException();
    }

    public void buttonPress(int button, Vector3 origin, Vector3 direction)
    {
        throw new NotImplementedException();
    }
}

