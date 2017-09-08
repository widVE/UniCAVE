using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Class to run editor co-routines. Not used right now.
/// </summary>
public class EditorCoroutine
{
    /// <summary>
    /// Starts a new coroutine
    /// </summary>
    /// <param name="_routine">Function to call</param>
    /// <returns></returns>
    public static EditorCoroutine start(IEnumerator _routine)
    {
        EditorCoroutine coroutine = new EditorCoroutine(_routine);
        coroutine.start();
        return coroutine;
    }


    readonly IEnumerator routine;
    
    /// <summary>
    /// Constructor for routine
    /// </summary>
    /// <param name="_routine"></param>
    EditorCoroutine(IEnumerator _routine)
    {
        routine = _routine;
    }

    /// <summary>
    /// Starts the coroutine exec
    /// </summary>
    void start()
    {
#if UNITY_EDITOR
        EditorApplication.update += update;
#endif
    }

    /// <summary>
    /// Stops the coroutine
    /// </summary>
    public void stop()
    {
#if UNITY_EDITOR
        EditorApplication.update -= update;
#endif
    }

    /// <summary>
    /// Handles the coroutine exec.
    /// </summary>
    void update()
    {
        if (!routine.MoveNext())
        {
            stop();
        }
    }
}

