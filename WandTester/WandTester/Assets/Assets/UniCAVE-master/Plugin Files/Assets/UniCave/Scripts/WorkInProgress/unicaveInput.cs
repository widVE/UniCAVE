//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
//Brady Boettcher
//Living Environments Laboratory
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//[System.Serializable]
/*
public class uInput : ScriptableObject
{
    public string inputName;
    public bool isPressed;
    public uInput(string inputName, bool pressed)
    {
        this.inputName = inputName;
        this.isPressed = pressed;
        //Debug.Log(inputName);
        //name = inputName;
    }

    //setter
    public void setPressed(bool isPressed)
    {
        this.isPressed = isPressed;
    }
    public void setName(string inputName)
    {
        this.inputName = inputName;
    }
    //getters
    public bool getPressed()
    {
        return isPressed;
    }
    public string getName()
    {
        return this.inputName;
    }
}
*/
/*
 * Unicave Input Class
 * 
 */
public class unicaveInput : MonoBehaviour  {

    //uInput script = gameObject.GetComponent<uInput>();
    [SerializeField]
    public List<uInput> inputs; //list of all the inputs found so far
    private static unicaveInput _instance;
    private string machineName; //name of the node

    // Use this for initialization
    void Start () {
        machineName = System.Environment.MachineName;
    }

    void Awake()
    {
        _instance = this;
        //Instance().inputs = inputs;
    }

    public static unicaveInput Instance()
    {
        return _instance;
    }

    //this method collects all instances of when the Input class is being used, and replaces it with our custom Input class
    [ContextMenu("Sync Inputs")]
    private void syncInputs()
    {
        bool changeFile = true;
        inputs = new List<uInput>();
        Debug.Log("Syncing Inputs...");
        if (inputs == null)
        {
            inputs = new List<uInput>();
        } else
        {
            inputs.Clear();
        }
            
        char[] UNICAVE = { 'u', 'n', 'i', 'c', 'a', 'v', 'e'};
        char[] instanceChar = { 'I', 'n', 's', 't', 'a', 'n', 'c', 'e', '(', ')', '.'};
        DirectoryInfo dir = new DirectoryInfo("Assets"); //directory to start search in
        List<FileInfo> filepaths = new List<FileInfo>(); //make list to put all cs files in
        syncInputs(filepaths, dir);
        List<int> indices = new List<int>();
        foreach (FileInfo file in filepaths)
        {
            changeFile = true;
            if (/* file.Name != "unicaveInput.cs" */ file.Name == "PlayerShooting.cs")
            {
                //parse each file looking for 'Input.getButtonDown' and replace with 'unicaveInput.getButtonDown'
                StreamReader sr = file.OpenText();
                string s = sr.ReadToEnd();
                sr.Close();
                List<char> chars = new List<char>((char[])s.ToCharArray());
                for (int i = 0; i < chars.Count; i++)
                {
                    if (chars[i] == 'I' && chars[i + 1] == 'n' && chars[i + 2] == 'p' && chars[i + 3] == 'u' && chars[i + 4] == 't' && chars[i + 5] == '.')
                    {
                        if (chars[i - 1] == 'e') {
                            //continue;
                            changeFile = false;
                            //break;
                        }
                        indices.Add(i);
                        if (changeFile)
                        {
                            //insert 'unicave' before 'Input.' and 'Instance(). after it
                            for (int j = 0; j < 7; j++)
                            {
                                chars.Insert(i + j, UNICAVE[j]);
                            }
                            i += 13;
                            for (int j = 0; j < 11; j++)
                            {
                                chars.Insert(i + j, instanceChar[j]);
                            }
                        }
                        i += 11;
                        //check if 'getButtonDown' is being called
                        if (chars[i + 6] == 'G' && chars[i + 9] == 'B' && chars[i + 19] == '(')
                        {
                            List<char> inputName = new List<char>();
                            int count = 21;
                            while (chars[i + count] != '\"')
                            {
                                inputName.Add(chars[i + count]);
                                count++;
                            }
                            string inputNameString = new string(inputName.ToArray());
                            uInput newInput = (uInput)ScriptableObject.CreateInstance(typeof(uInput));
                            newInput.inputName = inputNameString;
                            newInput.isPressed = false;
                            newInput.name = inputNameString;
                            bool shouldAdd = true;
                            for (int j = 0; j < inputs.Count; j++)
                            {
                                if (inputs[j].inputName.Equals(newInput.inputName))
                                {
                                    shouldAdd = false;
                                    break;
                                }
                            }
                            if (shouldAdd)
                            {
                                inputs.Add(newInput);
                                Debug.Log("Input: \"" + newInput.inputName + "\" added.");
                            }

                        }
                        //check if 'getButton' is being called
                        if (chars[i + 6] == 'G' && chars[i + 9] == 'B' && chars[i + 15] == '(')
                        {
                            List<char> inputName = new List<char>();
                            int count = 17;
                            while (chars[i + count] != '\"')
                            {
                                inputName.Add(chars[i + count]);
                                count++;
                            }
                            string inputNameString = new string(inputName.ToArray());
                            uInput newInput = (uInput)ScriptableObject.CreateInstance(typeof(uInput));
                            newInput.inputName = inputNameString;
                            newInput.isPressed = false;
                            newInput.name = inputNameString;
                            bool shouldAdd = true;
                            for (int j = 0; j < inputs.Count; j++)
                            {
                                if (inputs[j].inputName.Equals(newInput.inputName))
                                {
                                    shouldAdd = false;
                                    break;
                                }
                            }
                            if (shouldAdd)
                            {
                                inputs.Add(newInput);
                                Debug.Log("Input: \"" + newInput.inputName + "\" added.");
                            }

                        }

                    } 
                }
                if (indices.Count == 0)
                {
                    Debug.Log("Nothing found.");
                }
                //Debug.Log(new string(chars.ToArray()));

            //overwrite file with new contents
                StreamWriter writer = File.CreateText(file.FullName);
                writer.Write(new string(chars.ToArray()));
                writer.Close();
            }
        }
        Debug.Log("Inputs Synced.");
    }
    
    //recursive helper method for syncInputs
    private void syncInputs(List<FileInfo> files, DirectoryInfo dir)
    {
        DirectoryInfo[] subs = dir.GetDirectories(); //get subdirectories
        FileInfo[] newFiles = dir.GetFiles("*.cs"); //get files with .cs extension
        foreach (FileInfo f in newFiles)
        {
            //add files with *.cs to file array
            files.Add(f);
        }
        foreach (DirectoryInfo d in subs)
        {
            //call helper method on each subdirectory
            syncInputs(files, d);
        }
    }

    public bool GetButtonDown(string s)
    {
        if (machineName == MasterTrackingData.HeadNodeMachineName)
        {
            if (Input.GetButtonDown(s))
            {
                GetComponent<NetworkView>().RPC("buttonPressed", RPCMode.Others, s);
                return true;
            }
            else
            {
                foreach (uInput input in inputs)
                {
                    if (input.inputName.Equals(s))
                    {
                        input.isPressed = false;
                    }
                }
                return false;
            }
        }
        else
        {
            foreach (uInput input in inputs)
            {
                //Debug.Log("saw input: " + input.inputName);
                if (input.inputName.Equals(s))
                {
                    if (input.isPressed)
                    {
                        input.isPressed = false;
                        return true;
                    } else
                    {
                        return false;
                    }
                    
                }
            }
            Debug.LogError("input: " + s + " not found in slave.");
            return false;
        }
    }

    public bool GetButton(string s)
    {
        if (machineName == MasterTrackingData.HeadNodeMachineName)
        {
            if (Input.GetButton(s))
            {
                //Debug.LogError(s + " pressed.");
                GetComponent<NetworkView>().RPC("buttonPressed", RPCMode.Others, s);
                return true;
            }
            else
            {
                foreach (uInput input in inputs)
                {
                    if (input.inputName.Equals(s))
                    {
                        input.isPressed = false;
                    }
                }
                return false;
            }   
        }
        else
        {
            foreach (uInput input in inputs)
            {
                //Debug.Log("saw input: " + input.inputName);
                if (input.inputName.Equals(s))
                {
                    if (input.isPressed)
                    {
                        input.isPressed = false;
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
            }
            //Debug.LogError("input: " + s + " not found in slave.");
            return false;
        }
    }

    //RPC function called to let slave nodes know that input has been asserted
    [RPC]
    public void buttonPressed(string name)
    {
        foreach (uInput input in inputs)
        {
            if (input.inputName.Equals(name))
            {
                input.isPressed = true;
            }
        }
    }

}