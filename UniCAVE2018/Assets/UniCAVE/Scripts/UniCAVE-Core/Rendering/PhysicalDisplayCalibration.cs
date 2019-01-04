//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
//Benny Wysong-Grass
//University of Wisconsin - Madison Virtual Environments Group
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

[RequireComponent(typeof(PhysicalDisplay))]
public class PhysicalDisplayCalibration : MonoBehaviour {

    public static Vector3 globalPostOffset = new Vector3(0.0f, 0.0f, 0.0f);

    public bool loadAtRuntime = false;

    private float displayRatio {
        get {
            return GetComponent<PhysicalDisplay>().windowBounds.width / (float)GetComponent<PhysicalDisplay>().windowBounds.height;
        }
    }

    [Tooltip("This should be an unlit textured material")]
    public Material postProcessMaterial;

    public Vector2 upperRightPosition;
    public Vector2 upperLeftPosition;
    public Vector2 lowerLeftPosition;
    public Vector2 lowerRightPosition;

    [Tooltip("The resolution the camera will render at before warp correction")]
    public Vector2Int resolution = new Vector2Int(1280, 720);

    [ContextMenuItem("Load Warp File", "preloadWarpFile")]
    public string warpFile;
    private void preloadWarpFile() {
#if UNITY_EDITOR
        string loaded= System.IO.File.ReadAllText(warpFile);
        string[] lines = loaded.Split('\n');
        Vector2 offset = new Vector2(-0.5f, -0.5f);

        for(int i = 0; i < lines.Length; i++) {
            try {
                string line = lines[i].Substring(3);
                if(line.StartsWith("0_0")) { //lower left, I assume
                    string[] parts = line.Split(':')[1].Split(',');
                    lowerLeftPosition = (new Vector2(float.Parse(parts[0]), float.Parse(parts[1])) + offset) * 2.0f;
                    Debug.Log("Found Lower Left");
                } else if (line.StartsWith("0_12")) {
                    string[] parts = line.Split(':')[1].Split(',');
                    upperLeftPosition = (new Vector2(float.Parse(parts[0]), float.Parse(parts[1])) + offset) * 2.0f;
                    Debug.Log("Found Upper Left");
                } else if (line.StartsWith("16_0")) {
                    string[] parts = line.Split(':')[1].Split(',');
                    lowerRightPosition = (new Vector2(float.Parse(parts[0]), float.Parse(parts[1])) + offset) * 2.0f;
                    Debug.Log("Found Lower Right");
                } else if (line.StartsWith("16_12")) {
                    string[] parts = line.Split(':')[1].Split(',');
                    upperRightPosition = (new Vector2(float.Parse(parts[0]), float.Parse(parts[1])) + offset) * 2.0f;
                    Debug.Log("Found Upper Right");
                }
            } catch (Exception ignored) { }
        }
        EditorUtility.SetDirty(this);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
#endif
    }

    private GameObject leftChild, rightChild, camChild;

    private Material leftRenderMat, rightRenderMat;

    public List<Camera> postCams = new List<Camera>();

    void SetupPostProcessing() {
        PhysicalDisplay display = GetComponent<PhysicalDisplay>();

        bool stereo = true;
        if (display.leftCam != null) {
            //create left child object that will contain the dewarping mesh
            leftChild = new GameObject("Dewarp Mesh (left) For: " + gameObject.name);
            leftChild.layer = 8;
            leftChild.transform.parent = transform;

            //add the dewarping mesh to the left child
            MeshFilter meshComponent = leftChild.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            Vector2 multiplier = new Vector2(displayRatio, 1.0f);
            Vector3[] verts = {
                upperRightPosition * multiplier,
                upperLeftPosition * multiplier,
                lowerLeftPosition * multiplier,
                lowerRightPosition * multiplier
            };
            Vector2[] uvs = {
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f)
            };
            Vector3[] normals = {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            int[] triangles = {
                0, 2, 1,
                0, 3, 2
            };
            mesh.vertices = verts;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.normals = normals;
            meshComponent.mesh = mesh;
            leftChild.layer = 8; //post processing layer is 8

            //create material for left mesh
            leftRenderMat = new Material(postProcessMaterial);
            leftRenderMat.name = "Left Material";

            //create render texture for left camera
            display.leftCam.cullingMask &= ~(1 << 8); //remove post processing layer of the worldspace camera
            Vector3 oldPos = display.leftCam.transform.localPosition;
            display.leftCam.stereoTargetEye = StereoTargetEyeMask.None;
            display.leftCam.transform.localPosition = oldPos;

            //assign the render texture to the material and the material to the mesh
            leftRenderMat.mainTexture = display.leftTex;
            leftChild.AddComponent<MeshRenderer>().material = leftRenderMat;

        } else {
            stereo = false;
        }

        if (display.rightCam != null) {
            //create right child object that will contain the dewarping mesh
            rightChild = new GameObject("Dewarp Mesh (right) For: " + gameObject.name);
            rightChild.layer = 8;
            rightChild.transform.parent = transform;

            //add the dewarping mesh to the right child
            MeshFilter meshComponent = rightChild.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            Vector2 multiplier = new Vector2(displayRatio, 1.0f);
            Vector3[] verts = {
                upperRightPosition * multiplier,
                upperLeftPosition * multiplier,
                lowerLeftPosition * multiplier,
                lowerRightPosition * multiplier
            };
            Vector2[] uvs = {
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f)
            };
            Vector3[] normals = {
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward,
                -Vector3.forward
            };
            int[] triangles = {
                0, 2, 1,
                0, 3, 2
            };
            mesh.vertices = verts;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.normals = normals;
            meshComponent.mesh = mesh;
            rightChild.layer = 8; //post processing layer is 8

            //create material for right mesh
            rightRenderMat = new Material(postProcessMaterial);
            rightRenderMat.name = "Right Material";

            //create render texture for right camera
            display.rightCam.cullingMask &= ~(1 << 8); //remove post processing layer of the worldspace camera
            Vector3 oldPos = display.rightCam.transform.localPosition;
            display.rightCam.stereoTargetEye = StereoTargetEyeMask.None;
            display.rightCam.transform.localPosition = oldPos;

            //assign the render texture to the material and the material to the mesh
            rightRenderMat.mainTexture = display.rightTex;
            rightChild.AddComponent<MeshRenderer>().material = rightRenderMat;
        } else {
            stereo = false;
        }

        //set the positions of the dewarping mesh children
        //we have to do this later because if its not stereo only one exists and it should be at the center of the screen
        if (leftChild != null) leftChild.transform.localPosition = globalPostOffset + new Vector3(stereo ? -displayRatio : 0.0f, 0.0f, 0.0f);
        if (rightChild != null) rightChild.transform.localPosition = globalPostOffset + new Vector3(stereo ? displayRatio : 0.0f, 0.0f, 0.0f);


        {
            camChild = new GameObject("Calibration Cam (Left)");
            camChild.transform.parent = transform;
            Camera postCam = camChild.AddComponent<Camera>();
            postCam.transform.localPosition = globalPostOffset + new Vector3(stereo ? -displayRatio : 0.0f, 0.0f, -1.0f);
            postCam.nearClipPlane = 0.1f;
            postCam.farClipPlane = 10.0f;
            postCam.fieldOfView = 90.0f;
            postCam.stereoSeparation = 0.0f;
            postCam.stereoConvergence = 1000.0f; //probably doesn't matter but far away makes the most sense
            postCam.cullingMask = 1 << 8; //post processing layer
            postCam.backgroundColor = Color.black;
            postCam.clearFlags = CameraClearFlags.SolidColor;
            postCam.depth = 1;
            postCam.stereoTargetEye = StereoTargetEyeMask.Left;
            postCam.allowHDR = false;
            postCam.allowMSAA = false;
            postCam.renderingPath = RenderingPath.Forward;
            postCams.Add(postCam);
        }
        {
            GameObject obj2 = new GameObject("Calibration Cam (Right)");
            obj2.transform.parent = transform;
            Camera postCam = obj2.AddComponent<Camera>();
            postCam.transform.localPosition = globalPostOffset + new Vector3(stereo ? displayRatio : 0.0f, 0.0f, -1.0f);
            postCam.nearClipPlane = 0.1f;
            postCam.farClipPlane = 10.0f;
            postCam.fieldOfView = 90.0f;
            postCam.stereoSeparation = 0.0f;
            postCam.stereoConvergence = 1000.0f; //probably doesn't matter but far away makes the most sense
            postCam.cullingMask = 1 << 8; //post processing layer
            postCam.backgroundColor = Color.black;
            postCam.clearFlags = CameraClearFlags.SolidColor;
            postCam.depth = 1;
            postCam.stereoTargetEye = StereoTargetEyeMask.Right;
            postCam.allowHDR = false;
            postCam.allowMSAA = false;
            postCam.renderingPath = RenderingPath.Forward;
            postCams.Add(postCam);
        }
        globalPostOffset = globalPostOffset + new Vector3(10, 10, 10);
    }

    void OnDrawGizmosSelected() {
        PhysicalDisplay disp = GetComponent<PhysicalDisplay>();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(disp.ScreenspaceToWorld(upperRightPosition), disp.ScreenspaceToWorld(upperLeftPosition));
        Gizmos.DrawLine(disp.ScreenspaceToWorld(upperRightPosition), disp.ScreenspaceToWorld(lowerRightPosition));
        Gizmos.DrawLine(disp.ScreenspaceToWorld(lowerLeftPosition), disp.ScreenspaceToWorld(upperLeftPosition));
        Gizmos.DrawLine(disp.ScreenspaceToWorld(lowerLeftPosition), disp.ScreenspaceToWorld(lowerRightPosition));
    }

    public bool Initialized() {
        return initialized;
    }

    private bool initialized = false;
    void Update() {
        if (!initialized) {
            if(GetComponent<PhysicalDisplay>().Initialized()) {
                SetupPostProcessing();
                initialized = true;
            }
        } else {
            PhysicalDisplay display = GetComponent<PhysicalDisplay>();
        }
    }
}
