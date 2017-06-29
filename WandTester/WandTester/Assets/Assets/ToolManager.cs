using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ToolManager //: MonoBehaviour
{

    public static List<ITool> list;
    //public static ITool currentTool;
    static GameObject holder;
    static GameObject wandObject;
    public static int toolNumber = 0;
 

    public static void init(GameObject wandObject_, GameObject holder_)
    {

        //holder = holder_;
        //wandObject = wandObject_;
        list = new List<ITool>();
        //WarpTool warpObject = new WarpTool(wandObject_, holder_);
        //GrabberTool grabberObject = new GrabberTool(wandObject_, holder_);
        //fill the list with all the tool interfaces 
        //list.Add(warpObject);
        //list.Add(grabberObject);      
    }

    public static void NextTool()
    {
        if (toolNumber + 1 > list.Count)
        {
            toolNumber = 0;            
        }
         else
        {
            toolNumber++;
        }

     
            Debug.Log("GRAB");
    }

    public static void PreviousTool()
    {
        if (toolNumber - 1 < 0)
        {
            toolNumber = list.Count;
        }
        else
        {
            toolNumber--;
        }
      
            Debug.Log("Teleport");
    }
    //Maybe?
    public static void DefaultTool()
    {

    }
   
}
