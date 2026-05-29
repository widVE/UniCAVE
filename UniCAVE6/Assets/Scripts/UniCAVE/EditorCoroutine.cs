//MIT License
//Copyright 2016-Present 
//James H. Money
//Luke Kingsley
//Idaho National Laboratory
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
//to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, 
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
//INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
//IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
//TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UniCAVE
{
    /// <summary>
    /// Class to run editor co-routines. Not used right now.
    /// </summary>
    public class EditorCoroutine
    {
        /// <summary>
        /// Starts a new coroutine
        /// </summary>
        /// <param name="routine">Function to call</param>
        /// <returns></returns>
        public static EditorCoroutine Start(IEnumerator routine)
        {
            EditorCoroutine coroutine = new(routine);
            coroutine.Start();
            return coroutine;
        }


        readonly IEnumerator _routine;

        /// <summary>
        /// Constructor for routine
        /// </summary>
        /// <param name="routine"></param>
        EditorCoroutine(IEnumerator routine)
        {
            _routine = routine;
        }

        /// <summary>
        /// Starts the coroutine exec
        /// </summary>
        void Start()
        {
#if UNITY_EDITOR
            EditorApplication.update += Update;
#endif
        }

        /// <summary>
        /// Stops the coroutine
        /// </summary>
        public void Stop()
        {
#if UNITY_EDITOR
            EditorApplication.update -= Update;
#endif
        }

        /// <summary>
        /// Handles the coroutine exec.
        /// </summary>
        void Update()
        {
            if(!_routine.MoveNext())
            {
                Stop();
            }
        }
    }
}