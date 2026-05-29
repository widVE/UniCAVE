using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.Rendering;

public class Defines : Editor
{
    static bool done;

    [InitializeOnLoad]
    public class EditorScriptStart
    {
        static EditorScriptStart()
        {
            Debug.Log("EditorScriptStart " + Time.time);
            PlayerSettings.useFlipModelSwapchain = false;
            //CompilationPipeline.assemblyCompilationStarted -= RemoveDefines;
            //CompilationPipeline.assemblyCompilationStarted += RemoveDefines;
            //CompilationPipeline.compilationStarted -= RemoveDefines;
            CompilationPipeline.compilationStarted += RemoveDefines;

            //if (GraphicsSettings.defaultRenderPipeline.name.Contains("Universal"))
            //    AddDefine("URP");

            //if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
            //    AddDefine("URP");

            //if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("HDRenderPipelineAsset"))
            //    AddDefine("HDRP");

            //if (GraphicsSettings.defaultRenderPipeline)
            if (GraphicsSettings.currentRenderPipeline)
            {
                //if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
                if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
                    AddDefine("URP");
                else
                    //if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("HDRenderPipelineAsset"))
                    if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HDRenderPipelineAsset"))
                    AddDefine("HDRP");
            }

            //if (debug) Debug.Log(GraphicsSettings.defaultRenderPipeline.GetType().ToString());
            //if (debug) Debug.Log(GraphicsSettings.defaultRenderPipeline);

            if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.postprocessing") != null)
                AddDefine("POST_PROCESSING_STACK_V2");
            //else
            //    RemoveDefine("POST_PROCESSING_STACK_V2");

            if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.cinemachine") != null)
            {
                ////if (debug) Debug.Log(UnityEditor.EditorUserBuildSettings.selectedStandaloneTarget);
                //var buildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);
                //UnityEditor.PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] defines);
                ////if (debug) Debug.Log(defines.Length);

                //bool cinemachineDefine = false;

                //foreach (string define in defines)
                //    if (define.Contains("CINEMACHINE"))
                //        cinemachineDefine = true;

                //if (!cinemachineDefine)
                //{
                //    string[] newDefines = new string[defines.Length + 1];
                //    defines.CopyTo(newDefines, 0);
                //    newDefines.SetValue("CINEMACHINE", defines.Length);
                //    UnityEditor.PlayerSettings.SetScriptingDefineSymbols(buildTarget, newDefines);

                //    //foreach (string define in newDefines)
                //        //if (debug) Debug.Log(define);
                //}

                AddDefine("CINEMACHINE");
            }

            if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.inputsystem") != null)
                AddDefine("INPUT_SYSTEM");
            //else
            //    RemoveDefine("INPUT_SYSTEM");

            bool lookWithMouseFound = false;
            //bool simpleCameraControllerFound = false;

            foreach (var assembly in CompilationPipeline.GetAssemblies())
            {
                if (assembly.name.StartsWith("Assembly-CSharp"))
                {
                    foreach (var fileName in assembly.sourceFiles)
                    {
                        if (fileName.Contains("LookWithMouse"))
                            lookWithMouseFound = true;

                        //if (fileName.Contains("SimpleCameraController"))
                        //    simpleCameraControllerFound = true;
                    }
                }
            }

            if (lookWithMouseFound)
                AddDefine("LookWithMouse");

            //if (simpleCameraControllerFound)
            //    AddDefine("SimpleCameraController");
        }
    }

    //static void RemoveDefines(string s)
    static void RemoveDefines(object o)
    {
        //throw new System.NotImplementedException();
        //Debug.Log("RemoveDefines: " + s);
        Debug.Log("RemoveDefines: " + o);

        if (!done)
        {
            done = true;

            //if (GraphicsSettings.currentRenderPipeline)
            //{
            //    //if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
            //    if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
            //    {
            //        AddDefine("URP");
            //        RemoveDefine("HDRP");
            //    }
            //    else
            //        //if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("HDRenderPipelineAsset"))
            //        if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HDRenderPipelineAsset"))
            //        {
            //            AddDefine("HDRP");
            //            RemoveDefine("URP");
            //        }
            //}
            //else
            //{
            //    RemoveDefine("URP");
            //    RemoveDefine("HDRP");
            //}

            //if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.postprocessing") != null)
            //    AddDefine("POST_PROCESSING_STACK_V2");
            //else
            //    RemoveDefine("POST_PROCESSING_STACK_V2");

            //if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.cinemachine") != null)
            //    AddDefine("CINEMACHINE");
            //else
            //    RemoveDefine("CINEMACHINE");

            //if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.inputsystem") != null)
            //    AddDefine("INPUT_SYSTEM");
            //else
            //    RemoveDefine("INPUT_SYSTEM");

            //bool lookWithMouseFound = false;
            //bool simpleCameraControllerFound = false;

            //foreach (var assembly in CompilationPipeline.GetAssemblies())
            //{
            //    if (assembly.name.StartsWith("Assembly-CSharp"))
            //    {
            //        bool lookWithMouseFound = false;
            //        bool simpleCameraControllerFound = false;

            //        foreach (var fileName in assembly.sourceFiles)
            //        {
            //            if (fileName.Contains("LookWithMouse"))
            //                //AddDefine("LookWithMouse");
            //                lookWithMouseFound = true;

            //            if (fileName.Contains("SimpleCameraController"))
            //                //AddDefine("SimpleCameraController");
            //                simpleCameraControllerFound = true;
            //        }
            //    }
            //}

            //if (lookWithMouseFound)
            //    AddDefine("LookWithMouse");
            //else
            //    RemoveDefine("LookWithMouse");

            //if (simpleCameraControllerFound)
            //    AddDefine("SimpleCameraController");
            //else
            //    RemoveDefine("SimpleCameraController");

            if (GraphicsSettings.currentRenderPipeline)
            {
                //if (GraphicsSettings.defaultRenderPipeline.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
                if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("UniversalRenderPipelineAsset"))
                    RemoveDefine("HDRP");
                else
                    if (GraphicsSettings.currentRenderPipeline.GetType().ToString().Contains("HDRenderPipelineAsset"))
                        RemoveDefine("URP");
            }
            else
            {
                RemoveDefine("URP");
                RemoveDefine("HDRP");
            }

            if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.postprocessing") == null)
                RemoveDefine("POST_PROCESSING_STACK_V2");

            if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.cinemachine") == null)
                RemoveDefine("CINEMACHINE");

            if (UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/com.unity.inputsystem") == null)
                RemoveDefine("INPUT_SYSTEM");

            bool lookWithMouseFound = false;
            //bool simpleCameraControllerFound = false;

            foreach (var assembly in CompilationPipeline.GetAssemblies())
            {
                if (assembly.name.StartsWith("Assembly-CSharp"))
                {
                    foreach (var fileName in assembly.sourceFiles)
                    {
                        if (fileName.Contains("LookWithMouse"))
                            lookWithMouseFound = true;

                        //if (fileName.Contains("SimpleCameraController"))
                        //    simpleCameraControllerFound = true;
                    }
                }
            }

            if (!lookWithMouseFound)
                RemoveDefine("LookWithMouse");

            //if (!simpleCameraControllerFound)
            //    RemoveDefine("SimpleCameraController");
        }

        //CompilationPipeline.assemblyCompilationStarted -= RemoveDefines;
        //CompilationPipeline.compilationStarted -= RemoveDefines;
    }

    static void AddDefine(string defineName)
    {
        Debug.Log("AddDefine: " + defineName);
#if UNITY_2021_2_OR_NEWER
        var buildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] defines);

        bool alreadyDefined = false;

        foreach (string define in defines)
            //if (define.Contains(defineName))
            if (define == defineName)
            {
                //if (debug) Debug.Log(defineName + " already Defined");
                alreadyDefined = true;
            }

        if (!alreadyDefined)
        {
            string[] newDefines = new string[defines.Length + 1];
            defines.CopyTo(newDefines, 0);
            newDefines.SetValue(defineName, defines.Length);

            //foreach(var def in newDefines)
            //    if (debug) Debug.Log(def);

            PlayerSettings.SetScriptingDefineSymbols(buildTarget, newDefines);
        }
#else
        var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
        string filteredDefines = defines.Replace("UNITY_" + defineName, "");

        //if (!filteredDefines.Contains(defineName))
        //if (defines != defineName && !defines.Contains(";" + defineName) && !defines.Contains(defineName + ";"))
        //if (!defines.Contains(defineName) || defines.Contains("UNITY_" + defineName))
        if (!filteredDefines.Contains(defineName))
        {
            if (defines != "")
                defines += ";";

            defines += defineName;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
        }
#endif
    }

    static void RemoveDefine(string defineName)
    {
        Debug.Log("RemoveDefine: " + defineName);
#if UNITY_2021_2_OR_NEWER
        var buildTarget = UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] defines);
        //Debug.Log(defines.ToString());
        //bool alreadyDefined = false;

        for (int i = 0; i < defines.Length; i++)
            //if (define.Contains(defineName))
            if (defines[i] == defineName)
            {
                Debug.Log("defines[i] == defineName");
                //if (debug) Debug.Log(defineName + " already Defined");
                //alreadyDefined = true;
                defines.SetValue("", i);
                //done = false;
            }

        PlayerSettings.SetScriptingDefineSymbols(buildTarget, defines);
#else
        var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

        if (defines == defineName)
            defines = "";
        else
            defines = defines.Replace(";" + defineName, "");

        PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, defines);
#endif
    }
}
