/*
 * This class represents the toolManager object and keeps track of the different tool interfaces and shuffles between them as desired.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolManager2 : MonoBehaviour
{
    //Initialize member variables
    public List<ITool> list;
    public GameObject wandObject;
    public GameObject wandControls;
    public GameObject TopLevelUniCAVE;
    public int toolNumber = 0;
    public Text tool;
    public GameObject canvas;
    private DriveTool driveTool;

    /// <summary>
    /// Creates the toolManager object which holds a list of all ITOOL interfaces
    /// </summary>
    /// <param name="wandObject_"></param>
    /// <param name="holder_"></param>
    public ToolManager2(GameObject wandObject_, GameObject wandControls_, GameObject TopLevelUniCAVE_, double deadZone, float rotationSpeed, float movementSpeed, Text tool_)
    {
        wandObject = wandObject_;
        tool = tool_;
        wandControls = wandControls_;
        TopLevelUniCAVE = TopLevelUniCAVE_;

        list = new List<ITool>();
        //Add the scripts to the wandObject
        /*
        if (wandObject.GetComponent<GrabberTool>() == null)
            wandObject.AddComponent<GrabberTool>();
        if (wandObject.GetComponent<WarpTool>() == null)
            wandObject.AddComponent<WarpTool>();
        if (wandObject.GetComponent<ButtonTool>() == null)
            wandObject.AddComponent<ButtonTool>();
        if (wandObject.GetComponent<RotatorTool>() == null)
            wandObject.AddComponent<RotatorTool>();*/

        //fill the list with all the tool interfaces 
        list.Add(wandControls.GetComponent<WarpTool>());
        list.Add(wandControls.GetComponent<GrabberTool>());
        list.Add(wandControls.GetComponent<ButtonTool>());
        list.Add(wandControls.GetComponent<RotatorTool>());

        driveTool = new DriveTool(TopLevelUniCAVE, wandObject, deadZone, rotationSpeed, movementSpeed);

        toolNumber = 0;
        updateToolName(tool);
    }

    /// <summary>
    /// Increments the tool number 
    /// </summary>
    public void NextTool()
    {
        list[toolNumber].shutDown();
        if (toolNumber + 1 >= list.Count)
        {
            toolNumber = 0;
        }
        else
        {
            toolNumber++;
        }
    }

    /// <summary>
    /// Decreases the tool number
    /// </summary>
    public void PreviousTool()
    {
        list[toolNumber].shutDown();
        if (toolNumber - 1 < 0)
        {
            toolNumber = list.Count -1;
        }
        else
        {
            toolNumber--;
        }

    }


    public bool handleButtonClick(TrackerButton button, Vector3 origin, Vector3 direction)
    {
        if (button == TrackerButton.NextTool)
        {
            NextTool();
            updateToolName(tool);
            return true;
        }
        else if (button == TrackerButton.PreviousTool)
        {
            PreviousTool();
            updateToolName(tool);
            return true;
        }
        else
        {
            list[toolNumber].ButtonClick(button, origin, direction);
            driveTool.ButtonClick(button, origin, direction);
            return true;
        }
    }

    public bool handleButtonDrag(TrackerButton button,RaycastHit hit, Vector3 offset, Vector3 origin, Vector3 direction)
    {
        list[toolNumber].ButtonDrag(hit, offset, origin, direction);
        return true;
    }


    public bool handleAnalog(double horizontal, double vertical)
    {
        driveTool.Analog(horizontal, vertical);
        return true;
    }

    public void updateToolName(Text text)
    {
        text.text = "Tool: " + list[toolNumber].ToolName;
    }

    
}
