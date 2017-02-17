using UnityEngine;
using System.Collections;

//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

public class wandButtonsCheck : MonoBehaviour {
    private bool isActive;
    public GameObject colorTest;
	
	void Update () {
        //Transform transform = gameObject.transform;
        if (transform.localPosition.x != 0)
        {
            isActive = true;
        } else { isActive = false; }
        if (isActive) { 
            switch ((int)transform.localPosition.z) //tell unity what to do when each button is pressed HERE
            {
                case 0:
                    Debug.Log("Button 0");
                    break;
                case 1:
                    Debug.Log("Button 1");
                    break;
                case 2:
                    Debug.Log("Button 2");
                    break;
                case 3:
                    Debug.Log("Button 3");
                    break;
                case 4:
                    Debug.Log("Button 4");
                    break;
                case 5: //trigger
                    Debug.Log("Button 5");
                    break;
                default:
                    Debug.Log("hmm, error!");
                    break;
            }
          /*  if (transform.localPosition.x == 6)
            {
                colorTest.GetComponent<Renderer>().material.color = Color.green;
            } else
            {
                colorTest.GetComponent<Renderer>().material.color = Color.red;
            }  */
        }
    }
}
