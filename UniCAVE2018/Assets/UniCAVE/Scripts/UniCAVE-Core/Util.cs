//MIT License
//Copyright 2016-Present 
//Ross Tredinnick
//Brady Boettcher
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

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public static class Util {
    public static string GetMachineName() {
        string overridden = GetArg("overrideMachineName");
        if (overridden != null) {
            return overridden;
        } else {
            string appendMachineName = GetArg("appendMachineName");
            if (appendMachineName == null) appendMachineName = "";
            return System.Environment.MachineName + appendMachineName;
        }
    }

    public static Dictionary<string, string> cachedArgs = new Dictionary<string, string>();
    public static string GetArg(string name) {
        if (cachedArgs.ContainsKey(name))
            return cachedArgs[name];

        string[] args = System.Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
            if (args[i] == name && args.Length > i + 1)
                return (cachedArgs[name] = args[i + 1]);

        return null;
    }

    public static Matrix4x4 getAsymProjMatrix(Vector3 pa, Vector3 pb, Vector3 pc, Vector3 pe, float ncp, float fcp) {
        //compute orthonormal basis for the screen - could pre-compute this...
        Vector3 vr = (pb - pa).normalized;
        Vector3 vu = (pc - pa).normalized;
        Vector3 vn = Vector3.Cross(vr, vu).normalized;

        //compute screen corner vectors
        Vector3 va = pa - pe;
        Vector3 vb = pb - pe;
        Vector3 vc = pc - pe;

        //find the distance from the eye to screen plane
        float n = ncp;
        float f = fcp;
        float d = Vector3.Dot(va, vn); // distance from eye to screen
        float nod = n / d;
        float l = Vector3.Dot(vr, va) * nod;
        float r = Vector3.Dot(vr, vb) * nod;
        float b = Vector3.Dot(vu, va) * nod;
        float t = Vector3.Dot(vu, vc) * nod;

        //put together the matrix - bout time amirite?
        Matrix4x4 m = Matrix4x4.zero;

        //from http://forum.unity3d.com/threads/using-projection-matrix-to-create-holographic-effect.291123/
        m[0, 0] = 2.0f * n / (r - l);
        m[0, 2] = (r + l) / (r - l);
        m[1, 1] = 2.0f * n / (t - b);
        m[1, 2] = (t + b) / (t - b);
        m[2, 2] = -(f + n) / (f - n);
        m[2, 3] = (-2.0f * f * n) / (f - n);
        m[3, 2] = -1.0f;

        return m;
    }
}

