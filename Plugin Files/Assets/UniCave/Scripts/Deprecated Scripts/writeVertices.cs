using UnityEngine;
using System.Collections;

//Copyright Living Environments Laboratory - University of Wisconsin - Madison
//Ross Tredinnick
//Brady Boettcher

public class writeVertices : MonoBehaviour {

    private GameObject[] planes;
    public string filePath;
    public string server;
    public string port;
    public string xRes;
    public string yRes;
    private string fileText = "";
    public int numViewportsPerWall;
    public bool splitStereo;
    [Header("Only Fill In If Viewports Aren't Relative To Their Parent Wall")]
    public string[] viewports;


	// Use this for initialization
	void Start () {

        planes = GameObject.FindGameObjectsWithTag("plane");

        fileText += "server " + server + System.Environment.NewLine;
        fileText += "port " + port + System.Environment.NewLine;
        fileText += "height " + yRes + System.Environment.NewLine;
        fileText += "width " + xRes + System.Environment.NewLine;

        for (int i = 0; i < planes.Length; i++)
        {
            Mesh tempMesh = planes[i].GetComponent<MeshFilter>().mesh;

            fileText += planes[i].name + " " + trimString(planes[i].transform.TransformPoint(tempMesh.vertices[0]).ToString()) + " ";
            fileText += trimString((planes[i].transform.TransformPoint(tempMesh.vertices[2]) - planes[i].transform.TransformPoint(tempMesh.vertices[0])).ToString()) + " ";
            fileText += trimString((planes[i].transform.TransformPoint(tempMesh.vertices[3]) - planes[i].transform.TransformPoint(tempMesh.vertices[0])).ToString()) + System.Environment.NewLine;


            if (viewports.Length == 0)
            {
                for (int j = 0; j < numViewportsPerWall; j++)
                {
                fileText += "   viewport ";
                GameObject tempChild = planes[i].transform.GetChild(j).gameObject;
                Mesh childMesh = tempChild.GetComponent<MeshFilter>().mesh;

                //checks for floor or ceiling
                if (Vector3.Cross(tempChild.transform.TransformPoint(childMesh.vertices[0]) - tempChild.transform.TransformPoint(childMesh.vertices[3]), tempChild.transform.TransformPoint(childMesh.vertices[0]) - tempChild.transform.TransformPoint(childMesh.vertices[2])).normalized == new Vector3(1, 0, 0) || Vector3.Cross(tempChild.transform.TransformPoint(childMesh.vertices[0]) - tempChild.transform.TransformPoint(childMesh.vertices[3]), tempChild.transform.TransformPoint(childMesh.vertices[0]) - tempChild.transform.TransformPoint(childMesh.vertices[2])).normalized == new Vector3(-1, 0, 0))
                {
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[0]).z - planes[i].transform.TransformPoint(tempMesh.vertices[0]).z) / (planes[i].transform.TransformPoint(tempMesh.vertices[2]).z - planes[i].transform.TransformPoint(tempMesh.vertices[0]).z)).ToString() + " ";
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[0]).y - planes[i].transform.TransformPoint(tempMesh.vertices[0]).y) / (planes[i].transform.TransformPoint(tempMesh.vertices[3]).y - planes[i].transform.TransformPoint(tempMesh.vertices[0]).y)).ToString() + " ";
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[2]).z - tempChild.transform.TransformPoint(childMesh.vertices[0]).z) / (planes[i].transform.TransformPoint(tempMesh.vertices[2]).z - planes[i].transform.TransformPoint(tempMesh.vertices[0]).z)) + " ";
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[3]).y - tempChild.transform.TransformPoint(childMesh.vertices[0]).y) / (planes[i].transform.TransformPoint(tempMesh.vertices[3]).y - planes[i].transform.TransformPoint(tempMesh.vertices[0]).y));
                }
                    //checks for left or right wall
                else if (Vector3.Cross(tempChild.transform.TransformPoint(childMesh.vertices[0]) - tempChild.transform.TransformPoint(childMesh.vertices[3]), tempChild.transform.TransformPoint(childMesh.vertices[0]) - tempChild.transform.TransformPoint(childMesh.vertices[2])).normalized == new Vector3(0, 1, 0) || Vector3.Cross(tempChild.transform.TransformPoint(childMesh.vertices[0]) - tempChild.transform.TransformPoint(childMesh.vertices[3]), tempChild.transform.TransformPoint(childMesh.vertices[0]) - tempChild.transform.TransformPoint(childMesh.vertices[2])).normalized == new Vector3(0, -1, 0))
                {
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[0]).x - planes[i].transform.TransformPoint(tempMesh.vertices[0]).x) / (planes[i].transform.TransformPoint(tempMesh.vertices[2]).x - planes[i].transform.TransformPoint(tempMesh.vertices[0]).x)).ToString() + " ";
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[0]).z - planes[i].transform.TransformPoint(tempMesh.vertices[0]).z) / (planes[i].transform.TransformPoint(tempMesh.vertices[3]).z - planes[i].transform.TransformPoint(tempMesh.vertices[0]).z)).ToString() + " ";
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[2]).x - tempChild.transform.TransformPoint(childMesh.vertices[0]).x) / (planes[i].transform.TransformPoint(tempMesh.vertices[2]).x - planes[i].transform.TransformPoint(tempMesh.vertices[0]).x)) + " ";
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[3]).z - tempChild.transform.TransformPoint(childMesh.vertices[0]).z) / (planes[i].transform.TransformPoint(tempMesh.vertices[3]).z - planes[i].transform.TransformPoint(tempMesh.vertices[0]).z));
                }
                else
                {
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[0]).x - planes[i].transform.TransformPoint(tempMesh.vertices[0]).x) / (planes[i].transform.TransformPoint(tempMesh.vertices[2]).x - planes[i].transform.TransformPoint(tempMesh.vertices[0]).x)).ToString() + " ";
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[0]).y - planes[i].transform.TransformPoint(tempMesh.vertices[0]).y) / (planes[i].transform.TransformPoint(tempMesh.vertices[3]).y - planes[i].transform.TransformPoint(tempMesh.vertices[0]).y)).ToString() + " ";
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[2]).x - tempChild.transform.TransformPoint(childMesh.vertices[0]).x) / (planes[i].transform.TransformPoint(tempMesh.vertices[2]).x - planes[i].transform.TransformPoint(tempMesh.vertices[0]).x)) + " ";
                    fileText += ((tempChild.transform.TransformPoint(childMesh.vertices[3]).y - tempChild.transform.TransformPoint(childMesh.vertices[0]).y) / (planes[i].transform.TransformPoint(tempMesh.vertices[3]).y - planes[i].transform.TransformPoint(tempMesh.vertices[0]).y));
                }

                
                
                if (splitStereo)
                {
                    fileText += grabViewport(tempChild);
                }
                fileText += System.Environment.NewLine;
            }
            }
            else
            {
                    fileText += "   viewport " + viewports[i] + System.Environment.NewLine;
            }

            

        }
        System.IO.File.WriteAllText(filePath, fileText);
    }

    string trimString(string text)
    {
        text = text.Replace("(", "").Replace(",", "").Replace(")", "");
        return text;
    }

    string grabViewport(GameObject viewport)
    {
        var script = viewport.GetComponent<setViewport>();
        string viewportText = " ";
        viewportText += script.leftX + " " + script.leftY + " " + script.leftWidth + " " + script.leftHeight + " ";
        viewportText += script.rightX + " " + script.rightY + " " + script.rightWidth + " " + script.rightHeight;

        return viewportText;
    }
}
